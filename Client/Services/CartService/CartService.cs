using BlazorApp1.Client.Shared;
using BlazorApp1.Shared;
using BlazorApp1.Shared.DTO;
using BlazorApp1.Shared.Users;
using Blazored.LocalStorage;
using Microsoft.VisualBasic;

namespace BlazorApp1.Client.Services.CartService
{
    public class CartService : ICartService
    { 
        public event Action OnChange;
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly IAuthService _authService;

        public CartService(ILocalStorageService _localstorage, HttpClient httpClient, IAuthService authService)
        {
            _localStorage = _localstorage;
            _http = httpClient;
            _authService = authService;
        } 

        public async Task Addtocart(CartItem cartItem)
        {
            if(await _authService.IsUserAuthenticated())
            {
                await _http.PostAsJsonAsync("api/cart/add", cartItem);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    cart = new List<CartItem>();
                }

                var sameItem = cart.FirstOrDefault(x => x.ProductId == cartItem.ProductId
                               && x.ProductTypeId == cartItem.ProductTypeId);
                if (sameItem == null)
                {
                    cart.Add(cartItem);
                }
                else
                {
                    sameItem.Quantity += cartItem.Quantity;
                }

                await _localStorage.SetItemAsync("cart", cart);
            }

            await GetCartItemsCount();
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            if(await _authService.IsUserAuthenticated())
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("api/cart");
                return response.Data;
            }
            else
            {
                var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if(cartItems == null)
                {
                    return new List<CartProductResponse>();
                }
                var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);
                var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
                return cartProducts.Data;
            }
            
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            if(await _authService.IsUserAuthenticated())
            {
                await _http.DeleteAsync($"api/cart/{productId}/{productTypeId}");
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == productId
                    && x.ProductTypeId == productTypeId);
                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    await _localStorage.SetItemAsync("cart", cart);
                }
            }
        }

        public async Task StoreCartItems(bool emptyLocalCart)
        {
            var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if(localCart == null)
            {
                return;
            }

            await _http.PostAsJsonAsync("api/cart", localCart);

            if (emptyLocalCart)
            {
                await _localStorage.RemoveItemAsync("cart");
            }
        }

        public async Task GetCartItemsCount()
        {
            if(await _authService.IsUserAuthenticated())
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = response.Data;

                await _localStorage.SetItemAsync("cartItemsCount", count);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                await _localStorage.SetItemAsync("cartItemsCount", cart != null ? cart.Count : 0);

            }

            OnChange.Invoke();
        }

        public async Task UpdateQuantity(CartProductResponse product)
        {
            if(await _authService.IsUserAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductTypeId = product.ProductTypeId,
                    Quantity = product.Quantity
                };
                await _http.PutAsJsonAsync("api/cart/update-quantity", request);
            } 
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if(cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == product.ProductId
                                           && x.ProductTypeId == product.ProductTypeId);

                if (cartItem != null)
                {
                    cartItem.Quantity = product.Quantity;
                    await _localStorage.SetItemAsync("cart", cart);
                }
            }
        }
    }
}
