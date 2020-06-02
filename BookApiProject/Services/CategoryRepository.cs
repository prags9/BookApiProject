using BookApi.Models;
using BookApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Services
{
   
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BookDbContext _bookDbContext;
        
        public CategoryRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }
        public bool CategoryExists(int categoryId)
        {
            return _bookDbContext.Categories.Any(c => c.Id == categoryId);
        }

        public bool CreateCategory(Category category)
        {
            _bookDbContext.Add(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _bookDbContext.Remove(category);
            return Save();
        }

        public ICollection<Book> GetBooksForCategory(int categoryId)
        {
            return _bookDbContext.BookCategories.Where(c => c.CategoryId == categoryId).Select(c => c.Book).ToList();
        }

        public ICollection<Category> GetCategories()
        {
            return _bookDbContext.Categories.OrderBy(c => c.Name).ToList() ;
        }

        public ICollection<Category> GetCategoriesofABook(int bookId)
        {
            return _bookDbContext.BookCategories.Where(c => c.BookId == bookId).Select(c => c.Category).ToList();
        }

        public Category GetCategory(int categoryId)
        {
            return _bookDbContext.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
        }

        public bool IsDuplicateCategoryName(int categoryId, string categoryName)
        {
            //return _bookDbContext.Books.Any(a => a.Isbn == isbn);            
            var category = _bookDbContext.Categories.Where(b => b.Name.Trim().ToUpper() == categoryName.Trim().ToUpper() && b.Id != categoryId).FirstOrDefault();
            return category == null ? false : true;
        }

        public bool Save()
        {
            var saved = _bookDbContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _bookDbContext.Update(category);
            return Save();
        }
    }
}
