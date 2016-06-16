using MApp.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MApp.Middleware.Models
{
    public class ReviewModel : ListModel<Review, ReviewModel>, IListModel<Review, ReviewModel>
    {
        public int IssueId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Explanation { get; set; }

        public Review ToEntity(ReviewModel model)
        {
            Review entity = new Review();
            entity.UserId = model.UserId;
            entity.IssueId = model.IssueId;
            entity.Rating = model.Rating;
            entity.Explanation = model.Explanation;
            return entity;
        }

        public ReviewModel ToModel(Review entity)
        {
            ReviewModel model = new ReviewModel();
            model.UserId = entity.UserId;
            model.IssueId = entity.IssueId;
            model.Rating = entity.Rating;
            model.Explanation = entity.Explanation;
            return model;
        }
    }
}
