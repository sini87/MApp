using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// makes operations to table Rating
    /// </summary>
    public class RatingOp
    {
        /// <summary>
        /// gets user ratings of an issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Rating> GetUserRatings(int issueId, int userId)
        {
            Rating rat;
            List<Rating> list = new List<Rating>();
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            var query = from r in ctx.Rating
                        where
                          r.UserId == userId &&
                            (from Criterion in ctx.Criterion
                             where Criterion.Issue == issueId
                             select new
                             {
                                 Criterion.Id
                             }).Contains(new { Id = r.CriterionId })
                        orderby
                          r.CriterionId,
                          r.AlternativeId
                        select new
                        {
                            r.CriterionId,
                            r.AlternativeId,
                            r.UserId,
                            r.Value
                        };
            
            
            //check if user hast already done rating
            if (query.Count() > 0)
            {
                foreach (var entity in query.AsNoTracking())
                {
                    rat = new Rating();
                    rat.AlternativeId = entity.AlternativeId;
                    rat.CriterionId = entity.CriterionId;
                    rat.UserId = entity.UserId;
                    rat.Value = entity.Value;
                    list.Add(rat);
                }
            }else if (query.Count() == 0 )
            {//get dummy ratings for user
                var query2 = from c in ctx.Criterion
                             from a in ctx.Alternative
                             where
                               c.Issue == issueId &&
                               a.IssueId == issueId
                             orderby
                               c.Id, a.Id
                             select new
                             {
                                 CriterionId = c.Id,
                                 AlternativeId = a.Id
                             };
                int convertedUserId = Convert.ToInt32(userId);
                foreach (var entity in query2.AsNoTracking())
                {
                    rat = new Rating();
                    rat.AlternativeId = entity.AlternativeId;
                    rat.CriterionId = entity.CriterionId;
                    rat.UserId = convertedUserId;
                    rat.Value = 0.0;
                    list.Add(rat);
                }
            }

            ctx.Dispose();

            return list;
        }

        /// <summary>
        /// returns list of all ratings expect own rating
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Rating> GetAllIssueRatings(int issueId, int userId)
        {
            List<Rating> list = new List<Rating>();
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            var query = from rat in ctx.Rating
                        where
                          rat.UserId != userId &&
                          rat.Alternative.IssueId == issueId
                        orderby
                          rat.UserId,
                          rat.Criterion.Id,
                          rat.Alternative.Id
                        select rat;
            list = query.AsNoTracking().ToList();
            ctx.Dispose();

            return list;
        }

        /// <summary>
        /// saves ratings to an issue
        /// </summary>
        /// <param name="userRatings"></param>
        public static void SaveUserRatings(List<Rating> userRatings)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            if (userRatings.Count > 0)
            {
                List<Rating> list;
                bool insert;
                int issueId;
                Rating hRat;

                issueId = ctx.Criterion.Find(userRatings[0].CriterionId).Issue;

                int userId = userRatings[0].UserId;
                var query = from Rating in ctx.Rating
                            where
                                Rating.UserId == userId &&
                                (from Criterion in ctx.Criterion
                                 where
                             Criterion.Issue == issueId
                                 select new
                                 {
                                     Criterion.Id
                                 }).Contains(new { Id = Rating.CriterionId })
                            select Rating;
                list = query.ToList();

                if (list.Count == 0)
                {
                    insert = true;
                }else
                {
                    insert = false;
                }

                foreach (Rating rat in userRatings)
                {
                    if (insert)
                    {
                        ctx.Rating.Add(rat);
                        ctx.Entry(rat).State = EntityState.Added;
                    }else
                    {
                        hRat = ctx.Rating.Find(rat.CriterionId, rat.AlternativeId, rat.UserId);
                        hRat.Value = rat.Value;
                        ctx.Entry(hRat).State = EntityState.Modified;
                    }
                    ctx.SaveChanges();
                }
            }

            ctx.Dispose();
        }

        /// <summary>
        /// returns all users who have rated/evaluated some issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId">user who is performint this operation</param>
        /// <returns></returns>
        public static List<int> GetAlreadyRatedUsers(int issueId, int userId)
        {
            List<int> users;
            ApplicationDBEntities ctx = new ApplicationDBEntities();

            string query = "select distinct(rat.UserId) from appSchema.Rating rat, " +
                "appSchema.Alternative alt where alt.Id = rat.AlternativeId and alt.IssueId = {0} AND " +
                "rat.UserId !=  {1}";

            users = ctx.Database.SqlQuery<int>(query, issueId, userId).ToList() ;
            ctx.Dispose();

            return users;
        }

        /// <summary>
        /// returns true if user has to evaluate
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool GetRatingActionRequired(int issueId, int userId)
        {
            bool ret;
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            AccessRight ar = ctx.AccessRight.Find(userId, issueId);
            if (ctx.Issue.Find(issueId).Status == "EVALUATING" && ar.Right != "V")
            {
                List<Alternative> aList = ctx.Alternative.Where(x => x.IssueId == issueId).ToList();
                if (aList.Count != 0)
                {
                    int id = aList.FirstOrDefault().Id;
                    List<Rating> rList = ctx.Rating.Where(x => x.AlternativeId == id).ToList();
                    if (rList == null || rList.Count == 0)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }else
                {
                    ret = false;
                }
            }
            else
            {
                ret = false;
            }

            ctx.Dispose();
            return ret;
        }
    }
}
