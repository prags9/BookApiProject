using BookApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
    public interface IAuthorRepository
    {
        Author GetAuthor(int authorId);
        ICollection<Author> GetAuthors();
        ICollection<Author> GetAuthorsByBook(int bookId);
        ICollection<Book> GetBooksByAuthor(int authorId);
        Boolean AuthorExists(int authorId);
        bool CreateAuthor(Author author);
        bool UpdateAuthor(Author author);
        bool DeleteAuthor(Author author);
        bool Save();
    }
}
