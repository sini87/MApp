using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// class where all opeartions to comment table are made
    /// </summary>
    public class CommentOp
    {
        /// <summary>
        /// adds a new comment
        /// </summary>
        /// <param name="comment"></param>
        public static void AddComment(Comment comment)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            comment.DateTime = System.DateTime.Now;
            ctx.Comment.Add(comment);
            ctx.Entry(comment).State = EntityState.Added;
            ctx.SaveChanges();

            ctx.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is performing operation</param>
        /// <returns>list of alternatives comments</returns>
        public static List<Comment> GetAlternativeComments(int issueId, int userId)
        {
            string sql;
            List<Comment> list;
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            using (var dbContextTransaction = ctx.Database.BeginTransaction())
            {
                sql = "Select * from Comment " + 
                    "Where Type in (" + 
                    "Select CONCAT('Alternative', Id) From Alternative Where IssueId = {0})";
                list = ctx.Database.SqlQuery<Comment>(sql,  issueId).ToList();
            }

            ctx.Dispose();

            return list;
        }

        /// <summary>
        /// deletes a list of alternative comments
        /// </summary>
        /// <param name="alternativeIds">list of alternative ids</param>
        public static void DeleteAlternativeComments(List<int> alternativeIds)
        {
            string alt = "";
            bool first = true;
            string sql;

            if (alternativeIds == null || alternativeIds.Count == 0)
            {
                return;
            }

            foreach(int id in alternativeIds)
            {
                if (first)
                {
                    alt = "('Alternative" + id + "'";
                    first = false;
                }
                else
                {
                    alt = alt + ",'Alternative" + id + "'";
                }
            }
            alt = alt + ")";

            ApplicationDBEntities ctx = new ApplicationDBEntities();
            using (var dbContextTransaction = ctx.Database.BeginTransaction())
            {
                sql = "DELETE FROM  appSchema.Comment WHERE Type in " + alt;
                ctx.Database.ExecuteSqlCommand(sql);
                dbContextTransaction.Commit();
            }

            ctx.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is performing operation</param>
        /// <returns>list of criteria comments</returns>
        public static List<Comment> GetCriterionComments(int issueId, int userId)
        {
            string sql;
            List<Comment> list;
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            using (var dbContextTransaction = ctx.Database.BeginTransaction())
            {
                sql = "Select * from Comment " +
                    "Where Type in (" +
                    "Select CONCAT('Criterion', Id) From Criterion Where Issue = {0})";
                list = ctx.Database.SqlQuery<Comment>(sql, issueId).ToList();
            }

            ctx.Dispose();

            return list;
        }

        /// <summary>
        /// deletes criteria comments
        /// </summary>
        /// <param name="criterionIds">list of criterion ids</param>
        public static void DeleteCriterionComments(List<int> criterionIds)
        {
            string criterions = "";
            bool first = true;
            string sql;

            if (criterionIds == null || criterionIds.Count == 0)
            {
                return;
            }

            foreach (int id in criterionIds)
            {
                if (first)
                {
                    criterions = "('Criterion" + id + "'";
                    first = false;
                }
                else
                {
                    criterions = criterions + ",'Criterion" + id + "'";
                }
            }
            criterions = criterions + ")";

            ApplicationDBEntities ctx = new ApplicationDBEntities();
            using (var dbContextTransaction = ctx.Database.BeginTransaction())
            {
                sql = "DELETE FROM  appSchema.Comment WHERE Type in " + criterions;
                ctx.Database.ExecuteSqlCommand(sql);
                dbContextTransaction.Commit();
            }

            ctx.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <param name="userId">user who is performing operation</param>
        /// <param name="type">comment type (eg: Issue)</param>
        /// <returns>list of comments</returns>
        public static List<Comment> GetTypeComments(int issueId, int userId, string type)
        {
            string sql;
            List<Comment> list;
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            using (var dbContextTransaction = ctx.Database.BeginTransaction())
            {
                sql = "Select * from appSchema.Comment " +
                    "Where Type LIKE 'Issue" + issueId + "'";
                list = ctx.Database.SqlQuery<Comment>(sql, issueId).ToList();
            }

            ctx.Dispose();

            return list;
        }
    }
}
