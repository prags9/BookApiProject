using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        //api/books/{bookId}
        [HttpGet("{bookId}")]
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
    }
}
