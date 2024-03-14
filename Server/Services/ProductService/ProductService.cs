﻿using BlazorApp1.Server.Data;

namespace BlazorApp1.Server.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>() { 
                Data = await _context.Products.Include(p => p.Variants).ToListAsync()
            };
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await _context.Products
                    .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower()))
                    .Include(p => p.Variants)
                    .ToListAsync()
            };
            return response;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        { 
            var response = new ServiceResponse<Product>(); 
            var product = await _context.Products.Include(p => p.Variants).ThenInclude(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                response.Success = false;
                response.Message = "Sorry, but this product does not exist";
            }
            else
            {
                response.Data = product;
            }

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> FindProductsBySearchText(string searchText)
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await _context.Products.Where(p => 
                    p.Title.ToLower().Contains(searchText.ToLower()) || 
                    p.Description.ToLower().Contains(searchText.ToLower())
                    )
                    .Include(p => p.Variants) 
                    .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
        {
            var products = (await FindProductsBySearchText(searchText)).Data; 
             
            List<string> suggestions = new List<string>();

            foreach (var product in products)
            {
                if(product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    suggestions.Add(product.Title);
                }

                if(product.Description != null)
                {
                    var punctuation = product.Description.Where(Char.IsPunctuation).Distinct().ToArray();
                    var words = product.Description.Split().Select(x => x.Trim(punctuation));

                    foreach (var word in words)
                    {
                        if(word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && !suggestions.Contains(word))
                        {
                            suggestions.Add(word);
                        }
                    }
                }
            }

            return new ServiceResponse<List<string>>()
            {
                Data = suggestions
            };
        }
    }
}
