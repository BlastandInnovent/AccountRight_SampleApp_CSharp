//  File:        AccountController.cs
//  Copyright:   Copyright 2012 MYOB Technology Pty Ltd. All rights reserved.
//  Website:     http://www.myob.com
//  Author:      MYOB
//  E-mail:      info@myob.com
//
//Documentation, code and sample applications provided by MYOB Australia are for 
//information purposes only. MYOB Technology Pty Ltd and its suppliers make no 
//warranties, either express or implied, in this document. 
//
//Information in this document or code, including website references, is subject
//to change without notice. Unless otherwise noted, the example companies, 
//organisations, products, domain names, email addresses, people, places, and 
//events are fictitious. 
//
//The entire risk of the use of this documentation or code remains with the user. 
//Complying with all applicable copyright laws is the responsibility of the user. 
//
//Copyright 2012 MYOB Technology Pty Ltd. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSharpSamples.Account.Models;
using CSharpSamples.Common;
using System.Collections;
using CSharpSamples.Common.Exceptions;
using Newtonsoft.Json;
using CSharpSamples.Common.Models;
using CSharpSamples.Common.Helpers;
using CSharpSamples.Common.Enums;

namespace CSharpSamples.Account.Controllers
{
    public class AccountController : BusinessController<AccountModel>
    {
        protected override string Module
        {
            get
            {
                return "Account";
            }
        }

        private class AccountSubTypeMap
        {
            public string value { get { return SubTypeText; } }
            public string label { get { return SubTypeText; } }

            public SubAccountType SubType { get; set; }
            public string SubTypeText { get { return SubType.ToString().AddSpacesToSentence(); } }
            public AccountType AccountType { get; set; }
            public string AccountTypeText { get { return AccountType.ToString().AddSpacesToSentence(); } }
        }

        private IList<AccountSubTypeMap> GetAccountSubTypeMap()
        {
            var map =  new List<AccountSubTypeMap>();
            foreach (var type in Enum.GetValues(typeof(AccountType)))
            {
                map.AddRange(GetSubTypeByAccountType((AccountType)type).Select(subType => new AccountSubTypeMap
                {
                    AccountType = (AccountType)type,
                    SubType = subType
                }));
            }

            return map;
        }

        private IEnumerable<SubAccountType> GetSubTypeByAccountType(AccountType type)
        {
            var lstSubAccounts = new List<SubAccountType>();
            switch (type)
            {
                case AccountType.Asset:
                    lstSubAccounts.Add(SubAccountType.Bank);
                    lstSubAccounts.Add(SubAccountType.AccountReceivable);
                    lstSubAccounts.Add(SubAccountType.OtherCurrentAsset);
                    lstSubAccounts.Add(SubAccountType.FixedAsset);
                    lstSubAccounts.Add(SubAccountType.OtherAsset);
                    break;

                case AccountType.Liability:
                    lstSubAccounts.Add(SubAccountType.CreditCard);
                    lstSubAccounts.Add(SubAccountType.AccountsPayable);
                    lstSubAccounts.Add(SubAccountType.OtherCurrentLiability);
                    lstSubAccounts.Add(SubAccountType.LongTermLiability);
                    lstSubAccounts.Add(SubAccountType.OtherLiability);
                    break;

                case AccountType.Equity:
                    lstSubAccounts.Add(SubAccountType.Equity);
                    break;

                case AccountType.Income:
                    lstSubAccounts.Add(SubAccountType.Income);
                    break;

                case AccountType.CostOfSales:
                    lstSubAccounts.Add(SubAccountType.CostOfSales);
                    break;

                case AccountType.Expense:
                    lstSubAccounts.Add(SubAccountType.Expense);
                    break;

                case AccountType.OtherIncome:
                    lstSubAccounts.Add(SubAccountType.OtherIncome);
                    break;

                case AccountType.OtherExpense:
                    lstSubAccounts.Add(SubAccountType.OtherExpense);
                    break;   
            }
            return lstSubAccounts;
        }

        public ActionResult Index()
        {
            ViewBag.SubTypes = new SelectList(EnumHelper.GetEnumList(typeof(SubAccountType), SetSubTypeText), "key", "value");
            ViewBag.AccountTypes = new SelectList(EnumHelper.GetEnumList(typeof(AccountType), SetAccountTypeText), "key", "value");
            return View();
        }

