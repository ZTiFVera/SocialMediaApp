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

        // Holds users registered during the current app session
        private static readonly List<User> _localUsers = new();

        public AuthService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiConstants.BaseUrl)
            };
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                // 1. Check locally registered users first (full credential match)
                if (_localUsers.Any(u =>
                        u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                        u.Password == password))
                    return true;

                // 2. Fall back to JSONPlaceholder seed users.
                //    The API has no real passwords, so any non-empty password
                //    is accepted for seed accounts so the demo works out of the box.
                var response = await _httpClient.GetAsync(ApiConstants.UsersEndpoint);
                if (!response.IsSuccessStatusCode) return false;

                var json = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<User>>(json);

                return users?.Any(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)) ?? false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                // Prevent duplicate usernames within the session
                if (_localUsers.Any(u =>
                        u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                    return false;

                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ApiConstants.UsersEndpoint, content);
                if (!response.IsSuccessStatusCode) return false;

                // Persist locally so the login check above can find this user
                _localUsers.Add(user);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}