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
using CSharpSamples.Common;
using System.Collections;
using CSharpSamples.Common.Exceptions;
using CSharpSamples.Common.Models;
using CSharpSamples.Common.Utilities;
using Newtonsoft.Json;
using CSharpSamples.Common.Helpers;
using CSharpSamples.Common.Enums;
using GL = MYOB.AccountRight.SDK.Contracts.Version2.GeneralLedger;
using MYOB.AccountRight.SDK.Services.GeneralLedger;

namespace CSharpSamples.Account.Controllers
{
    public class AccountController : MutableBusinessController<GL.Account, AccountService>
    {

        private class AccountSubTypeMap
        {
            public string value { get { return SubTypeText; } }
            public string label { get { return SubTypeText; } }

            public GL.AccountType SubType { get; set; }
            public string SubTypeText { get { return SubType.ToString().AddSpacesToSentence(); } }
            public GL.Classification AccountType { get; set; }
            public int AccountTypePrefix { get; set; }
            public string AccountTypeText { get { return AccountType.ToString().AddSpacesToSentence(); } }
        }

        private TaxCodeService _taxCodeService;
        private TaxCodeService taxCodeService
        {
            get { return _taxCodeService ?? (_taxCodeService = new TaxCodeService(APIConfiguration, null, KeyService)); }
        }


        private IList<AccountSubTypeMap> GetAccountSubTypeMap()
        {
            var map =  new List<AccountSubTypeMap>();
            foreach (var type in Enum.GetValues(typeof(GL.Classification)))
            {
                map.AddRange(GetSubTypeByAccountType((GL.Classification)type).Select(subType => new AccountSubTypeMap
                {
                    AccountType = (GL.Classification)type,
                    AccountTypePrefix = ((GL.Classification)type).GetAccountNumberPrefix(),
                    SubType = subType
                }));
            }

            return map;
        }

        private IEnumerable<GL.AccountType> GetSubTypeByAccountType(GL.Classification type)
        {
            var lstSubAccounts = new List<GL.AccountType>();
            switch (type)
            {
                case GL.Classification.Asset:
                    lstSubAccounts.Add(GL.AccountType.Bank);
                    lstSubAccounts.Add(GL.AccountType.AccountReceivable);
                    lstSubAccounts.Add(GL.AccountType.OtherCurrentAsset);
                    lstSubAccounts.Add(GL.AccountType.FixedAsset);
                    lstSubAccounts.Add(GL.AccountType.OtherAsset);
                    break;

                case GL.Classification.Liability:
                    lstSubAccounts.Add(GL.AccountType.CreditCard);
                    lstSubAccounts.Add(GL.AccountType.AccountsPayable);
                    lstSubAccounts.Add(GL.AccountType.OtherCurrentLiability);
                    lstSubAccounts.Add(GL.AccountType.LongTermLiability);
                    lstSubAccounts.Add(GL.AccountType.OtherLiability);
                    break;

                case GL.Classification.Equity:
                    lstSubAccounts.Add(GL.AccountType.Equity);
                    break;

                case GL.Classification.Income:
                    lstSubAccounts.Add(GL.AccountType.Income);
                    break;

                case GL.Classification.CostOfSales:
                    lstSubAccounts.Add(GL.AccountType.CostOfSales);
                    break;

                case GL.Classification.Expense:
                    lstSubAccounts.Add(GL.AccountType.Expense);
                    break;

                case GL.Classification.OtherIncome:
                    lstSubAccounts.Add(GL.AccountType.OtherIncome);
                    break;

                case GL.Classification.OtherExpense:
                    lstSubAccounts.Add(GL.AccountType.OtherExpense);
                    break;   
            }
            return lstSubAccounts;
        }

        public ActionResult Index()
        {
            ViewBag.SubTypes = new SelectList(EnumHelper.GetEnumList(typeof(GL.AccountType), SetSubTypeText), "key", "value");
            ViewBag.AccountTypes = new SelectList(EnumHelper.GetEnumList(typeof(GL.Classification), SetAccountTypeText), "key", "value");
            return View();
        }

        private string SetSubTypeText(int value, string name)
        {
            return name.AddSpacesToSentence();
        }
        private string SetAccountTypeText(int value, string name)
        {
            return name.AddSpacesToSentence();
        }


        private IEnumerable<SortDescription> DefaultSorting()
        {
            return new[]
                {
                    new SortDescription
                        {
                            Field = "Classification",
                            Direction = SortDirection.asc
                        },
                    new SortDescription
                        {
                            Field = "DisplayID",
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
            return View("Details", new GL.Account
            {
                Name = "New Account",
                DisplayID= "",
                //Number = "",
                Type = GL.AccountType.OtherAsset,
                Classification = GL.Classification.Asset,
                IsActive = true,
                TaxCode = new GL.TaxCodeLink() { Code = "N-T" },
                BankingDetails = new GL.BankingDetails()
            }
            );
        }

        private GL.Classification GetAccountTypeBySubType(GL.AccountType subType)
        {
            var mapTypes = GetAccountSubTypeMap();

            return mapTypes.SingleOrDefault(m => m.SubType == subType).AccountType;
        }

        public JsonResult SaveAccount(GL.Account account)
        {
            try
            {
                if (account.TaxCode != null && account.TaxCode.UID == Guid.Empty)
                {
                    var tax =  taxCodeService.GetRange(CompanyFile, "$filter=Code eq '" + account.TaxCode.Code + "'",
                                             CompanyCredential).Items.FirstOrDefault();
                    if (tax != null)
                    {
                        account.TaxCode = new GL.TaxCodeLink()
                            {
                                Code = tax.Code,
                                UID = tax.UID
                            };
                    }
                }

                if (account.UID == Guid.Empty)
                {
                    account.DisplayID = string.Format("{0}-{1}", GetAccountTypeBySubType(account.Type).GetHashCode(), account.Number);
                    Post(account);
                }
                else
                    Put(account, account.UID.ToString());
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
                account.BankingDetails = new GL.BankingDetails();

            return View(account);
        }

        public JsonResult SearchAccount(string field, string search, SubAccountType subType, AccountType accountType, bool? isHeader, bool? isActive, int? pageCount = null)
        {
            if (string.IsNullOrEmpty(search) && !isHeader.HasValue && !isActive.HasValue &&
                subType == SubAccountType.Unknown && accountType == AccountType.Unknown)
            {
                return Json(GetPageItem(null, LogicalOperator.and, DefaultSorting(), pageCount));
            }

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
                    Field = "Type",
                    SearchText = subType.GetHashCode().ToString(),
                    FieldType = typeof(string)
                });

            if (accountType != AccountType.Unknown)
                searchCriteria.Add(new SearchCriteria
                {
                    Field = "Classification",
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
                    Field = "IsActive",
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
