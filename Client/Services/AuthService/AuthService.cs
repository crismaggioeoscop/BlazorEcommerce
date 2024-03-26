using BlazorApp1.Shared.Users;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp1.Client.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider) {
            _http = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/change-password", request.Password);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<string>> Login(UserLogin request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public async Task<ServiceResponse<int>> Register(UserRegister request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        }

        public async Task<bool> IsUserAuthenticated()
        {
            return (await _authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }
    }
}
