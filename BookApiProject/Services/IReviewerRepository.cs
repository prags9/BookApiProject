using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Models;

namespace BookApiProject.Services
{
    public interface IReviewerRepository
    {
        ICollection<Reviewer> GetReviewers();
        Reviewer GetReviewer(int reviewerId);
        ICollection<Review> GetReviewsByReviewer(int reviewerId);
        Reviewer GetReviewerOfaReview(int reviewId);
        Boolean ReviewerExists(int reviewerId);
        Boolean CreateReviewer(Reviewer reviewer);
        Boolean UpdateReviewer(Reviewer reviewer);
        Boolean DeleteReviewer(Reviewer reviewer);
        Boolean Save();
    }
}
