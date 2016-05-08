using System;
using System.Collections.Generic;
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
                var query2 = from a in Ctx.Alternative
                        from c in Ctx.Criterion
                        where
                          a.IssueId == 1
                        select new
                        {
                            AlternativeId = a.Id,
                            CriterionId = c.Id
                        };
                int convertedUserId = Convert.ToInt32(userId);
                foreach (var entity in query2)
                {
                    rat = new Rating();
                    rat.AlternativeId = entity.AlternativeId;
                    rat.CriterionId = entity.CriterionId;
                    rat.UserId = convertedUserId;
                    rat.Value = 2.0;
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

            var query = from Rating in Ctx.Rating
                    where
                      Rating.UserId != userId &&
                        (from Criterion in Ctx.Criterion
                         where
                            Criterion.Issue == issueId
                         select new
                         {
                             Criterion.Id
                         }).Contains(new { Id = Rating.CriterionId })
                    select Rating;
            list = query.ToList();

            return list;
        }
    }
}
