using System.ComponentModel.DataAnnotations;

namespace ParParWebsite.Api.DTOs;

public class PortfolioDTO
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Caption { get; set; }

    public string? ThumbnailUrl { get; set; }

    public List<PortfolioImageDTO> Images { get; set; }
}