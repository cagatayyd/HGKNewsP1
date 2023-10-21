using Microsoft.EntityFrameworkCore;
using HGKNews.Context;
using HGKNews.Entities;
using HGKNews.Services.Abstract;
using HGKNews.Models.NewsItem;

namespace HGKNews.Services
{
    public class NewsRepository : INewsRepository
    {
        #region Fields
            private readonly NewsDbContext _dbContext;
            private readonly IRepository<NewsItem> _repository;

        #endregion

        #region Ctor
        public NewsRepository(NewsDbContext dbContext, IRepository<NewsItem> repository)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _repository = repository;
        }
        #endregion

        #region Methods
        public async Task<IEnumerable<NewsItem>> GetNewsAsync()
        {
            return await _repository.GetAllAsync();
        }
        //public async Task<IEnumerable<NewsItem>> GetNewsAsync()
        //{
        //    var news = _dbContext.NewsItems.Where(x => !x.IsDeleted);
        //    return news;
        //}
        public List<NewsItemModel> GetNewsList() // For Excel 
        {
            var newsItems = _dbContext.NewsItems.ToList();

            return newsItems.Select(x => new NewsItemModel
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                Category = x.Category,
                NewsDate = x.NewsDate,
                Country = x.Country,
                CreateOn = x.CreateOn,
                Author = x.Author,
                NewsPicture = x.NewsPicture,
            }).ToList();
        }

        public async Task<NewsItem?> GetNewsByIdAsync(int newsId)
        {
            return await _repository.GetByIdAsync(newsId);
        }
        public async Task AddNewsAsync(NewsItem news)
        {
            if (news != null)
            {
                await _repository.AddAsync(news);
                await _repository.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<NewsItem>> SearchNewsAsync(string? searchQuery, string? title)
        {
            var collection = _dbContext.NewsItems as IQueryable<NewsItem>;

            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.Trim();
                collection = collection.Where(c => c.Title.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a =>
                    a.Title.Contains(searchQuery) ||
                    (a.Content != null && a.Content.Contains(searchQuery)) ||
                    (a.Country != null && a.Country.Contains(searchQuery))
                );
            }

            var totalItemCount = await collection.CountAsync();

            var collectionToReturn = await collection.OrderBy(c => c.Title)
                .ToListAsync();

            return collectionToReturn;
        }

        public async Task EditNewsAsync(int newsId, NewsItem news)
        {
            await _repository.EditAsync(newsId,news);
        }
        //public async Task DeleteNewsAsync(int newsId)
        //{
        //    var news = await _repository.GetByIdAsync(newsId);
        //    if (news != null)
        //    {
        //        await _repository.DeleteAsync(newsId);
        //        await _repository.SaveChangesAsync();
        //    }
        //}
        public async Task DeleteNewsAsync(int newsId)
        {
            var news = await _repository.GetByIdAsync(newsId);
            if (news != null)
            {
                news.IsDeleted = true;
                await _repository.SaveChangesAsync();
            }
        }


        #endregion

    }
}
