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
        public async Task<bool> CreatePostAsync(Post post)
        {
            return await Task.FromResult(false);
        }

        public async Task<bool> UpdatePostAsync(Post post)
        {
            return await Task.FromResult(false);
        }
    }
}
