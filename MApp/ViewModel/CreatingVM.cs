using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MApp.Web.ViewModel
{
    /// <summary>
    /// view model for define view (Creating.cshtml)
    /// </summary>
    public class CreatingVM
    {
        public IssueModel Issue { get; set; }
        /// <summary>
        /// issue tags
        /// </summary>
        public List<TagModel> Tags { get; set; }
        /// <summary>
        /// all available tags
        /// </summary>
        public List<TagModel> AllTags { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        /// <summary>
        /// all available issues, used for linking issue to another one
        /// </summary>
        public List<IssueShort> Issues { get; set; }
        /// <summary>
        /// list of added tags, for posting
        /// </summary>
        public List<TagModel> AddedTags { get; set; }
        /// <summary>
        /// list of deleted tags, for posting
        /// </summary>
        public List<TagModel> DeletedTags { get; set; }
        /// <summary>
        /// list of all users
        /// </summary>
        public List<UserShortModel> AllUsers { get; set; }
        public List<AccessRightModel> AccessRights { get; set; }
        /// <summary>
        /// list of deleted access rights
        /// </summary>
        public List<AccessRightModel> DeletedAR { get; set; }
        /// <summary>
        /// list of added access rights
        /// </summary>
        public List<AccessRightModel> AddedAR { get; set; }
        public string AccessRight { get; set; }
        public int UserId { get; set; }
        public double SelfAssessmentValue { get; set; }
        public string SelfAssessmentDescription { get; set; }
        /// <summary>
        /// issue comments
        /// </summary>
        public List<CommentModel> Comments { get; set; }
        public List<NotificationModel> GroupthinkNotifications { get; set; }
        public List<KeyValuePair<string,List<string>>> GroupshiftProperties { get; set; }
        public KeyValuePair<string,int> UserWithMostChanges { get; set; }
        /// <summary>
        /// change counts of all users
        /// </summary>
        public List<KeyValuePair<UserShortModel,int>> AllUserChangeCounts { get; set; }
        public int UserChangesCount { get; set; }
        public List<UserChangeModel> UserChanges { get; set; }
        public int InfoCount { get; set; }
        public int ReadInfoCount { get; set; }
        public List<KeyValuePair<string,int>> UnreadInformation { get; set; }
        public UserChangeModel LastChange { get; set; }
        public List<UserChangeModel> Last100Changes { get; set; }
        public List<KeyValuePair<string,int>> GroupActivity { get; set; }
        public List<string> GroupTrustworthiness { get; set; }
        public List<string> DecisionTrustworthiness { get; set; }
    }
}