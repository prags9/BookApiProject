using BookApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int categoryId);
        //Assuming we can have multiplt categories for a book
        ICollection<Category> GetCategoriesofABook(int bookId);
        //Category GetCategoryofABook(int bookId);
        ICollection<Book> GetBooksForCategory(int categoryId);            
        Boolean CategoryExists(int categoryId);
        Boolean IsDuplicateCategoryName(int categoryId, string categoryName);
        Boolean CreateCategory(Category category);
        Boolean UpdateCategory(Category category);
        Boolean DeleteCategory(Category category);
        Boolean Save();
    }
}
