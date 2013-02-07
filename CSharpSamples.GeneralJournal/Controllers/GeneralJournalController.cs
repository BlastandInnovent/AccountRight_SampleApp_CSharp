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
using CSharpSamples.GeneralJournal.Models;
using System.Collections.Generic;
using CSharpSamples.Common.Models;

namespace CSharpSamples.GeneralJournal.Controllers
{
    [HandleError]
    public class GeneralJournalController : BusinessController<GeneralJournalModel>
    {        
        protected override string Module
        {
            get
            {
                return "GeneralJournal";
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateNew()
        {
            return View("Details", new GeneralJournalModel
            {
                GSTReportingMethod = GSTReportingMethod.Purchase,
                DateOccurred = DateTime.Now,
                Lines = (new[] { new JournalLineModel() }).ToList()
            }
            );
        }

        public ActionResult Details(string id)
        {
            var journal = GetById(id);

            if (journal == null)
                return Json(new { ok = false, message = "Record does not exists" });

            if (journal.Lines == null)
                journal.Lines = new List<JournalLineModel>();

            return View(journal);
        }


        public JsonResult Save(GeneralJournalModel journal)
        {
            try
            {	
				if(journal.IsNew)
                    Post(journal); // if Id is provided, it will update
                else
                    Put(journal, journal.Id);
            }
            catch (Exception ex)
            {
                return Json(new SaveResult { ok = false, message = "Failed to save " + ex.Message });
            }
            return Json(new SaveResult { ok = true, message = "" });
        }
                      
        
        public new JsonResult Delete(string id)
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

        public JsonResult Search(string field, string search, 
            bool? yearEndAdjustment,
            GSTReportingMethod gstReportingMethod, SortDescription sort, int? pageCount = null)
        {
            if ((string.IsNullOrEmpty(search) || string.IsNullOrEmpty(field)) && gstReportingMethod == GSTReportingMethod.Unknown && !yearEndAdjustment.HasValue
                && (sort==null || string.IsNullOrEmpty(sort.Field)))
                return Json(base.GetAll(pageCount));

            var searchCriteria = new List<SearchCriteria>();

            if (yearEndAdjustment.HasValue)
                searchCriteria.Add(new SearchCriteria
                {
                    Field = "IsYearEndAdjustment",
                    SearchText = yearEndAdjustment.Value.ToString().ToLower(),
                    FieldType = typeof(bool)
                });


            if (gstReportingMethod != GSTReportingMethod.Unknown)
                searchCriteria.Add(new SearchCriteria
                {
                    Field = "GSTReportingMethod",
                    SearchText = gstReportingMethod.GetHashCode().ToString(),
                    FieldType = typeof(string)
                });

            if (!string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(field))
            {
                if (field == "DateOccurred")
                {
                    searchCriteria.Add(new SearchCriteria
                    {
                        Field = field,
                        SearchText = search,
                        FieldType = GetSearchType(field),
                        OperandType = OperandType.GreaterThanOrEqual
                    });

                    searchCriteria.Add(new SearchCriteria
                    {
                        Field = field,
                        SearchText = Convert.ToDateTime(search).AddDays(1).ToString("yyyy-MM-dd"),
                        FieldType = GetSearchType(field),
                        OperandType = OperandType.LessThan
                    });
                }
                else
                {
                    searchCriteria.Add(new SearchCriteria
                    {
                        Field = field,
                        SearchText = search,
                        FieldType = GetSearchType(field)
                    });
                }
            }


            var sorting = sort == null ? Enumerable.Empty<SortDescription>() : new[] { sort };
            var model = GetPageItem(searchCriteria, LogicalOperator.and, sorting, pageCount);
            return Json(model);
        }

        private Type GetSearchType(string field)
        {
            switch (field)
            {
                case "IsYearEndAdjustment":
                    return typeof(bool);
                case "DateOccurred":
                    return typeof(DateTime);
            }

            return typeof(string);
        }

        public PartialViewResult AddNewLine()
        {
            return PartialView("_JournalLine", new JournalLineModel());
        }
    }
}
