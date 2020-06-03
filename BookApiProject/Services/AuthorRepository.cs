using BookApi.Models;
using BookApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        private BookDbContext _bookDbContext;
        public AuthorRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }

        public bool AuthorExists(int authorId)
        {
            return _bookDbContext.Authors.Any(a => a.Id == authorId);
        }

        public bool CreateAuthor(Author author)
        {
            _bookDbContext.Add(author);
            return Save();
        }

        public bool DeleteAuthor(Author author)
        {
            _bookDbContext.Remove(author);
            return Save();
        }

        public Author GetAuthor(int authorId)
        {
            return _bookDbContext.Authors.Where(a => a.Id == authorId).FirstOrDefault();
        }

        public ICollection<Author> GetAuthors()
        {
            return _bookDbContext.Authors.OrderBy(a => a.LastName).ToList();
        }

        public ICollection<Author> GetAuthorsByBook(int bookId)
        {
            return _bookDbContext.BookAuthors.Where(a => a.BookId == bookId).Select(a => a.Author).ToList();
        }

        public ICollection<Book> GetBooksByAuthor(int authorId)
        {
            return _bookDbContext.BookAuthors.Where(a => a.AuthorId == authorId).Select(a => a.Book).ToList();
        }

        public bool Save()
        {
            var saved = _bookDbContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateAuthor(Author author)
        {
            _bookDbContext.Update(author);
            return Save();
        }
    }
}
