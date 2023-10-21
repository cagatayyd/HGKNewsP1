using System.ComponentModel.DataAnnotations;

namespace HGKNews.Entities
{
    public class NewsItem : BaseEntity
    {
        [Required]
        [StringLength(30)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public byte[] NewsPicture { get; set; }
        public DateTime NewsDate {  get; set; }
        public DateTime CreateOn { get; set; } = DateTime.Now;
        public string Category { get; set; }
        public string Author { get; set; }
        public string Country { get; set; }
        public bool IsDeleted { get; set; }

    }
}
