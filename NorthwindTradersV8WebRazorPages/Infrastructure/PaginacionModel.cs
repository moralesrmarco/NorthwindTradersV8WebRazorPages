namespace NorthwindTradersV8WebRazorPages.Infrastructure
{
    public class PaginacionModel
    {
        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public string PageName { get; set; } = string.Empty;
    }
}
