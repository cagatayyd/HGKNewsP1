using HGKNews.Entities;
using HGKNews.Models.NewsItem;
using System.Collections.Generic;

namespace HGKNews.Factories.Abstract
{
    public interface INewsItemModelFactory
    {
        Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem);
        Task<NewsItemModel> PrepareNewsItemModelForSearchAsync(NewsItemModel model);
        Task<IEnumerable<NewsItemModel>> PrepareNewsItemModelForListAsync(NewsItemModel model);
    }
}
