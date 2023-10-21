using System.ComponentModel.DataAnnotations;

namespace HGKNews.Entities
{
    public abstract partial class BaseEntity
    {
        [Required]
        [Key]
        public int Id { get; set; }
    }
}
