﻿
using BlazorApp1.Server.Data;

namespace BlazorApp1.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;

        public CategoryService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Category>>> AddCategory(Category category)
        {
            category.Editing = category.IsNew = false;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return await GetAdminCategories();
        }

        public async Task<ServiceResponse<List<Category>>> DeleteCategory(int categoryId)
        {
            Category category = await GetCategoryById(categoryId);

            if (category == null)
            {
                return new ServiceResponse<List<Category>>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            category.Deleted = true;
            await _context.SaveChangesAsync();

            return await GetAdminCategories();
        }

        public async Task<ServiceResponse<List<Category>>> GetAdminCategories()
        {
            var categories = await _context.Categories
                .Where(c => !c.Deleted )
                .ToListAsync();

            return new ServiceResponse<List<Category>>
            {
                Data = categories
            };
        }

        public async Task<ServiceResponse<List<Category>>> GetCategories()
        {
            var categories = await _context.Categories
                .Where(c => !c.Deleted && c.Visible)
                .ToListAsync();

            return new ServiceResponse<List<Category>>{
                Data = categories
            };
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<ServiceResponse<List<Category>>> UpdateCategory(Category category)
        {
            var dbCategory = await GetCategoryById(category.Id);
            if(dbCategory == null)
            {
                return new ServiceResponse<List<Category>>
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            dbCategory.Name = category.Name;
            dbCategory.Url = category.Url;
            dbCategory.Visible = category.Visible;

            await _context.SaveChangesAsync();

            return await GetAdminCategories();
        }
    }
}
