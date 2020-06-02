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
    public class CategoriesController : Controller
    {
        private ICategoryRepository _categoryRepository;
        private IBookRepository _bookRepository;

        public CategoriesController(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
        }
        [HttpGet("{categoryId}", Name = "GetCategory")] //Rediredt will find it based on this attribute
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type =typeof(CategoryDto))]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _categoryRepository.GetCategory(categoryId);
            var category = new CategoryDto()
            {
                Id = result.Id,
                Name = result.Name
            };
            return Ok(category);
        }

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategories()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _categoryRepository.GetCategories();
            var categoriesDto = new List<CategoryDto>();
            foreach(var c in result)
            {
                categoriesDto.Add(new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                });
            }
            return Ok(categoriesDto);
        }

        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategoriesofABook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _categoryRepository.GetCategoriesofABook(bookId);
            var categoriesDto = new List<CategoryDto>();
            foreach (var c in result)
            {
                categoriesDto.Add(new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                });
            }
            return Ok(categoriesDto);
        }

        [HttpGet("{categoryId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooksForCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _categoryRepository.GetBooksForCategory(categoryId);
            var bookDto = new List<BookDto>();
            foreach (var c in result)
            {
                bookDto.Add(new BookDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    BookPublished = c.BookPublished,
                    Isbn = c.Isbn
                });
            }
            return Ok(bookDto);
        }

        //api/categories        
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategory([FromBody] Category cateogryToCreate)
        {
            if (cateogryToCreate == null)
            {
                return BadRequest(ModelState);
            }
            var category = _categoryRepository.GetCategories().Where(b => b.Name.Trim().ToUpper() == cateogryToCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", $"Category {cateogryToCreate.Name} already exists");
                return StatusCode(422, ModelState); // status code 422 - the object is unprocessable
                                                    // return StatusCode(422, $"Country {countryToCreate.Name} already exists");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CreateCategory(cateogryToCreate))
            {
                ModelState.AddModelError("", $"Sonething went wrong saving {cateogryToCreate.Name}");
                return StatusCode(500, ModelState); //Server error
            }
            //After all went Ok, we want to route back to the source i.e. we will display the obejct which we wanted to save.
            return CreatedAtRoute("GetCategory", new { categoryId = cateogryToCreate.Id }, cateogryToCreate);
        }

        //api/categories/{categoryId}
        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)] //No content
        [ProducesResponseType(400)] //Bad request
        [ProducesResponseType(404)] //Not found
        [ProducesResponseType(500)] //Server error
        [ProducesResponseType(422)] //Unprocessable entity
        //the categoryId will be matched with the hidden Id in updatedCategoryInfo, if its same we are good to go, else it wont be processed
        public IActionResult UpdateCategory(int categoryId, [FromBody] Category updatedCategoryInfo)
        {
            if (updatedCategoryInfo == null || categoryId != updatedCategoryInfo.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
            if(_categoryRepository.IsDuplicateCategoryName(categoryId, updatedCategoryInfo.Name))
            {
                ModelState.AddModelError("", $"Category {updatedCategoryInfo.Name} already exists");
                return StatusCode(422, ModelState);
            }
            if (!_categoryRepository.UpdateCategory(updatedCategoryInfo))
            {
                ModelState.AddModelError("", $"Sonething went wrong updating {updatedCategoryInfo.Name}");
                return StatusCode(500, ModelState); //Server error
            }
            return NoContent();
        }

        //api/categories/{categoryId}
        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)] //No content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] //Not found
        [ProducesResponseType(500)]
        [ProducesResponseType(409)]
        public IActionResult DeleteCategory(int categoryId)
        {
            //if the category is used by atleast one book, DB will restict the deletion, but we want to display an error to the user
            var categorytoDelete = _categoryRepository.GetCategory(categoryId);
            if (_categoryRepository.GetCategoriesofABook(categoryId).Count > 0)
            {
                ModelState.AddModelError("", $"Country {categorytoDelete.Name} cannot be deleted because it is used by atleast one book");
                return StatusCode(409, ModelState); //Conflict
            }
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.DeleteCategory(categorytoDelete))
            {
                ModelState.AddModelError("", $"Sonething went wrong deleting {categorytoDelete.Name}");
                return StatusCode(500, ModelState); //Server error
            }
            return NoContent();
        }
    }
}
