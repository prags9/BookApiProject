using BookApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
    public interface IReviewRepository
    {
        Review GetReview(int reviewId);
        ICollection<Review> GetReviews();
        ICollection<Review> GetReviewsOfaBook(int bookId);
        Book GetBookOfaReview(int reviewId);
        Boolean ReviewExists(int reviewId);
        Boolean CreateReview(Review review);
        Boolean UpdateReview(Review review);
        Boolean DeleteReview(Review review);
        Boolean DeleteReviews(List<Review> reviews);
        Boolean Save();
    }
}
