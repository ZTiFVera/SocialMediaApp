using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SocialMediaApp.Models;
using SocialMediaApp.Services;
using System.Collections.ObjectModel;

namespace SocialMediaApp.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IPostService _postService;

        [ObservableProperty]
        private ObservableCollection<Post> posts = new();

        [ObservableProperty]
        private string newPostTitle = string.Empty;

        [ObservableProperty]
        private string newPostBody = string.Empty;

        [ObservableProperty]
        private Post? selectedPost;

        public HomeViewModel(IPostService postService)
        {
            _postService = postService;
            Title = "Home";
        }

        [RelayCommand]
        private Task LoadPostsAsync()
        {
            Posts.Clear();
            return Task.CompletedTask;
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
                    Posts.Insert(0, new Post
                    {
                        Id = Posts.Count + 1,
                        Title = NewPostTitle,
                        Body = NewPostBody,
                        UserId = 1
                    });
                    Preferences.Set("postsCount", Posts.Count);
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
                {
                    Posts.Remove(post);
                    Preferences.Set("postsCount", Posts.Count);
                }
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task UpdatePostAsync(Post post)
        {
            string? newTitle = await Shell.Current.DisplayPromptAsync(
                "Update Post", "Enter new title:", initialValue: post.Title);
            if (string.IsNullOrWhiteSpace(newTitle)) return;

            IsBusy = true;
            try
            {
                post.Title = newTitle;
                var success = await _postService.UpdatePostAsync(post);
                if (success)
                {
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