using MApp.DA;
using MApp.DA.Repository;
using MApp.Middleware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware
{
    public class IssueCreating
    {
        public List<UserShortModel> userList;
        public IssueCreating()
        {

        }

        public IssueModel GetIssue(int issueId)
        {
            IssueModel im = new IssueModel();
            im = im.ToModel(IssueOp.GetIssueById(issueId));
            if (im.Parent != null)
            {
                int parrentIssueId = Convert.ToInt32(im.Parent);
                im.ParentTitle = IssueOp.GetIssueById(parrentIssueId).Title;
            }else
            {
                im.Parent = -1;
            }
            if (im.DependsOn != null)
            {
                int dependsOnIssueId = Convert.ToInt32(im.DependsOn);
                im.DependsOnTitle = IssueOp.GetIssueById(dependsOnIssueId).Title;
            }
            else
            {
                im.DependsOn = -1;
            }

            TagModel tm = new TagModel();

            im.Tags = tm.ToModelList(TagOp.GetIssueTags(issueId),tm);

            return im;
        }

        public List<TagModel> GetIssueTags(int issueId)
        {
            TagModel tm = new TagModel();
            return tm.ToModelList(TagOp.GetIssueTags(issueId),tm);
        }

        public List<TagModel> GetAllTags()
        {
            TagModel tm = new TagModel();
            return tm.ToModelList(TagOp.GetAllTags(), tm);
        }

        public List<IssueShort> GetUserIssuesShort(int userId)
        {
            List<IssueShort> list = new List<IssueShort>();
            foreach(Issue issue in IssueOp.UserIssues(userId))
            {
                list.Add(new IssueShort(issue.Id, issue.Title));
            }
            return list;
        }

        public int SaveIssue(IssueModel issueModel, int userId, double selfAssessmentValue, string selfAssessmentDescr)
        {
            Issue issue = issueModel.ToEntity();
            int issueId = -1;
            if (issue.Parent == -1)
            {
                issue.Parent = null;
            }
            if (issue.DependsOn == -1)
            {
                issue.DependsOn = null;
            }
            if (issue.Id > 0)
            {
                issueId = IssueOp.UpdateIssue(issue, userId);
                if (issueModel.Status == "CREATING" || issueModel.Status == "BRAINSTORMING1")
                {
                    AccessRightOp.UpdateSelfAssesment(selfAssessmentValue, selfAssessmentDescr, issueModel.Id, userId);
                }
                return issueId;
            }else
            {
                issueId = IssueOp.InsertIssue(issue, userId, selfAssessmentValue, selfAssessmentDescr);
                return issueId;
            }
        }

        public void UpdateIsseuTags(int issueId, List<TagModel> addedTags, List<TagModel> deletedTags, int userId)
        {
            TagModel tm = new TagModel();
            TagOp.AddTagsToIssue(tm.ToEntityList(addedTags), issueId, userId);
            TagOp.RemoveTagsFromIssue(tm.ToEntityList(deletedTags.Where(x => x.Id > 0).ToList()), issueId, userId);
        }

        /// <summary>
        /// returns all Users
        /// </summary>
        /// <returns></returns>
        public List<UserShortModel> GetAllUsers()
        {
            List<UserShortModel> list = new List<UserShortModel>();
            foreach (User u in UserOp.GetAllUsers())
            {
                list.Add(new UserShortModel(u.Id, u.FirstName, u.LastName));
            }
            userList = list;
            return userList;
        }

        /// <summary>
        /// returns list of access right by IssueId
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns>Accessright with UserId and full Username</returns>
        public List<AccessRightModel> GetAccessRightsOfIssue(int issueId)
        {
            List<AccessRightModel> list = new List<AccessRightModel>();
            List<AccessRight> arEntityList = AccessRightOp.GetAccessRightsForIssue(issueId);
            string right;
            string name;
            AccessRightModel arm;
            foreach(AccessRight ar in arEntityList)
            {
                right = ar.Right;
                switch (right)
                {
                    case "O":
                        right = "Owner";
                        break;
                    case "C":
                        right = "Contributor";
                        break;
                    case "V":
                        right = "Viewer";
                        break;

                }
                arm = new AccessRightModel();
                arm = arm.ToModel(ar);
                arm.Right = right;

                arm.SelfAssessmentHistory = GetSelfAssessmentHistoryForAr(ar.UserId, issueId);

                if (userList == null)
                {
                    list.Add(arm);
                }else
                {
                    arm.Name = userList.Find(x => x.Id == arm.UserId).FirstName + " " + userList.Find(x => x.Id == arm.UserId).LastName;
                    list.Add(arm);
                }
                
            }
            return list;
        }

        private List<SelfAssessmentHEntry> GetSelfAssessmentHistoryForAr(int userId, int issueId)
        {
            List<SelfAssessmentHEntry> list = new List<SelfAssessmentHEntry>();
            foreach (HAccessRight har in AccessRightOp.GetAccessRightsHistorical(userId, issueId))
            {
                list.Add(new SelfAssessmentHEntry(har.ChangeDate, Convert.ToDouble(har.SelfAssessmentValue), har.SelfAssesmentDescr));
            }
            return list;
        }

        public AccessRightModel GetAccessRight(int issueId, int userId)
        {
            AccessRightModel arm = new AccessRightModel();
            arm = arm.ToModel(AccessRightOp.GetAccessRight(issueId, userId));
            arm.SelfAssessmentHistory = GetSelfAssessmentHistoryForAr(userId, issueId);
            return arm;
        }

        /// <summary>
        /// updates accessrights which user modified
        /// </summary>
        /// <param name="addedAr">new granted permissions</param>
        /// <param name="deletedAr">removed permissions</param>
        /// <param name="updatedAr">updated permissions</param>
        /// <param name="issueId">Issue for permissions</param>
        /// <param name="userId">user who is making changes</param>
        public void UpdateAccessRights(List<AccessRightModel> addedAr, List<AccessRightModel> deletedAr, List<AccessRightModel> updatedAr, int issueId, int userId)
        {
            AccessRightModel arm = new AccessRightModel();
            List<AccessRight> dList = arm.ToEntityList(deletedAr);
            List<AccessRight> aList = arm.ToEntityList(addedAr);
            List<AccessRight> uList = null;
            if (updatedAr != null)
            {
                uList = arm.ToEntityList(updatedAr).Except(dList).Except(aList).ToList();
            }
            AccessRightOp.UpdateRights(aList, dList, uList, issueId, userId);
        }

        /// <summary>
        /// deltetes an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public bool DeleteIssue(int issueId)
        {
            return IssueOp.DeleteIssue(issueId);
        }

        /// <summary>
        /// puts the issue to next stage
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        public void NextStage (int issueId, int userId)
        {
            IssueOp.NextStage(issueId, userId);
        }

        /// <summary>
        /// returns accessright for user to an issue
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public AccessRightModel AccessRightOfUserForIssue(int userId, int issueId)
        {
            AccessRightModel arm = new AccessRightModel();
            AccessRightModel ar = arm.ToModel(AccessRightOp.AccessRightOfUserForIssue(userId, issueId));
            return ar;
        }

        /// <summary>
        /// adds a comment to an alternative
        /// </summary>
        /// <param name="commentModel"></param>
        /// <param name="userId"></param>
        public void AddCommentToAlternative(CommentModel commentModel, int userId)
        {
            Comment cmt = new Comment();
            cmt.Anonymous = false;
            cmt.Text = commentModel.Text;
            cmt.IssueId = commentModel.IssueId;
            cmt.UserId = userId;
            cmt.Type = commentModel.Type;
            CommentOp.AddComment(cmt);
        }

        /// <summary>
        /// returns all comments for an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CommentModel> GetIssueComments(int issueId, int userId)
        {
            CommentModel cm = new CommentModel();
            List<CommentModel> list = cm.ToModelList(CommentOp.GetTypeComments(issueId, userId,"Issue"), cm);
            List<KeyValuePair<int, string>> userList = UserOp.GetUserNames(list.Select(x => x.UserId).Distinct().ToList());

            foreach (CommentModel model in list)
            {
                model.Name = userList.Where(x => x.Key == model.UserId).FirstOrDefault().Value;
            }

            return list;
        }

        /// <summary>
        /// gets grouphtink notifications
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<NotificationModel> GetGroupthinkNotifications(int issueId, int userId)
        {
            NotificationModel nm = new NotificationModel();
            return nm.ToModelList(NotificationOp.GetGroupthinkNotifications(issueId, userId), nm);
        }

        /// <summary>
        /// adds a new groupthink notification
        /// </summary>
        /// <param name="notification"></param>
        public int SendNotification(NotificationModel notification)
        {
            return NotificationOp.AddNotification(notification.ToEntity(notification));
        }

        /// <summary>
        /// marks an notification as read
        /// </summary>
        /// <param name="notificationId"></param>
        public void MarkNotificationAsRead(int notificationId)
        {
            NotificationOp.MarkNotificationAsRead(notificationId);
        }

        /// <summary>
        /// adds an access right to the issue
        /// </summary>
        /// <param name="accessRight"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool AddAccessRight(AccessRightModel accessRight, int userId)
        {
            AccessRightModel arm = new AccessRightModel();
            return AccessRightOp.AddAccessRight(arm.ToEntity(accessRight), userId);
        }

        /// <summary>
        /// removes an accessright from issue
        /// </summary>
        /// <param name="accessRight"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool RemoveAccessRight(AccessRightModel accessRight, int userId)
        {
            AccessRightModel arm = new AccessRightModel();
            return AccessRightOp.RemoveAccessRight(arm.ToEntity(accessRight), userId);
        }

        /// <summary>
        /// returns a list of key-value pairs
        /// the key is the name of the property
        /// the value is a list of users who share the property/skill
        /// the threshold is currently 50%
        /// so if 50% of the users of an issue share the same property 
        /// this method will return these properties with the users
        /// only 'Owners' and 'Contributors' are considered, not owners
        /// minimum 2 users must have accessright to the issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public List<KeyValuePair<string,List<String>>> GetGropshiftProperties(int issueId)
        {
            return NotificationOp.GetGroupshiftProperties(issueId);
        }

        /// <summary>
        /// makrs issue core infomations as read
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        public void MarkAsRead(int issueId, int userId)
        {
            InformationReadOp.MarkIssue(issueId, userId);
        }

        /// <summary>
        /// marks issue comments as read
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        public void MarkCommentsAsRead(int issueId, int userId)
        {
            InformationReadOp.MarkIssueComments(issueId, userId);
        }

        /// <summary>
        /// returns users with the count of changes they made
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns>list of key value pairs, key is the userId and value the count of changes</returns>
        public KeyValuePair<string,int> UserWithMostChanges(int issueId)
        {
            return ChangesOp.UserWithMostChanges(issueId);
        }

        /// <summary>
        /// returns users with the count of changes they made
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns>list of key value pairs, key is the userId and value the count of changes</returns>
        public List<KeyValuePair<UserShortModel,int>> GetAllChangeCountsByUser(int issueId)
        {
            List<KeyValuePair<UserShortModel, int>> list = new List<KeyValuePair<UserShortModel, int>>();
            List<KeyValuePair<int, int>> countList = ChangesOp.GetAllChangeCountsByUser(issueId);
            KeyValuePair<UserShortModel, int> userCntKvp;

            if(userList == null || userList.Count() == 0)
            {
                GetAllUsers();
            }

            foreach(KeyValuePair<int,int> cntKvp in countList)
            {
                userCntKvp = new KeyValuePair<UserShortModel, int>(userList.Find(x => x.Id == cntKvp.Key), cntKvp.Value);
                list.Add(userCntKvp);
            }

            return list;
        }

        /// <summary>
        /// returns the count of changes made by user
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetUserChangesCount(int issueId, int userId)
        {
            return ChangesOp.GetUserChangesCount(issueId, userId);
        }

        public int GetInfoCountForUser(int issueId, int userId)
        {
            return InformationReadOp.GetInfosCount(issueId, userId);
        }

        public int GetReadInfoCountForUser(int issueId, int userId)
        {
            return InformationReadOp.GetReadInfosCount(issueId, userId);
        }

        public List<KeyValuePair<string,int>> GetUnreadInformation(int issueId, int userId)
        {
            return InformationReadOp.GetUnreadInfos(issueId, userId);
        }

        public List<UserChangeModel> GetUserChanges(int issueId, int userId)
        {
            UserChangeModel ucm = new UserChangeModel();
            return ucm.ToModelList(ChangesOp.GetUserChanges(issueId, userId), ucm);
        }

        public UserChangeModel GetLastChange(int issueId)
        {
            UserChangeModel ucm = new UserChangeModel();
            ucm = ucm.ToModel(ChangesOp.LastChange(issueId));
            if (userList == null || userList.Count == 0)
            {
                GetAllUsers();
            }
            ucm.Name = userList.Find(x => x.Id == ucm.UserId).Name;
            return ucm;
        }

        public List<UserChangeModel> GetLast100Changes(int issueId)
        {
            List<UserChangeModel> changesList;
            UserChangeModel ucm = new UserChangeModel();
            if (userList == null || userList.Count == 0)
            {
                GetAllUsers();
            }
            changesList = ucm.ToModelList(ChangesOp.GetLast100Changes(issueId), ucm);

            foreach(UserChangeModel change in changesList)
            {
                change.Name = userList.Find(x => x.Id == change.UserId).Name;
            }

            return changesList;
        }
    }
}
