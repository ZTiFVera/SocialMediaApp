using Newtonsoft.Json;
using SocialMediaApp.Helpers;
using SocialMediaApp.Models;
using System.Text;

namespace SocialMediaApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient();
        }

        public Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var storedUsers = GetStoredUsers();

                System.Diagnostics.Debug.WriteLine($"[Login] Checking {storedUsers.Count} stored users");
                foreach (var u in storedUsers)
                    System.Diagnostics.Debug.WriteLine($"[Login] Stored: '{u.Username}' / '{u.Password}'");
                System.Diagnostics.Debug.WriteLine($"[Login] Trying: '{username}' / '{password}'");

                var found = storedUsers.Any(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == password);

                return Task.FromResult(found);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Login] Exception: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                // Check duplicate
                var storedUsers = GetStoredUsers();
                if (storedUsers.Any(u =>
                    u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    System.Diagnostics.Debug.WriteLine($"[Register] Duplicate username: {user.Username}");
                    return false;
                }

                // Post to MockAPI
                var url = ApiConstants.BaseUrl + ApiConstants.UsersEndpoint;
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"[Register] POST {url} | {json}");

                var response = await _httpClient.PostAsync(url, content);
                var raw = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"[Register] Status: {response.StatusCode} | Body: {raw}");

                // Save locally regardless of API result
                // so login always works after register
                storedUsers.Add(user);
                SaveStoredUsers(storedUsers);

                System.Diagnostics.Debug.WriteLine($"[Register] Saved user locally. Total: {storedUsers.Count}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Register] Exception: {ex.Message}");
                return false;
            }
        }

        private List<User> GetStoredUsers()
        {
            try
            {
                var json = Preferences.Get("registered_users", "[]");
                System.Diagnostics.Debug.WriteLine($"[Storage] Raw: {json}");
                return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        private void SaveStoredUsers(List<User> users)
        {
            try
            {
                var json = JsonConvert.SerializeObject(users);
                Preferences.Set("registered_users", json);
                System.Diagnostics.Debug.WriteLine($"[Storage] Saved: {json}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Storage] Save failed: {ex.Message}");
            }
        }
    }
}