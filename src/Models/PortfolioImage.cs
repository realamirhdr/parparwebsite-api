namespace ParParWebsite.Api.Models;

public class PortfolioImage
{
    public int Id { get; set; }
    public string? ImageUrl { get; set; }
    public int CollectionId { get; set; }
    public Portfolio Portfolio { get; set; }

}