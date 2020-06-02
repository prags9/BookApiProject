using BookApi.Models;
using BookApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
    public class ReviewRepository : IReviewRepository
    {
        private BookDbContext _bookDbContext;
        public ReviewRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }

        public bool CreateReview(Review review)
        {
            _bookDbContext.Add(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _bookDbContext.Remove(review);
            return Save();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            _bookDbContext.RemoveRange(reviews);
            return Save();
        }

        public Book GetBookOfaReview(int reviewId)
        {
            return _bookDbContext.Reviews.Where(r => r.Id == reviewId).Select(c => c.Book).FirstOrDefault();
        }

        public Review GetReview(int reviewId)
        {
            return _bookDbContext.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return _bookDbContext.Reviews.OrderBy(r => r.Rating).ToList();
        }

        public ICollection<Review> GetReviewsOfaBook(int bookId)
        {
           // return _bookDbContext.Reviews.Where(b => b.Book.Id == bookId).ToList();
            return _bookDbContext.Books.Where(b => b.Id == bookId).SelectMany(b => b.Reviews).ToList();
        }

        public bool ReviewExists(int reviewId)
        {
            return _bookDbContext.Reviews.Any(r => r.Id == reviewId);
        }

        public bool Save()
        {
            var saved = _bookDbContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            _bookDbContext.Update(review);
            return Save();
        }
    }
}
