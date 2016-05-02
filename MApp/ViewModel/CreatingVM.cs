using MApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    public class CreatingVM
    {
        public IssueModel Issue { get; set; }
        public List<TagModel> Tags { get; set; }
        public List<TagModel> AllTags { get; set; }
    }
}