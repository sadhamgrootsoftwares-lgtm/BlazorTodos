using System.Net.Http.Json;
using System.Net.Http.Headers;
using BlazorTodos.Shared.DTOs;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorTodos.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<string> Register(RegisterRequestDto request)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync("api/auth/register", request);
                if (result.IsSuccessStatusCode)
                    return "Registration successful";

                return await result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<bool> Login(LoginRequestDto request)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync("api/auth/login", request);
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (response != null)
                    {
                        await _localStorage.SetItemAsync("authToken", response.Token);
                        ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(response.Token);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
