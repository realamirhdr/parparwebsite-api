using System.ComponentModel.DataAnnotations;

namespace ParParWebsite.Api.Models
{
    public class Portfolio
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Slug { get; set; }

        public string Caption { get; set; }

        public string? ThumbnailUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public List<PortfolioImage> Images { get; set; }
    }
}
