using HGKNews.Entities;
using HGKNews.Models.NewsItem;

namespace HGKNews.Services.Abstract
{
    public interface INewsRepository
    {
        Task<IEnumerable<NewsItem>> GetNewsAsync();
        Task<NewsItem?> GetNewsByIdAsync(int newsId);
        List<NewsItemModel> GetNewsList();
        Task<IEnumerable<NewsItem>> SearchNewsAsync(string? searchQuery, string? title);
        Task AddNewsAsync(NewsItem news);
        Task EditNewsAsync(int newsId, NewsItem news);
        Task DeleteNewsAsync(int newsId);
        
    }
}
