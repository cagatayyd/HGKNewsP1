using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HGKNews.Entities;
using HGKNews.Factories.Abstract;
using HGKNews.Services.Abstract;
using HGKNews.Models.NewsItem;
using OfficeOpenXml;
using HGKNews.Context;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using HGKNews.Factories;
using Microsoft.IdentityModel.Tokens;

namespace HGKNews.Controllers
{
    public class NewsController : Controller
    {
        #region Fields
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;
        private readonly INewsItemModelFactory _newsItemModelFactory;
        #endregion

        #region Ctor
        public NewsController(INewsRepository newsRepository, IMapper mapper, INewsItemModelFactory newsItemModelFactory)
        {
            _newsRepository = newsRepository ?? throw new ArgumentNullException(nameof(newsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _newsItemModelFactory = newsItemModelFactory ?? throw new ArgumentNullException(nameof(newsItemModelFactory));
        }
        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var model = await _newsItemModelFactory.PrepareNewsItemModelForListAsync(new NewsItemModel());
            var sortedNews = model.OrderByDescending(news => news.CreateOn).ToList();
            return View("List", sortedNews);
        }

        [HttpGet("News/Details/{newsId}")]
        public async Task<IActionResult> Details(int newsId)
        {
            var news = await _newsRepository.GetNewsByIdAsync(newsId);
            var newsModel = _mapper.Map<NewsItemModel>(news);

            return View("Details", newsModel);
        }

        [HttpGet]
        public async Task<IActionResult> NewsTable()
        {
            var model = await _newsItemModelFactory.PrepareNewsItemModelForListAsync(new NewsItemModel());
            return View("NewsTable", model);
        }
        [HttpPost]
        public async Task<IActionResult> List(NewsItemModel model)
        {
            var newsItemModel = await _newsItemModelFactory.PrepareNewsItemModelForListAsync(model);
            return View(newsItemModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var model = await _newsItemModelFactory.PrepareNewsItemModelAsync(new NewsItemModel(), null);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(NewsItemModel news)
        {
            var finalNews = _mapper.Map<NewsItem>(news);

            await _newsRepository.AddNewsAsync(finalNews);

            return RedirectToAction("List");
        }

        [HttpGet("News/Edit/{newsId}")]
        public async Task<IActionResult> Edit(int newsId)
        {
            var newsItem = await _newsRepository.GetNewsByIdAsync(newsId);

            if (newsItem == null)
            {
                return NotFound();
            }

            var newsModel = await _newsItemModelFactory.PrepareNewsItemModelAsync(null, newsItem);

            return View(newsModel);
        }


        [HttpPost("News/Edit/{newsId}")]
        public async Task<IActionResult> Edit(int newsId, NewsItemModel newsItemModel)
        {
            var existingNews = await _newsRepository.GetNewsByIdAsync(newsId);
            if (existingNews == null)
            {
                return NotFound();
            }

            existingNews.Title = newsItemModel.Title;
            existingNews.Content = newsItemModel.Content;
            existingNews.Author = newsItemModel.Author;
            existingNews.NewsDate = newsItemModel.NewsDate;
            existingNews.Category = newsItemModel.Category;
            existingNews.Country = newsItemModel.Country;

            _mapper.Map<NewsItem>(existingNews);

            await _newsRepository.EditNewsAsync(newsId, existingNews);

            return RedirectToAction("List");
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int newsId)
        {
            var newsEntity = await _newsRepository.GetNewsByIdAsync(newsId);

            if (newsEntity == null)
            {
                return NotFound();
            }

            await _newsRepository.DeleteNewsAsync(newsId);

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult ExportExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            List<NewsItemModel> newsItemModels = _newsRepository.GetNewsList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Report");

                worksheet.Cells["A3"].Value = "Downloaded At";
                worksheet.Cells["B3"].Value = DateTimeOffset.Now.ToString("dd MM yyyy") + DateTimeOffset.Now.ToString("H:mm tt");

                var headers = new List<string> { "Id", "Title", "Content", "Author", "CreateOn", "NewsDate", "Category", "Country", "NewsPicture" };
                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[6, i + 1].Value = headers[i];

                    var cell = worksheet.Cells[6, i + 1];
                    var fill = cell.Style.Fill;
                    fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int rowStart = 7;

                foreach (var item in newsItemModels)
                {
                    worksheet.Cells[rowStart, 1].Value = item.Id;
                    worksheet.Cells[rowStart, 2].Value = item.Title;
                    worksheet.Cells[rowStart, 3].Value = item.Content;
                    worksheet.Cells[rowStart, 4].Value = item.Author;
                    worksheet.Cells[rowStart, 5].Value = item.CreateOn.ToString("dd.MM.yyyy HH:mm:ss");
                    worksheet.Cells[rowStart, 6].Value = item.NewsDate.ToString("dd.MM.yyyy");
                    worksheet.Cells[rowStart, 7].Value = item.Category;
                    worksheet.Cells[rowStart, 8].Value = item.Country;
                    worksheet.Cells[rowStart, 9].Value = item.NewsPicture;
                    rowStart++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var content = package.GetAsByteArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NewsDataTable.xlsx");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Choose an Excel file.");
                return View();
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    int rowCount = worksheet.Dimension.Rows;

                    var newsItems = new List<NewsItem>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var item = new NewsItemModel
                        {
                            Title = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                            Content = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                            Author = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                            NewsDate = DateTime.TryParse(worksheet.Cells[row, 6].Text, out DateTime newsDate)
                                ? newsDate
                                : DateTime.MinValue,
                            Category = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                            Country = worksheet.Cells[row, 8].Value?.ToString() ?? "",
                        };
                        if (worksheet.Cells[1, 2].Value != "Title"
                            && worksheet.Cells[1, 3].Value != "Content"
                            && worksheet.Cells[1, 4].Value != "Author"
                            && worksheet.Cells[1, 5].Value != "NewsDate"
                            && worksheet.Cells[1, 6].Value != "CreateOn"
                            && worksheet.Cells[1, 7].Value != "Category"
                            && worksheet.Cells[1, 8].Value != "Country")
                        {
                            return RedirectToAction("NewsTable");
                        }
                        else
                        {
                            if (item.Title != "" || item.Content != "" || item.Author != "" || item.Category != "" || item.Country != "")
                            {
                                item.CreateOn = DateTime.Now;
                                var finalNews = _mapper.Map<NewsItem>(item);
                                await _newsRepository.AddNewsAsync(finalNews);
                            }
                        }
                    }
                }
            }
            return RedirectToAction("NewsTable");
        }
    }
}

#endregion
