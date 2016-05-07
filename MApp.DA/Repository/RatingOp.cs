using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    public class RatingOp : Operations
    {
        public static List<Rating> GetRatings(int issueId, int? userId)
        {
            Rating rat;
            List<Rating> list = new List<Rating>();
            var query = from Rating in Ctx.Rating where Rating.Value > 1000 select Rating;

            //user has already done rating
            if (userId != null)
            {
                query = from Rating in Ctx.Rating
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
            }else//gets all ratings of an issue
            {
                query = from Rating in Ctx.Rating
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
            }
            

            if (query.Count() > 0 && userId != null)
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
            }else if (query.Count() == 0 && userId != null)
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
                    rat.Value = 0.0;
                    list.Add(rat);
                }
            }
            return list;
        }
    }
}
