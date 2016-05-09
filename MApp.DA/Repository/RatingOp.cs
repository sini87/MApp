using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class RatingOp : Operations
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

            var query = from r in Ctx.Rating
                        where
                          r.UserId == userId &&
                            (from Criterion in Ctx.Criterion
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
                foreach (var entity in query)
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
                var query2 = from c in Ctx.Criterion
                             from a in Ctx.Alternative
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
                foreach (var entity in query2)
                {
                    rat = new Rating();
                    rat.AlternativeId = entity.AlternativeId;
                    rat.CriterionId = entity.CriterionId;
                    rat.UserId = convertedUserId;
                    rat.Value = 0.0;
                    list.Add(rat);
                }
            }
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

            var query = from rat in Ctx.Rating
                        where
                          rat.UserId != userId &&
                          rat.Alternative.IssueId == issueId
                        orderby
                          rat.UserId,
                          rat.Criterion.Id,
                          rat.Alternative.Id
                        select rat;
            list = query.ToList();

            return list;
        }

        /// <summary>
        /// saves ratings to an issue
        /// </summary>
        /// <param name="userRatings"></param>
        public static void SaveUserRatings(List<Rating> userRatings)
        {
            if (userRatings.Count > 0)
            {
                List<Rating> list;
                bool insert;
                int issueId;
                Rating hRat;

                issueId = Ctx.Criterion.Find(userRatings[0].CriterionId).Issue;

                int userId = userRatings[0].UserId;
                var query = from Rating in Ctx.Rating
                            where
                                Rating.UserId == userId &&
                                (from Criterion in Ctx.Criterion
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
                        Ctx.Rating.Add(rat);
                        Ctx.Entry(rat).State = EntityState.Added;
                    }else
                    {
                        hRat = Ctx.Rating.Find(rat.CriterionId, rat.AlternativeId, rat.UserId);
                        hRat.Value = rat.Value;
                        Ctx.Entry(hRat).State = EntityState.Modified;
                    }
                    Ctx.SaveChanges();
                }
            }
        }

        public static List<int> GetAlreadyRatedUsers(int issueId, int userId)
        {
            List<int> users;

            string query = "select distinct(rat.UserId) from appSchema.Rating rat, " +
                "appSchema.Alternative alt where alt.Id = rat.AlternativeId and alt.IssueId = {0} AND " +
                "rat.UserId !=  {1}";

            users = Ctx.Database.SqlQuery<int>(query, issueId, userId).ToList() ;

            return users;
        }
    }
}
