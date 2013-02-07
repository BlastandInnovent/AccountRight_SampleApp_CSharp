//  File:        GeneralJournalController.cs
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
using System.Linq;
using System.Web.Mvc;
using CSharpSamples.Common;
using CSharpSamples.JournalTransactions.Models;
using System.Collections.Generic;
using CSharpSamples.Common.Helpers;

namespace CSharpSamples.JournalTransactions.Controllers
{
    [HandleError]
    public class JournalTransactionsController : BusinessController<JournalTransactionModel>
    {        
        protected override string Module
        {
            get
            {
                return "JournalTransaction";
            }
        }

        public ActionResult Index()
        {
            ViewBag.JournalTypes = new SelectList(EnumHelper.GetEnumList(typeof(JournalType), (value, name) => name.AddSpacesToSentence()), "key", "value", JournalType.General.GetHashCode());
            return View();
        }

        public ActionResult Details(string id)
        {
            var journal = GetById(id);

            if (journal == null)
                return Json(new { ok = false, message = "Record does not exists" });

            return PartialView(journal);
        }

        public JsonResult Search(string field, string searchText, JournalType journalType, SortDescription sort, int? pageCount)
        {
            if ((string.IsNullOrEmpty(searchText) || string.IsNullOrEmpty(field)) 
                && journalType == JournalType.All 
                && (sort==null || string.IsNullOrEmpty(sort.Field)))
                return Json(GetAll(pageCount));

            var searchCriteria = new List<SearchCriteria>();

            if (journalType != JournalType.All)
                searchCriteria.Add(new SearchCriteria
                {
                    Field = "JournalType",
                    SearchText = journalType.GetHashCode().ToString(),
                    FieldType = typeof(int)
                });


            if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(field))
            {
                if (field == "DateOccurred" || field == "DatePosted")
                {
                    searchCriteria.Add(new SearchCriteria
                    {
                        Field = field,
                        SearchText = searchText,
                        FieldType = typeof(DateTime),
                        OperandType = OperandType.GreaterThanOrEqual
                    });

                    searchCriteria.Add(new SearchCriteria
                    {
                        Field = field,
                        SearchText = Convert.ToDateTime(searchText).AddDays(1).ToString("yyyy-MM-dd"),
                        FieldType = typeof(DateTime),
                        OperandType = OperandType.LessThan
                    });
                }
                else
                {
                    searchCriteria.Add(new SearchCriteria
                    {
                        Field = field,
                        SearchText = searchText,
                        FieldType = typeof(string)
                    });
                }
            }

            var sorting = sort == null ? Enumerable.Empty<SortDescription>() : new[] { sort };
            var model = GetPageItem(searchCriteria, LogicalOperator.and, sorting, pageCount);
            return Json(model);
        }
    }
}
