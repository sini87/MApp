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

            foreach (CommentModel model in list)
            {
                model.Name = userList.Where(x => x.Key == model.UserId).FirstOrDefault().Value;
            }

            return list;
        }

        /// <summary>
        /// marks issue alternatives as read/seen
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        public void MarkAsRead(int issueId, int userId)
        {
            InformationReadOp.MarkAlternatives(issueId, userId);
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
