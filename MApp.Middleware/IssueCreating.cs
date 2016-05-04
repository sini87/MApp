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

        public int SaveIssue(IssueModel issueModel, int userId)
        {
            Issue issue = issueModel.ToEntity();
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
                return IssueOp.UpdateIssue(issue, userId);
            }else
            {
                return IssueOp.InsertIssue(issue, userId);
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
            Dictionary<int, string> arDict = IssueOp.GetAccessRightsForIssue(issueId);
            string right;
            string name;
            foreach(KeyValuePair<int,string> ar in arDict)
            {
                right = ar.Value;
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
                if (userList == null)
                {
                    list.Add(new AccessRightModel(ar.Key, right));
                }else
                {
                    name = userList.Find(x => x.Id == ar.Key).FirstName + " " + userList.Find(x => x.Id == ar.Key).LastName;
                    list.Add(new AccessRightModel(ar.Key, right, name));
                }
                
            }
            return list;
        }
    }
}
