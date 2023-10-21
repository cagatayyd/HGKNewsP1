using AutoMapper;
using HGKNews.Models.NewsItem;

namespace HGKNews.Profiles
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<Entities.NewsItem, NewsItemModel>();
            CreateMap<NewsItemModel, Entities.NewsItem>();
        }
    }
}
