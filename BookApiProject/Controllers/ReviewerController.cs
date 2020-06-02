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
    public class ReviewerController : Controller
    {        
        private IReviewerRepository _reviewerRepository;
        private IReviewRepository _reviewRepository;

        public ReviewerController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetReviewers()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reviewers = _reviewerRepository.GetReviewers();
            var reviewersDto = new List<ReviewerDto>();
            foreach(var r in reviewers)
            {
                reviewersDto.Add(new ReviewerDto()
                {
                    Id = r.Id,
                    FirstName = r.FirstName,
                    LastName = r.LastName
                });
            }
            return Ok(reviewersDto);
        }

        [HttpGet("{reviewerId}", Name = "GetReviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reviewer = _reviewerRepository.GetReviewer(reviewerId);
            var reviewerDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };
               
            return Ok(reviewerDto);
        }

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId);
            var reviewDto = new List<ReviewDto>();
            foreach(var r in reviews)
            {
                reviewDto.Add(new ReviewDto
                {
                    Id = r.Id,
                    Headline = r.Headline,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText
                });
            }
            return Ok(reviewDto);
        }

        [HttpGet("{reviewId}/reviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewerOfaReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reviewer = _reviewerRepository.GetReviewerOfaReview(reviewId);
            var reviewerDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };
            
            return Ok(reviewerDto);
        }

        //api/reviewer        
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateReviewer([FromBody] Reviewer reviewerToCreate)
        {
            if (reviewerToCreate == null)
            {
                return BadRequest(ModelState);
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.CreateReviewer(reviewerToCreate    ))
            {
                ModelState.AddModelError("", $"Sonething went wrong saving " + $"{ reviewerToCreate.FirstName} {reviewerToCreate.LastName}");
                return StatusCode(500, ModelState); //Server error
            }
            //After all went Ok, we want to route back to the source i.e. we will display the obejct which we wanted to save.
            return CreatedAtRoute("GetReviewer", new { reviewerId = reviewerToCreate.Id }, reviewerToCreate);
        }

        //api/reviewer/{reviewerId}
        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)] //No content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] //Not found
        [ProducesResponseType(500)]
        //the reviewerid will be matched with the hidden Id in updatedreviewerInfo, if its same we are good to go, else it wont be processed
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] Reviewer updatedreviewerInfo)
        {
            if (updatedreviewerInfo == null || reviewerId != updatedreviewerInfo.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
           
            if (!_reviewerRepository.UpdateReviewer(updatedreviewerInfo))
            {
                ModelState.AddModelError("", $"Sonething went wrong updating"+ "{updatedreviewerInfo.FirstName} {updatedreviewerInfo.LastName}");
                return StatusCode(500, ModelState); //Server error
            }
            return NoContent();
        }

        //api/reviewer/{reviewerId}
        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)] //No content
        [ProducesResponseType(400)] //Bad REquest
        [ProducesResponseType(404)] //Not found
        [ProducesResponseType(500)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
                        
            var reviewertoDelete = _reviewerRepository.GetReviewer(reviewerId);
            var reviewsToDelete = _reviewerRepository.GetReviewsByReviewer(reviewerId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewerRepository.DeleteReviewer(reviewertoDelete))
            {
                ModelState.AddModelError("", $"Sonething went wrong deleting" + "{updatedreviewerInfo.FirstName} {updatedreviewerInfo.LastName}");
                return StatusCode(500, ModelState); //Server error
            }
            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", $"Sonething went wrong deleting reviews by " + "{updatedreviewerInfo.FirstName} {updatedreviewerInfo.LastName}");
                return StatusCode(500, ModelState); //Server error
            }
            return NoContent();
        }
    }
}
