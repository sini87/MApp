using MApp.Middleware.Models;
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
        public string Title { get; set; }
        public string Desc { get; set; }
        public List<IssueShort> Issues { get; set; }
        public List<TagModel> AddedTags { get; set; }
        public List<TagModel> DeletedTags { get; set; }
        public List<UserShortModel> AllUsers { get; set; }
        public List<AccessRightModel> AccessRights { get; set; }
        public List<AccessRightModel> DeletedAR { get; set; }
        public List<AccessRightModel> AddedAR { get; set; }
        public string AccessRight { get; set; }
        public int UserId { get; set; }
        public double SelfAssessmentValue { get; set; }
        public string SelfAssessmentDescription { get; set; }
    }
}