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

        // Local store for newly registered users
        private static readonly List<User> _localUsers = new();

        public AuthService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(ApiConstants.BaseUrl) };
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                // Check locally registered users first
                if (_localUsers.Any(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == password))
                    return true;

                // Fall back to JSONPlaceholder users (no real passwords, so just check username)
                var response = await _httpClient.GetAsync(ApiConstants.UsersEndpoint);
                if (!response.IsSuccessStatusCode) return false;

                var json = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<User>>(json);

                return users?.Any(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)) ?? false;
            }
            catch { return false; }
        }

        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                // Prevent duplicate usernames
                if (_localUsers.Any(u =>
                    u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                    return false;

                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ApiConstants.UsersEndpoint, content);
                if (!response.IsSuccessStatusCode) return false;

                // Save locally so login works afterward
                _localUsers.Add(user);
                return true;
            }
            catch { return false; }
        }
    }
}
