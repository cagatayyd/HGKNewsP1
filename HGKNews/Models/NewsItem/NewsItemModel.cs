using System.ComponentModel.DataAnnotations;

namespace HGKNews.Models.NewsItem
{
    public class NewsItemModel : BaseModel
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public byte[] NewsPicture { get; set; }
        public DateTime NewsDate { get; set; }
        public DateTime CreateOn { get; set; } = DateTime.Now;
        public string searchQuery { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public string Country { get; set; }
        public bool IsDeleted { get; set; }

    }
}
