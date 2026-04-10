using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocialMediaApp.Models;
using SocialMediaApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApp.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IPostService _postService;

        [ObservableProperty] private ObservableCollection<Post> posts = new();
        [ObservableProperty] private string newPostTitle = string.Empty;
        [ObservableProperty] private string newPostBody = string.Empty;
        [ObservableProperty] private Post? selectedPost;

        public object LoadPostsCommand { get; internal set; }

        public HomeViewModel(IPostService postService)
        {
            _postService = postService;
            Title = "Home";
        }

        [RelayCommand]
        private async Task LoadPostsAsync()
        {
            IsBusy = true;
            try
            {
                var result = await _postService.GetPostsAsync();
                Posts.Clear();
                // Load only first 20 for performance
                foreach (var post in result.Take(20))
                    Posts.Add(post);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task CreatePostAsync()
        {
            if (string.IsNullOrWhiteSpace(NewPostTitle) ||
                string.IsNullOrWhiteSpace(NewPostBody))
            {
                await Shell.Current.DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var post = new Post
                {
                    Title = NewPostTitle,
                    Body = NewPostBody,
                    UserId = 1
                };

                var success = await _postService.CreatePostAsync(post);
                if (success)
                {
                    // Add to top of list locally
                    Posts.Insert(0, new Post
                    {
                        Id = Posts.Count + 1,
                        Title = NewPostTitle,
                        Body = NewPostBody,
                        UserId = 1
                    });

                    NewPostTitle = string.Empty;
                    NewPostBody = string.Empty;

                    await Shell.Current.DisplayAlert("Success", "Post created!", "OK");
                }
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task DeletePostAsync(Post post)
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Delete", $"Delete '{post.Title}'?", "Yes", "No");

            if (!confirm) return;

            IsBusy = true;
            try
            {
                var success = await _postService.DeletePostAsync(post.Id);
                if (success)
                    Posts.Remove(post);
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task UpdatePostAsync(Post post)
        {
            string newTitle = await Shell.Current.DisplayPromptAsync(
                "Update Post", "Enter new title:", initialValue: post.Title);

            if (string.IsNullOrWhiteSpace(newTitle)) return;

            IsBusy = true;
            try
            {
                post.Title = newTitle;
                var success = await _postService.UpdatePostAsync(post);
                if (success)
                {
                    // Refresh list
                    var index = Posts.IndexOf(post);
                    if (index >= 0)
                    {
                        Posts.RemoveAt(index);
                        Posts.Insert(index, post);
                    }
                    await Shell.Current.DisplayAlert("Success", "Post updated!", "OK");
                }
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
