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
    }
}
