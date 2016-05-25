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
        public List<CommentModel> Comments { get; set; }
        public List<NotificationModel> GroupthinkNotifications { get; set; }
        public List<KeyValuePair<string,List<string>>> GroupshiftProperties { get; set; }
        public KeyValuePair<string,int> UserWithMostChanges { get; set; }
        public List<KeyValuePair<UserShortModel,int>> AllUserChangeCounts { get; set; }
        public int UserChangesCount { get; set; }
        public List<UserChangeModel> UserChanges { get; set; }
        public int InfoCount { get; set; }
        public int ReadInfoCount { get; set; }
        public List<KeyValuePair<string,int>> UnreadInformation { get; set; }
        public UserChangeModel LastChange { get; set; }
        public List<UserChangeModel> Last100Changes { get; set; }
    }
}