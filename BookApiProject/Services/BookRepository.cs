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

        public bool CreateBook(List<int> authorsId, List<int> categoiesId, Book book)
        {
            //Get all the Author object from the list of authorsId passed
            var authors = _bookDbContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();

            //Get all the Category object from the list of categoriesId passed
            var categories = _bookDbContext.Categories.Where(a => categoiesId.Contains(a.Id)).ToList();

            //Looping through Author object list and populating the BookAuthor table
            foreach(var a in authors)
            {
                var bookAuthor = new BookAuthor()
                {
                    Author = a,
                    Book = book
                };
                _bookDbContext.Add(bookAuthor);
            }

            //Looping through Category object list and populating the BookCategory table
            foreach (var c in categories)
            {
                var bookCategory = new BookCategory()
                {
                    Category = c,
                    Book = book
                };
                _bookDbContext.Add(bookCategory);
            }

            //Populating to Books table
            _bookDbContext.Books.Add(book);

            return Save();
        }

        public bool UpdateBook(List<int> authorsId, List<int> categoiesId, Book book)
        {
            //Get all the Author object from the list of authorsId passed
            var authors = _bookDbContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();

            //Get all the Category object from the list of categoriesId passed
            var categories = _bookDbContext.Categories.Where(a => categoiesId.Contains(a.Id)).ToList();

            //Deleting the associations from BookAuthor and BookCategory
            var bookAuthorsToDelete = _bookDbContext.BookAuthors.Where(a => a.BookId == book.Id);
            var bookCategoriesToDelete = _bookDbContext.BookCategories.Where(a => a.BookId == book.Id);

            //Removing from the table
            _bookDbContext.RemoveRange(bookAuthorsToDelete);
            _bookDbContext.RemoveRange(bookCategoriesToDelete);

            //Adding data to tables(BookAuthor and BookCategory) with updated data
            //Looping through Author object list and populating the BookAuthor table
            foreach (var a in authors)
            {
                var bookAuthor = new BookAuthor()
                {
                    Author = a,
                    Book = book
                };
                _bookDbContext.Add(bookAuthor);
            }

            //Looping through Category object list and populating the BookCategory table
            foreach (var c in categories)
            {
                var bookCategory = new BookCategory()
                {
                    Category = c,
                    Book = book
                };
                _bookDbContext.Add(bookCategory);
            }

            //Updating to Books table
            _bookDbContext.Books.Update(book);

            return Save();
        }

        public bool DeleteBook(Book book)
        {
            _bookDbContext.Remove(book);
            return Save();
        }

        public bool Save()
        {
            var saved = _bookDbContext.SaveChanges();
            return saved >= 0 ? true : false;
        }
    }
}
