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
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ICountryRepository _countryRepository;

        public AuthorController(IAuthorRepository authorRepository , IBookRepository bookRepository, ICountryRepository countryRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _countryRepository = countryRepository;
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
        [HttpGet("{authorId}", Name = "GetAuthor")] //Redirects here from CreateAuthor because of Name action
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

        //api/author        
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Author))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateAuthor([FromBody] Author authorToCreate)
        {
            if (authorToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.CountryExists(authorToCreate.Country.Id))
            {
                ModelState.AddModelError("", "Country doesn't exist");
                return StatusCode(404, ModelState);
            }

            authorToCreate.Country = _countryRepository.GetCountry(authorToCreate.Country.Id);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_authorRepository.CreateAuthor(authorToCreate))
            {
                ModelState.AddModelError("", $"Sonething went wrong saving the author " + $"{authorToCreate.FirstName} {authorToCreate.LastName}");
                return StatusCode(500, ModelState); //Server error
            }
            //After all went Ok, we want to route back to the source i.e. we will display the obejct which we wanted to save.
            return CreatedAtRoute("GetAuthor", new { authorId = authorToCreate.Id }, authorToCreate);
        }

        //api/author/{authorId}        
        [HttpPut("{authorId}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateAuthor(int authorId,[FromBody] Author authorToUpdate)
        {
            if (authorToUpdate == null)
            {
                return BadRequest(ModelState);
            }
            if(authorId != authorToUpdate.Id)
            {
                return BadRequest(ModelState);
            }
            if(!_authorRepository.AuthorExists(authorId))
            {
                ModelState.AddModelError("", "Author doesn't exist");
            }
            if (!_countryRepository.CountryExists(authorToUpdate.Country.Id))
            {
                ModelState.AddModelError("", "Country doesn't exist");               
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(404, ModelState);
            }
            authorToUpdate.Country = _countryRepository.GetCountry(authorToUpdate.Country.Id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_authorRepository.UpdateAuthor(authorToUpdate))
            {
                ModelState.AddModelError("", $"Sonething went wrong updating the author " + $"{authorToUpdate.FirstName} {authorToUpdate.LastName}");
                return StatusCode(500, ModelState); //Server error
            }
            //After all went Ok, we want to route back to No content.
            return NoContent();
        }

        //api/author/{authorId}
        [HttpDelete("{authorId}")]
        [ProducesResponseType(204)] //No content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] //Not found
        [ProducesResponseType(500)]
        [ProducesResponseType(409)]
        public IActionResult DeleteAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            //if the author is used by atleast one book, DB will restict the deletion, but we want to display an error to the user
            var authorToDelete = _authorRepository.GetAuthor(authorId);
            if (_authorRepository.GetBooksByAuthor(authorId).Count > 0)
            {
                ModelState.AddModelError("", $"Author {authorToDelete.FirstName} {authorToDelete.LastName} cannot be deleted because it is used by atleast one book");
                return StatusCode(409, ModelState); //Conflict
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_authorRepository.DeleteAuthor(authorToDelete))
            {
                ModelState.AddModelError("", $"Sonething went wrong deleting {authorToDelete.FirstName} {authorToDelete.LastName}");
                return StatusCode(500, ModelState); //Server error
            }
            return NoContent();
        }
    }
}
