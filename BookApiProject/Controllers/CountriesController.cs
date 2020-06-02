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
    public class CountriesController : Controller
    {
        private ICountryRepository _countryRepository;
        private IAuthorRepository _authorRepository;

        public CountriesController(ICountryRepository countryRepository, IAuthorRepository authorRepository)
        {
            _countryRepository = countryRepository;
            _authorRepository = authorRepository;
        }

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type =typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = _countryRepository.GetCountries().ToList();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var countriesDto = new List<CountryDto>();
            foreach(var c in countries){
                countriesDto.Add(new CountryDto
                {
                    Id = c.Id,
                    Name = c.Name
                });
            }
            return Ok(countriesDto);
        }

        [HttpGet("{countryId}", Name ="GetCountry")] //Name parameter specifies the action
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId)) return NotFound();

            var country = _countryRepository.GetCountry(countryId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var countryDto = new CountryDto()
            {
                Id = country.Id,Name = country.Name
            };
           
            return Ok(countryDto);
        }

        [HttpGet("authors/{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountryOfAnAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();
            var country = _countryRepository.GetCountryofAnAuthor(authorId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }

        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorsFromCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authors = _countryRepository.GetAuthorsFromCountry(countryId);
            var authorsDto = new List<AuthorDto>();
            foreach(var a in authors)
            {
                authorsDto.Add(new AuthorDto()
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName
                });
            }
            return Ok(authorsDto);
        }

        //api/countries        
        [HttpPost]
        [ProducesResponseType(201, Type= typeof(Country))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCountry([FromBody] Country countryToCreate)
        {
            if(countryToCreate == null)
            {
                return BadRequest(ModelState);
            }
           var country =  _countryRepository.GetCountries().Where(b => b.Name.Trim().ToUpper() == countryToCreate.Name.Trim().ToUpper()).FirstOrDefault();
            
            if(country != null)
            {
                ModelState.AddModelError("", $"Country {countryToCreate.Name} already exists");
                 return StatusCode(422,ModelState); // status code 422 - the object is unprocessable
               // return StatusCode(422, $"Country {countryToCreate.Name} already exists");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.CreateCountry(countryToCreate))
            {
                ModelState.AddModelError("", $"Sonething went wrong saving {countryToCreate.Name}");
                return StatusCode(500, ModelState); //Server error
            }
            //After all went Ok, we want to route back to the source i.e. we will display the obejct which we wanted to save.
            return CreatedAtRoute("GetCountry", new { countryId = countryToCreate.Id }, countryToCreate);
        }

        //api/countries/{countryId}
        [HttpPut("{countryId}")]
        [ProducesResponseType(204)] //No content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] //Not found
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        //the countryId will be matched with the hidden Id in updatedCountryInfo, if its same we are good to go, else it wont be processed
        public IActionResult UpdateCountry(int countryId, [FromBody] Country updatedCountryInfo)
        {
            if(updatedCountryInfo == null || countryId != updatedCountryInfo.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_countryRepository.CountryExists(countryId))
            {
                return NotFound();  
            }
            if(_countryRepository.IsDuplicateCountryName(countryId, updatedCountryInfo.Name))
            {
                ModelState.AddModelError("", $"Country {updatedCountryInfo.Name} already exists");
                return StatusCode(422, ModelState); //Unprocessable entity
            }
            if (!_countryRepository.UpdateCountry(updatedCountryInfo))
            {
                ModelState.AddModelError("", $"Sonething went wrong updating {updatedCountryInfo.Name}");
                return StatusCode(500, ModelState); //Server error
            }
            return NoContent();
        }

        //api/countries/{countryId}
        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)] //No content
        [ProducesResponseType(400)]
        [ProducesResponseType(404)] //Not found
        [ProducesResponseType(500)]
        [ProducesResponseType(409)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
            {
                return NotFound();
            }

            //if the country is used by atleast one author, DB will restict the deletion, but we want to display an error to the user
            var countrytoDelete = _countryRepository.GetCountry(countryId);
            if (_countryRepository.GetAuthorsFromCountry(countryId).Count > 0)
            {
                ModelState.AddModelError("", $"Country {countrytoDelete.Name} cannot be deleted because it is used by atleast one author");
                return StatusCode(409, ModelState); //Coflict
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_countryRepository.DeleteCountry(countrytoDelete))
            {
                ModelState.AddModelError("", $"Sonething went wrong deleting {countrytoDelete.Name}");
                return StatusCode(500, ModelState); //Server error
            }
            return NoContent();
        }
    }
}
