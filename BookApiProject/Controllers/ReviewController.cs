using BookApi.Models;
using BookApiProject.DTOs;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private IReviewRepository _reviewRepository;
        private IReviewerRepository _reviewerRepository;
        private IBookRepository _bookRepository;

        public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IBookRepository bookRepository)
        {
            _reviewRepository = reviewRepository;
            _reviewerRepository = reviewerRepository;
            _bookRepository = bookRepository;
        }
                
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviews()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reviews = _reviewRepository.GetReviews();
            var reviewDto = new List<ReviewDto>();
            foreach (var r in reviews)
            {
                reviewDto.Add(new ReviewDto()
                {
                    Id = r.Id,
                    Headline = r.Headline,
                    Rating=r.Rating,
                    ReviewText=r.ReviewText
                });
            }
            return Ok(reviewDto);
        }

        [HttpGet("{reviewId}", Name = "GetReview")] //Rediredt will find it based on this attribute
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var review = _reviewRepository.GetReview(reviewId);
            var reviewDto = new ReviewDto()
            {
                Id = review.Id,
                Headline = review.Headline,
                Rating = review.Rating,
                ReviewText = review.ReviewText
            };

            return Ok(reviewDto);
        }

        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsOfaBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var reviews= _reviewRepository.GetReviewsOfaBook(bookId);
            var reviewDto = new List<ReviewDto>();
            foreach (var r in reviews)
            {
                reviewDto.Add(new ReviewDto() { 
                    Id=r.Id,
                    Headline=r.Headline,
                    Rating=r.Rating,
                    ReviewText=r.ReviewText
                });
            }
            return Ok(reviewDto);
        }

        [HttpGet("{reviewId}/book")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetBookOfaReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var book = _reviewRepository.GetBookOfaReview(reviewId);
            var bookDto = new BookDto()
            {
                Id = book.Id,
                BookPublished = book.BookPublished,
                Isbn = book.Isbn,
                Title = book.Title
            };
            return Ok(bookDto);
        }

        //api/review        
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateReview([FromBody] Review reviewToCreate)
        {
            if (reviewToCreate == null)
            {
                return BadRequest(ModelState);
            }
                      
            if (!_reviewerRepository.ReviewerExists(reviewToCreate.Reviewer.Id))
            {
                ModelState.AddModelError("","Reviewer doesn't exist");
            }
            if (!_bookRepository.BookExists(reviewToCreate.Book.Id))
            {
                ModelState.AddModelError("","Book doesn't exist");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(404, ModelState);
            }

            reviewToCreate.Book = _bookRepository.GetBook(reviewToCreate.Book.Id);
            reviewToCreate.Reviewer = _reviewerRepository.GetReviewer(reviewToCreate.Reviewer.Id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewRepository.CreateReview(reviewToCreate))
            {
                ModelState.AddModelError("", $"Sonething went wrong saving the review");
                return StatusCode(500, ModelState); //Server error
            }
            //After all went Ok, we want to route back to the source i.e. we will display the obejct which we wanted to save.
            return CreatedAtRoute("GetReview", new { reviewId = reviewToCreate.Id }, reviewToCreate);
        }

        //api/review/{reviewId}
        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)] //No content
        [ProducesResponseType(400)] //Bad request
        [ProducesResponseType(404)] //Not found
        [ProducesResponseType(500)] //Server error
        //the categoryId will be matched with the hidden Id in updatedCategoryInfo, if its same we are good to go, else it wont be processed
        public IActionResult UpdateReview(int reviewId, [FromBody] Review updatedReviewInfo)
        {
            if (updatedReviewInfo == null || reviewId != updatedReviewInfo.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                ModelState.AddModelError("", "Review doesn't exist");
            }
            if (!_reviewerRepository.ReviewerExists(updatedReviewInfo.Reviewer.Id))
            {
                ModelState.AddModelError("", "Reviewer doesn't exist");
            }
            if (!_bookRepository.BookExists(updatedReviewInfo.Book.Id))
            {
                ModelState.AddModelError("", "Book doesn't exist");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(404, ModelState);
            }
            updatedReviewInfo.Book = _bookRepository.GetBook(updatedReviewInfo.Book.Id);
            updatedReviewInfo.Reviewer = _reviewerRepository.GetReviewer(updatedReviewInfo.Reviewer.Id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewRepository.UpdateReview(updatedReviewInfo))
            {
                ModelState.AddModelError("", $"Sonething went wrong saving the review");
                return StatusCode(500, ModelState); //Server error
            }
            //After all went Ok, just return no content.
            return NoContent();           
        }

        //api/review/{reviewId}
        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)] //No content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] //Not found
        [ProducesResponseType(500)]
        [ProducesResponseType(409)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var reviewToDelete = _reviewRepository.GetReview(reviewId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewRepository.DeleteReview(reviewToDelete))
            {
                ModelState.AddModelError("", $"Sonething went wrong deleting");
                return StatusCode(500, ModelState); //Server error
            }
            return NoContent();
        }
    }
}
