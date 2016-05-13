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
            TagOp.AddTagsToIssue(tm.ToEntityList(addedTags), issueId);
            TagOp.RemoveTagsFromIssue(tm.ToEntityList(deletedTags.Where(x => x.Id > 0).ToList()), issueId);
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

                arm.SelfAssessmentHistory = new List<SelfAssessmentHEntry>();
                foreach (HAccessRight har in AccessRightOp.GetAccessRightsHistorical(ar.UserId, issueId))
                {
                    arm.SelfAssessmentHistory.Add( new SelfAssessmentHEntry(har.ChangeDate, Convert.ToDouble(har.SelfAssessmentValue), har.SelfAssesmentDescr));
                }

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

        public bool DeleteIssue(int issueId)
        {
            return IssueOp.DeleteIssue(issueId);
        }

        public void NextStage (int issueId, int userId)
        {
            IssueOp.NextStage(issueId, userId);
        }

        public AccessRightModel AccessRightOfUserForIssue(int userId, int issueId)
        {
            AccessRightModel arm = new AccessRightModel();
            AccessRightModel ar = arm.ToModel(AccessRightOp.AccessRightOfUserForIssue(userId, issueId));
            return ar;
        }
    }
}
