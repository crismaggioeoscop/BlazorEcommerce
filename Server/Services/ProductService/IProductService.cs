namespace BlazorApp1.Server.Services.ProductServices
{
    public interface IProductService
    {
        Task<ServiceResponse<Product>> GetProductAsync(int productId);
        Task<ServiceResponse<List<Product>>> GetProductsAsync();
        Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl);
        Task<ServiceResponse<List<Product>>> FindProductsBySearchText(string searchText);
        Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText);
    }
}
