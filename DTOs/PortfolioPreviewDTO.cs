using Microsoft.AspNetCore.Mvc;

namespace ParParWebsite.Api.DTOs;

public class PortfolioPreviewDTO
{
    public int Id { get; set; }
    public string ThumbnailUrl { get; set; }
    public string Title { get; set; }
    public string Caption { get; set; }
}