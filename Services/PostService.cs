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
    public class PostService : IPostService
    {
        private readonly HttpClient _httpClient;

        public PostService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(ApiConstants.BaseUrl) };
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiConstants.PostsEndpoint);
                if (!response.IsSuccessStatusCode) return new List<Post>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Post>>(json) ?? new List<Post>();
            }
            catch { return new List<Post>(); }
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            try
            {
                var json = JsonConvert.SerializeObject(post);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ApiConstants.PostsEndpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> UpdatePostAsync(Post post)
        {
            try
            {
                var json = JsonConvert.SerializeObject(post);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(
                    $"{ApiConstants.PostsEndpoint}/{post.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"{ApiConstants.PostsEndpoint}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}

