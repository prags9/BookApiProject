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
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;

        public AuthorController(IAuthorRepository authorRepository , IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        //api/authors
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthors()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authors = _authorRepository.GetAuthors();
            var authorDto = new List<AuthorDto>();
            foreach (var a in authors)
            {
                authorDto.Add(new AuthorDto()
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }) ;
            }
            return Ok(authorDto);
        }

        //api/author/{authorId}
        [HttpGet("{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type=typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return BadRequest(ModelState);
            }
            var author = _authorRepository.GetAuthor(authorId);
            var authorDTO = new AuthorDto() { Id = author.Id, FirstName = author.FirstName, LastName = author.LastName };
            return Ok(authorDTO);
        }

        //api/author/books/{bookId}
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorsByBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId)){
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authors = _authorRepository.GetAuthorsByBook(bookId);
            var authorDto = new List<AuthorDto>();
            foreach (var a in authors) {
                authorDto.Add(new AuthorDto()
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName
                });
            }
            return Ok(authorDto);
            
        }

        //api/author/{authorId}/books
        [HttpGet("{authorId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooksByAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var books = _authorRepository.GetBooksByAuthor(authorId);
            var bookDto = new List<BookDto>();
            foreach(var b in books)
            {
                bookDto.Add(new BookDto()
                {
                    Id = b.Id,
                    BookPublished = b.BookPublished,
                    Isbn = b.Isbn,
                    Title = b.Title
                });
            }
            return Ok(bookDto);
        }

      

    }
}
