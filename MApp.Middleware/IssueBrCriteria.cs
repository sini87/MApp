﻿using MApp.DA;
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
    /// Middleware class for BrCriteria view
    /// </summary>
    public class IssueBrCriteria
    {
        /// <summary>
        /// returns all criteria of an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performing this operation</param>
        /// <returns></returns>
        public List<CriterionModel> GetIssueCriteria(int issueId, int userId)
        {
            CriterionModel cm = new CriterionModel();
            List<CriterionModel> cmList = cm.ToModelList(CriterionOp.GetIssueCriterions(issueId, userId), cm);

            List<CommentModel> comments = GetComments(issueId, userId);
            foreach(CriterionModel model in cmList)
            {
                model.Comments = comments.Where(x => x.Type == "Criterion" + model.Id).ToList();
            }

            return cmList;
        }

        /// <summary>
        /// updates criteria of an issue
        /// </summary>
        /// <param name="updatedCriteria">new and old criteria who is updated</param>
        /// <param name="deletedCriteria">deleted criteria</param>
        /// <param name="userId">user who is performing this operation</param>
        public void UpdateCriteria(List<CriterionModel> updatedCriteria, List<int> deletedCriteria, int userId)
        {
            CriterionModel cm = new CriterionModel();
            List<Criterion> updateList = cm.ToEntityList(updatedCriteria.Where(x => x.Id > 0).ToList());
            List<Criterion> addedList = cm.ToEntityList(updatedCriteria.Where(x => x.Id == -1).ToList());
            CriterionOp.DeleteCriterions(deletedCriteria, userId);
            CriterionOp.UpdateCriterions(updateList, userId);
            CriterionOp.AddCriterions(addedList,userId);
        }

        private List<CommentModel> GetComments(int issueId, int userId)
        {
            CommentModel cm = new CommentModel();
            List<CommentModel> list = cm.ToModelList(CommentOp.GetCriterionComments(issueId, userId), cm);
            List<KeyValuePair<int, string>> userList = UserOp.GetUserNames(list.Select(x => x.UserId).Distinct().ToList());

            foreach (CommentModel model in list)
            {
                model.Name = userList.Where(x => x.Key == model.UserId).FirstOrDefault().Value;
            }

            return list;
        }
    }
}
