using BookApi.Models;
using BookApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
    public class ReviewerRepository : IReviewerRepository
    {
        private BookDbContext _bookDbContext;

        public ReviewerRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            _bookDbContext.Add(reviewer);
            return Save();
        }

        public bool DeleteReviewer(Reviewer reviewer)
        {
            _bookDbContext.Remove(reviewer);
            return Save();
        }

        public Reviewer GetReviewer(int reviewerId)
        {
            return _bookDbContext.Reviewers.Where(r => r.Id == reviewerId).FirstOrDefault();
        }

        public Reviewer GetReviewerOfaReview(int reviewId)
        {
            /*var reviewerId =  _bookDbContext.Reviews.Where(r => r.Id == reviewId).Select(r => r.Reviewer.Id).FirstOrDefault();
            return _bookDbContext.Reviewers.Where(r => r.Id == reviewerId).FirstOrDefault();*/
            return _bookDbContext.Reviews.Where(r => r.Id == reviewId).Select(r => r.Reviewer).FirstOrDefault();
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return _bookDbContext.Reviewers.OrderBy(c => c.LastName).ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            //return _bookDbContext.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
            return _bookDbContext.Reviewers.Where(r => r.Id == reviewerId).SelectMany(c => c.Reviews).ToList();
        }

        public bool ReviewerExists(int reviewerId)
        {
            return _bookDbContext.Reviewers.Any(c => c.Id == reviewerId);
        }

        public bool Save()
        {
            var saved = _bookDbContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            _bookDbContext.Update(reviewer);
            return Save();
        }
    }
}
