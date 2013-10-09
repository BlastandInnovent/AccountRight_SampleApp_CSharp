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
//using CSharpSamples.GeneralJournal.Models;
using System.Collections.Generic;
using CSharpSamples.Common.Models;
using MYOB.AccountRight.SDK.Services.GeneralLedger;
using MYOB.AccountRight.SDK;
using MYOB.AccountRight.SDK.Communication;
using GL = MYOB.AccountRight.SDK.Contracts.Version2.GeneralLedger;
using GSTReportingMethod = MYOB.AccountRight.SDK.Contracts.Version2.GeneralLedger.GSTReportingMethod;

namespace CSharpSamples.GeneralJournal.Controllers
{
    [HandleError]
    public class GeneralJournalController : MutableBusinessController<GL.GeneralJournal, GeneralJournalService>
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateNew()
        {
            // TODO: Business Obj
            return View("Details", new GL.GeneralJournal
            {
                GSTReportingMethod = GSTReportingMethod.Purchase,
                DateOccurred = DateTime.Now,
                Lines = (new[] { new GL.GeneralJournalLine() }).ToList()
            }
            );
        }

        public ActionResult Details(string id)
        {
            var journal = GetById(id);

            if (journal == null)
                return Json(new { ok = false, message = "Record does not exists" });

            if (journal.Lines == null)
                journal.Lines = new List<GL.GeneralJournalLine>();
            
            return View(journal);
        }


        public JsonResult Save(GL.GeneralJournal journal)
        {
            try
            {
                findLinkObject(journal);
                if (journal.UID == Guid.Empty) // if Id is provided, it will update
                    Post(journal); 
                else
                    Put(journal, journal.UID.ToString());
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
            int gstReportingMethod, SortDescription sort, int? pageCount = null)
        {
            if ((string.IsNullOrEmpty(search) || string.IsNullOrEmpty(field)) && gstReportingMethod == -1 && !yearEndAdjustment.HasValue
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


            if (gstReportingMethod != -1)
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

        private TaxCodeService _taxCodeService;
        private TaxCodeService taxCodeService
        {
            get { return _taxCodeService ?? (_taxCodeService = new TaxCodeService(APIConfiguration, null, KeyService)); }
        }
        private JobService _jobService;
        private JobService jobService
        {
            get { return _jobService ?? (_jobService = new JobService(APIConfiguration, null, KeyService)); }
        }
        private AccountService _accountService;
        private AccountService accountService
        {
            get { return _accountService ?? (_accountService = new AccountService(APIConfiguration, null, KeyService)); }
        }
        private CategoryService _categoryService;
        private CategoryService categoryService
        {
            get { return _categoryService ?? (_categoryService = new CategoryService(APIConfiguration, null, KeyService)); }
        }
        private void findLinkObject(GL.GeneralJournal generalJournal)
        {
            if (generalJournal.Category != null && generalJournal.Category.UID == Guid.Empty)
            {
                var category = categoryService.GetRange(CompanyFile, "$filter=DisplayID eq '" + generalJournal.Category.DisplayID + "'",
                                         CompanyCredential).Items.FirstOrDefault();
                if (category != null)
                {
                    generalJournal.Category = new GL.CategoryLink()
                    {
                        DisplayID = category.DisplayID,
                        UID = category.UID
                    };
                }
            }
            foreach (var line in generalJournal.Lines)
            {
                if (line.TaxCode != null &&
                    line.TaxCode.UID == Guid.Empty)
                {
                    var tax =
                        taxCodeService.GetRange(CompanyFile, "$filter=Code eq '" + line.TaxCode.Code + "'",
                                                CompanyCredential).Items.FirstOrDefault();
                    if (tax != null)
                    {
                        line.TaxCode = new GL.TaxCodeLink()
                            {
                                Code = tax.Code,
                                UID = tax.UID
                            };
                    }
                    else
                        line.TaxCode = null;
                }

                if (line.Job != null &&
                    line.Job.UID == Guid.Empty)
                {
                    var job =
                        jobService.GetRange(CompanyFile, "$filter=Number eq '" + line.Job.Number + "'",
                                                CompanyCredential).Items.FirstOrDefault();
                    if (job != null)
                    {
                        line.Job = new GL.JobLink()
                            {
                                UID = job.UID
                            };
                    }
                    else
                        line.Job = null; 
                }

                if (line.Account != null &&
                    line.Account.UID == Guid.Empty)
                {
                    var account =
                        accountService.GetRange(CompanyFile, "$filter=DisplayID eq '" + line.Account.DisplayID + "'",
                                                CompanyCredential).Items.FirstOrDefault();
                    if (account != null)
                    {
                        line.Account = new GL.AccountLink()
                        {
                            UID = account.UID
                        };
                    }
                }
            }
        }

        public PartialViewResult AddNewLine()
        {
            return PartialView("_JournalLine", new GL.GeneralJournalLine());
        }
    }
}
