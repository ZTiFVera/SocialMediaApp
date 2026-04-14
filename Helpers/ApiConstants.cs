using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApp.Helpers
{
    public static class ApiConstants
    {
        public const string BaseUrl = "https://69d8c5bd0576c9388259fac5.mockapi.io/api/v1";
        public const string UsersEndpoint = "/user";   // singular ← was /users
        public const string PostsEndpoint = "/post";   // singular ← was /posts
    }
}