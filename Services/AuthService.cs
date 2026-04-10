using Newtonsoft.Json;
using SocialMediaApp.Helpers;
using SocialMediaApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(ApiConstants.BaseUrl) };
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiConstants.UsersEndpoint);
                if (!response.IsSuccessStatusCode) return false;

                var json = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<User>>(json);

                // Simulate login check by username only (JSONPlaceholder has no real passwords)
                return users?.Any(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)) ?? false;
            }
            catch { return false; }
        }

        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // POST to REST API (JSONPlaceholder simulates success)
                var response = await _httpClient.PostAsync(ApiConstants.UsersEndpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}
