using AutoMapper;
using HGKNews.Entities;
using HGKNews.Factories.Abstract;
using HGKNews.Models.NewsItem;
using HGKNews.Services.Abstract;
using Microsoft.EntityFrameworkCore;


namespace HGKNews.Factories
{
    public class NewsItemModelFactory : INewsItemModelFactory
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IRepository<NewsItem> _newsItemRepository;

        #endregion

        #region Ctor

        public NewsItemModelFactory(IMapper mapper, IRepository<NewsItem> newsItemRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _newsItemRepository = newsItemRepository;
        }

        #endregion

        #region Methods
        public virtual Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem)
        {
            if (newsItem != null)
            {
                if (model == null)
                {
                    model = _mapper.Map<NewsItemModel>(newsItem);
                }
            }
            return Task.FromResult(model);
        }

        public Task<NewsItemModel> PrepareNewsItemModelForSearchAsync(NewsItemModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            return Task.FromResult(searchModel);
        }

        public virtual async Task<IEnumerable<NewsItemModel>> PrepareNewsItemModelForListAsync(NewsItemModel model)
        {
            var query = await _newsItemRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(model.searchQuery))
            {
                model.searchQuery = model.searchQuery.Trim();

                query = query.Where(item =>
                    item.Title.Contains(model.searchQuery) ||
                    (item.Content != null && item.Content.Contains(model.searchQuery))
                );
            }

            var totalItemCount = query.Count();

            var collectionToReturn = query.Where(x => !x.IsDeleted).OrderBy(c => c.Title).ToList(); //SoftDelete Gösterimi

            var resultModels = new List<NewsItemModel>();

            foreach (var item in collectionToReturn)
            {
                resultModels.Add(await PrepareNewsItemModelAsync(null, item));
            }

            return resultModels;

        }
        #endregion

    }
}
