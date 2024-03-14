
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
        public async Task<ServiceResponse<List<Category>>> GetCategoriesAsync()
        {
            var categories = new ServiceResponse<List<Category>>()
            {
                Data = await _context.Categories.ToListAsync()
            };

            return categories;
        }

        //public Task<ServiceResponse<Category>> GetCategoryAsync(int categoryId)
        //{ 
        //}
    }
}
