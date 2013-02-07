using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CSharpSamples.Common.Helpers;
using System.Collections;

namespace CSharpSamples.Common.Models
{
    public class PageItem<T>
    {
        public List<T> Items { get; set; }
        public string NextPageLink { get; set; }
        public int? Count { get; set; }
        public string PrevPageLink { get; internal set; }
    }
}
