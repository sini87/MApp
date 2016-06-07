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
    /// <summary>
    /// performs operations on AlternativeModel
    /// </summary>
    public class IssueBrAlternative
    {
        /// <summary>
        /// returns a list of alternatives to an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<AlternativeModel> GetIssueAlternatives(int issueId, int userId)
        {
            AlternativeModel am = new AlternativeModel();
            List<AlternativeModel> list = am.ToModelList(AlternativeOp.GetIssueAlternatives(issueId, userId),am);
            List<CommentModel> comments = GetComments(issueId, userId);

            foreach (AlternativeModel a in list)
            {
                a.Comments = comments.Where(x => x.Type == "Alternative" + a.Id).ToList();
            }

            return list;
        }

        /// <summary>
        /// updates alternatives for an issue
        /// </summary>
        /// <param name="updatedAlternatives">existing alternatives</param>
        /// <param name="deletedAlternatives">deleted alternatives</param>
        /// <param name="userId">user who is performing this operation</param>
        public void UpdateAlternatives(List<AlternativeModel> updatedAlternatives, List<int> deletedAlternatives, int userId)
        {
            AlternativeModel am = new AlternativeModel();
            List<Alternative> updateList = am.ToEntityList(updatedAlternatives.Where(x => x.Id > 0).ToList());
            List<Alternative> addedList = am.ToEntityList(updatedAlternatives.Where(x => x.Id == -1).ToList());
            AlternativeOp.DeleteAlternatives(deletedAlternatives, userId);
            AlternativeOp.UpdateAlternatives(updateList, userId);
            AlternativeOp.AddAlternatives(addedList, userId);
        }
        
        /// <summary>
        /// returns List of comments for issue alternatives
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<CommentModel> GetComments(int issueId, int userId)
        {
            CommentModel cm = new CommentModel();
            List<CommentModel> list = cm.ToModelList(CommentOp.GetAlternativeComments(issueId, userId), cm);
            List<KeyValuePair<int, string>> userList = UserOp.GetUserNames(list.Select(x => x.UserId).Distinct().ToList());
            List<KeyValuePair<int, string>> anonymousUsers = new List<KeyValuePair<int, string>>();
            List<KeyValuePair<int, string>> anonNames = new List<KeyValuePair<int, string>>();
            int i = 1;
            Random rnd = new Random();
            KeyValuePair<int, string> randName;
            List<int> usedRands = new List<int>();
            bool added;
            int rNumber = 0;
            foreach (KeyValuePair<int, string> userkvp in userList)
            {
                added = false;
                while (added == false)
                {
                    rNumber = rnd.Next(1, 999999);
                    if (!usedRands.Contains(rNumber))
                    {
                        usedRands.Add(rNumber);
                        added = true;
                    }
                }
                anonNames.Add(new KeyValuePair<int, string>(userkvp.Key, "Anonymous " + rNumber));
                i++;
            }

            foreach (CommentModel model in list)
            {
                if (model.Anonymous)
                {
                    model.Name = anonNames.Where(x => x.Key == model.UserId).FirstOrDefault().Value;
                }
                else
                {
                    model.Name = userList.Where(x => x.Key == model.UserId).FirstOrDefault().Value;
                }
            }

            return list;
        }

        /// <summary>
        /// marks issue alternatives as read/seen
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        public bool MarkAsRead(int issueId, int userId)
        {
            return InformationReadOp.MarkAlternatives(issueId, userId);
        }

        /// <summary>
        /// marks alternative comments as read
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        public void MarkCommentsAsRead(int issueId, int userId)
        {
            InformationReadOp.MarkAlternativeComments(issueId, userId);
        }
    }
}
