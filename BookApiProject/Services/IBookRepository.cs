using BookApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
    public interface IBookRepository
    {
        Book GetBook(int bookId);
        Book GetBook(string bookIsbn);
        ICollection<Book> GetBooks();
        Boolean IsDuplicateIsbn(int bookId, string isbn);
        decimal GetBookRating(int bookId);
        Boolean BookExists(int bookId);
        Boolean BookExists(string isbn);

        Boolean CreateBook(List<int> authorsId, List<int> categoiesId, Book book);
        Boolean UpdateBook(List<int> authorsId, List<int> categoiesId, Book book);
        Boolean DeleteBook(Book book);
        Boolean Save();
    }
}
