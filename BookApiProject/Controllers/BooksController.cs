using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApi.Models;
using BookApiProject.DTOs;
using BookApiProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private IBookRepository _bookRepository;
        private IAuthorRepository _authorRepository;
        private ICategoryRepository _categoryRepository;
        private IReviewRepository _reviewReposiory;

        public BooksController(IBookRepository bookRepository, IAuthorRepository authorRepository, ICategoryRepository categoryRepository, IReviewRepository reviewRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _reviewReposiory = reviewRepository;
        }

        //api/books/{bookId}
        [HttpGet("{bookId}", Name ="GetBook")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var book = _bookRepository.GetBook(bookId);
            var bookDto = new BookDto()
            {
                Id = book.Id,
                BookPublished = book.BookPublished,
                Isbn = book.Isbn,
                Title = book.Title
            };
            return Ok(bookDto);
        }

        //api/books/ISBN/{isbn}
        [HttpGet("ISBN/{isbn}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBook(string isbn)
        {
             if (!_bookRepository.BookExists(isbn))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var book = _bookRepository.GetBook(isbn);
            var bookDto = new BookDto()
            {
                Id = book.Id,
                BookPublished = book.BookPublished,
                Isbn = book.Isbn,
                Title = book.Title
            };
            return Ok(bookDto);
        }

        //api/books
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooks()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var books = _bookRepository.GetBooks();
            var booksDto = new List<BookDto>();
            foreach (var a in books)
            {
                booksDto.Add(new BookDto()
                {
                    Id = a.Id,
                    BookPublished = a.BookPublished,
                    Isbn=a.Isbn,
                    Title=a.Title
                });
            }
            return Ok(booksDto);
        }

        //api/books/{bookId}/rating
        [HttpGet("{bookId}/rating")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(decimal))]
        public IActionResult GetBookRating(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var bookRating = _bookRepository.GetBookRating(bookId);
            return Ok(bookRating);
        }

        private StatusCodeResult ValidateBook(List<int> authId, List<int> catId, Book book)
        {
            if (book == null || authId.Count <= 0 || catId.Count <= 0)
            {
                ModelState.AddModelError("", "Missing book, category or author");
                return BadRequest();
            }


            if (_bookRepository.IsDuplicateIsbn(book.Id, book.Isbn))
            {
                ModelState.AddModelError("", "Duplicate ISBN");
                return StatusCode(422);
            }

            foreach(var id in authId)
            {
                if (!_authorRepository.AuthorExists(id))
                {
                    ModelState.AddModelError("", "AUthor not found");
                    return StatusCode(404);
                }
            }

            foreach (var id in catId)
            {
                if (!_categoryRepository.CategoryExists(id))
                {
                    ModelState.AddModelError("", "Category not found");
                    return StatusCode(404);
                }
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Critical error");
                return BadRequest();
            }

            return NoContent();
        }

        //api/books?authId=1&authId=2&catId=1&catId=2
        // we will get the params authId and catId from the query string
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Book))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateBook([FromQuery] List<int> authId, [FromQuery] List<int> catId, [FromBody]Book bookToCreate)
        {
            var statusCode = ValidateBook(authId, catId, bookToCreate);
            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode, ModelState);

            if (!_bookRepository.CreateBook(authId, catId, bookToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong in saving the book {bookToCreate.Title}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetBook", new { bookId = bookToCreate.Id }, bookToCreate);
        }

        //api/books/bookId?authId=1&authId=2&catId=1&catId=2
        // we will get the params authId and catId from the query string
        [HttpPut("{bookId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateBook(int bookId, [FromQuery] List<int> authId, [FromQuery] List<int> catId, [FromBody]Book bookToUpdate)
        {
            var statusCode = ValidateBook(authId, catId, bookToUpdate);
            if(bookId != bookToUpdate.Id)
            {
                return BadRequest();
            }
            if (!_bookRepository.BookExists(bookId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode, ModelState);

            if (!_bookRepository.UpdateBook(authId, catId, bookToUpdate))
            {
                ModelState.AddModelError("", $"Something went wrong in saving the book {bookToUpdate.Title}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        //api/books/{bookId}
        [HttpDelete("{bookId}")]
        [ProducesResponseType(204)] //No content
        [ProducesResponseType(400)] //Bad REquest
        [ProducesResponseType(404)] //Not found
        [ProducesResponseType(500)]
        public IActionResult DeleteBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
            {
                return NotFound();
            }
                        
            var reviewsToDelete = _reviewReposiory.GetReviewsOfaBook(bookId);
            var bookToDelete = _bookRepository.GetBook(bookId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewReposiory.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", $"Sonething went wrong deleting reviews");
                return StatusCode(500, ModelState); //Server error
            }
            if (!_bookRepository.DeleteBook(bookToDelete))
            {
                ModelState.AddModelError("", $"Sonething went wrong deleting book {bookToDelete.Title}");
                return StatusCode(500, ModelState); //Server error
            }

            return NoContent();
        }
    }
}
