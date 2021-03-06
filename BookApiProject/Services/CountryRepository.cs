﻿using BookApi.Models;
using BookApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
    public class CountryRepository : ICountryRepository
    {
        private BookDbContext _countryContext;

        public ICollection<Author> GetAuthorsFromCountry(int countryId)
        {
            return _countryContext.Authors.Where(x => x.Country.Id == countryId).ToList();
        }

        public CountryRepository(BookDbContext countryContext)
        {
            _countryContext = countryContext;
        }

        public ICollection<Country> GetCountries()
        {
            return _countryContext.Countries.OrderBy(c => c.Name).ToList();
        }

        public Country GetCountry(int countryId)
        {
            return _countryContext.Countries.Where(c => c.Id == countryId).FirstOrDefault();
        }

        public Country GetCountryofAnAuthor(int authorId)
        {
            return _countryContext.Authors.Where(a => a.Id == authorId).Select(c => c.Country).FirstOrDefault();
        }

        public bool CountryExists(int countryId)
        {
            return _countryContext.Countries.Any(x => x.Id == countryId);
        }

        public bool IsDuplicateCountryName(int countryId, string countryName)
        {
            //return _bookDbContext.Books.Any(a => a.Isbn == isbn);            
            var country = _countryContext.Countries.Where(b => b.Name.Trim().ToUpper() == countryName.Trim().ToUpper() && b.Id != countryId).FirstOrDefault();
            return country == null ? false : true;
        }

        public bool CreateCountry(Country country)
        {
            _countryContext.Add(country);
            return Save();
        }

        public bool UpdateCountry(Country country)
        {
            _countryContext.Update(country);
            return Save();
        }

        public bool DeleteCountry(Country country)
        {
            _countryContext.Remove(country);
            return Save();
        }

        public bool Save()
        {
            var saved = _countryContext.SaveChanges();
            return saved >= 0 ? true : false;
        }
    }
}
