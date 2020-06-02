using BookApi.Models;
using BookApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
    public class BookRepository : IBookRepository
    {
        private BookDbContext _bookDbContext;

        public BookRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }
        public Boolean BookExists(int bookId)
        {
            return _bookDbContext.Books.Any(a => a.Id == bookId);
        }

        public bool BookExists(string isbn)
        {
            return _bookDbContext.Books.Any(a => a.Isbn == isbn);
        }

        public ICollection<Book> GetBooks()
        {
            return _bookDbContext.Books.OrderBy(b => b.Title).ToList();
        }

        public Book GetBook(string isbn)
        {
            return _bookDbContext.Books.Where(b => b.Isbn == isbn).FirstOrDefault();
        }

        public Book GetBook(int bookId)
        {
            return _bookDbContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public decimal GetBookRating(int bookId)
        {
            var reviews = (_bookDbContext.Reviews.Where(b => b.Book.Id == bookId).Select(r => r.Rating));
             int sum = 0;
            if (reviews.Count() <= 0)
                return 0;
             foreach(var r in reviews)
             {
                 sum += r;
             }
             return Convert.ToDecimal(sum / reviews.Count());

            //way 2
            /*var reviews = _bookDbContext.Reviews.Where(b => b.Book.Id == bookId);
            if (reviews.Count() <= 0)
                return 0;
            return ((decimal)reviews.Sum(r => r.Rating) / reviews.Count());*/
        }

        public bool IsDuplicateIsbn(int bookid, string isbn)
        {
            //return _bookDbContext.Books.Any(a => a.Isbn == isbn);            
            var book = _bookDbContext.Books.Where(b => b.Isbn.Trim().ToUpper() == isbn.Trim().ToUpper() && b.Id != bookid).FirstOrDefault();
            return book == null ? false : true;
        }
    }
}
