namespace DropBoxMarket.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<CategoryCard> TopCategories { get; set; } = Enumerable.Empty<CategoryCard>();
        public IEnumerable<Product> FeaturedProducts { get; set; } = Enumerable.Empty<Product>();
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrdersForUser { get; set; } 
        public string? SearchTerm { get; set; }
    }

    public class CategoryCard
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int ProductCount { get; set; }
    }
}
