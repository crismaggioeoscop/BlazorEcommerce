using BlazorApp1.Client.Shared;
using BlazorApp1.Shared.DTO;

namespace BlazorApp1.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnChange;
        Task Addtocart(CartItem cartItem);
        Task<List<CartProductResponse>> GetCartProducts();
        Task RemoveProductFromCart(int productId, int productTypeId);
        Task UpdateQuantity(CartProductResponse product);
        Task StoreCartItems(bool emptyLocalCart);
        Task GetCartItemsCount();
    }
}