        private string SetSubTypeText(int value, string name)
        {
            if((SubAccountType)value == SubAccountType.Unknown)
                return string.Empty;

            return name.AddSpacesToSentence();
        }
        private string SetAccountTypeText(int value, string name)
        {
            if ((AccountType)value == AccountType.Unknown)
                return string.Empty;

            return name.AddSpacesToSentence();
        }


        private IEnumerable<SortDescription> DefaultSorting()
        {
            return new[] { 
            new SortDescription{
                Field = "AccountType",
                Direction = SortDirection.asc
            },
            new SortDescription{
                Field = "AccountNumber",
                Direction = SortDirection.asc
            }
            };
        }
        public JsonResult GetAllAccounts()
        {
            var accounts = GetAll(null, LogicalOperator.and, DefaultSorting());

            return Json(accounts);
        }
        public ActionResult CreateNewAccount()
        {
            ViewBag.SubTypes = JsonConvert.SerializeObject(GetAccountSubTypeMap());
            return View("Details", new AccountModel
                {
                    AccountName = "New Account",
                    AccountNumber = "",
                    SubType = SubAccountType.OtherAsset,
                    AccountType = AccountType.Asset,
                    IsInactive = false,
                    TaxCodeId= "N-T",
                    BankingDetails = new BankingDetailsModel()
                }
            );
        }

        private AccountType GetAccountTypeBySubType(SubAccountType subType)
        {
            var mapTypes = GetAccountSubTypeMap();

            return mapTypes.SingleOrDefault(m => m.SubType == subType).AccountType;
        }

        public JsonResult SaveAccount(AccountModel account)
        {
            try
            {
                if (account.IsNew)
                {
                    account.Id = string.Format("{0}-{1}", GetAccountTypeBySubType(account.SubType).GetHashCode(), account.AccountNumber);
                    Post(account);
                }
                else
                    Put(account, account.Id);
            }
            catch (Exception ex)
            {
                return Json(new SaveResult { ok = false, message = "Failed to save " + ex.Message });
            }
            return Json(new SaveResult { ok = true, message = "" });
        }
        public ActionResult Details(string id)
        {
            ViewBag.SubTypes = JsonConvert.SerializeObject(GetAccountSubTypeMap());

            ViewBag.SubTypes2 = new SelectList(GetAccountSubTypeMap(), "", "");
            var account = GetById(id);

            if (account == null)
                return Json(new { ok = false, message = "Record does not exists" });

            if (account.BankingDetails == null)
                account.BankingDetails = new BankingDetailsModel();

            return View(account);
        }

        public JsonResult SearchAccount(string field, string search, SubAccountType subType, AccountType accountType, bool? isHeader, bool? isActive, int? pageCount = null)
        {
            if (string.IsNullOrEmpty(search) && !isHeader.HasValue && !isActive.HasValue && subType == SubAccountType.Unknown && accountType == AccountType.Unknown)
                return Json(GetAll(pageCount));

            var searchCriteria = new List<SearchCriteria>();

            if(!string.IsNullOrEmpty(search))
                searchCriteria.Add(new SearchCriteria
                {
                    Field = field,
                    SearchText = search,
                    FieldType = field.Contains("Balance")? typeof(decimal): typeof(string)
                });

            if (subType != SubAccountType.Unknown)
                searchCriteria.Add(new SearchCriteria
                {
                    Field = "SubType",
                    SearchText = subType.GetHashCode().ToString(),
                    FieldType = typeof(string)
                });

            if (accountType != AccountType.Unknown)
                searchCriteria.Add(new SearchCriteria
                {
                    Field = "AccountType",
                    SearchText = accountType.GetHashCode().ToString(),
                    FieldType = typeof(string)
                });
            if (isHeader.HasValue)
            {
                searchCriteria.Add(new SearchCriteria
                {
                    Field = "IsHeader",
                    SearchText = isHeader.Value.ToString().ToLower(),
                    FieldType = typeof(bool)
                });
            }

            if (isActive.HasValue)
            {
                searchCriteria.Add(new SearchCriteria
                {
                    Field = "IsInactive",
                    SearchText = (!isActive.Value).ToString().ToLower(),
                    FieldType = typeof(bool)
                });

            }

            return Json(GetPageItem(searchCriteria, LogicalOperator.and, DefaultSorting(), pageCount));
        }

        public JsonResult DeleteAccount(string id)
        {
            try
            {
                base.Delete(id);
            }
            catch (Exception ex)
            {
                return Json(new SaveResult { ok = false, message = "Failed to delete " + ex.Message });
            }

            return Json(new SaveResult { ok = true });
        }
    }
}
