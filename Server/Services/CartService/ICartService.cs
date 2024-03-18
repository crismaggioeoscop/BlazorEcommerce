using BlazorApp1.Client.Shared;
using BlazorApp1.Shared.DTO;

namespace BlazorApp1.Server.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems); 
    }
}
