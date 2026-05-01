using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Horizon.MVC.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Horizon.MVC.Models;

namespace Horizon.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void SetAuthHeader(string? token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { model.Email, model.Password });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null)
                {
                    result.Success = true;
                    return result;
                }
                return new AuthResponse { Success = false, Message = "Invalid response from server" };
            }

            try 
            {
                var errorResult = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return errorResult ?? new AuthResponse { Success = false, Message = "Login failed" };
            }
            catch 
            {
                var error = await response.Content.ReadAsStringAsync();
                return new AuthResponse { Success = false, Message = error ?? "Login failed" };
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", new 
            { 
                model.Email, 
                model.Password, 
                model.Role, 
                DisplayName = model.DisplayName ?? model.Email.Split('@')[0]
            });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null)
                {
                    result.Success = true;
                    return result;
                }
                return new AuthResponse { Success = true, Message = "Registration successful" };
            }

            try 
            {
                var errorResult = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return errorResult ?? new AuthResponse { Success = false, Message = "Registration failed" };
            }
            catch 
            {
                var error = await response.Content.ReadAsStringAsync();
                return new AuthResponse { Success = false, Message = error ?? "Registration failed" };
            }
        }

        public async Task<List<UserDto>> GetPendingTeachersAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/auth/pending-teachers");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UserDto>>() ?? new List<UserDto>();
            }
            return new List<UserDto>();
        }

        public async Task<bool> ApproveTeacherAsync(string token, string id, string notes)
        {
            SetAuthHeader(token);
            var response = await _httpClient.PostAsJsonAsync($"api/auth/approve-teacher/{id}", new { Notes = notes });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RejectTeacherAsync(string token, string id, string notes)
        {
            SetAuthHeader(token);
            var response = await _httpClient.PostAsJsonAsync($"api/auth/reject-teacher/{id}", new { Notes = notes });
            return response.IsSuccessStatusCode;
        }

        public async Task<List<UserDto>> GetApprovedUsersAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/auth/users/approved");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponseWrapper<List<UserDto>>>();
                return result?.Data ?? new List<UserDto>();
            }
            return new List<UserDto>();
        }
    }

    public class ApiResponseWrapper<T>
    {
        public bool Success { get; set; }
        public int Count { get; set; }
        public T? Data { get; set; }
    }
}


