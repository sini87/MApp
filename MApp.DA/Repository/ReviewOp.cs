using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA.Repository
{
    /// <summary>
    /// makes all operations on table review
    /// </summary>
    public class ReviewOp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns>review rating for an issue</returns>
        public static double GetReviewRating(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            double rating = 0.0;
            
            if (ctx.Review.Where(x => x.IssueId == issueId).Count() > 0)
            {
                rating = ctx.Review.Where(x => x.IssueId == issueId).Average(x => x.Rating);
            }

            ctx.Dispose();
            return rating;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">issue id</param>
        /// <returns>list of user reviews for issue</returns>
        public static List<Review> GetIssueReviews(int issueId)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            List<Review> list;

            list = ctx.Review.Where(x => x.IssueId == issueId).ToList();

            ctx.Dispose();
            return list;
        }

        /// <summary>
        /// saves user review for issue
        /// </summary>
        /// <param name="review"></param>
        public static void SaveIssueReview(Review review)
        {
            ApplicationDBEntities ctx = new ApplicationDBEntities();
            HReview hreview = ctx.HReview.Create();
            hreview.ChangeDate = DateTime.Now;
            hreview.IssueId = review.IssueId;
            hreview.UserId = review.UserId;

            if (ctx.Review.Where(x => x.IssueId == review.IssueId && x.UserId == review.UserId).Count() > 0)
            {
                hreview.Action = "Review updated";
                Review dbReview = ctx.Review.Where(x => x.IssueId == review.IssueId && x.UserId == review.UserId).FirstOrDefault();
                dbReview.Rating = review.Rating;
                dbReview.Explanation = review.Explanation;
                ctx.Entry(dbReview).State = System.Data.Entity.EntityState.Modified;
            }else
            {
                hreview.Action = "Review added";
                ctx.Review.Add(review);
                ctx.Entry(review).State = System.Data.Entity.EntityState.Added;
            }

            ctx.HReview.Add(hreview);
            ctx.Entry(hreview).State = System.Data.Entity.EntityState.Added;
            ctx.SaveChanges();

            ctx.Dispose();
        }
    }
}
