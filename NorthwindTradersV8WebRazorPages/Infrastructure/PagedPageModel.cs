using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NorthwindTradersV8WebRazorPages.Infrastructure
{
    public abstract class PagedPageModel : PageModel
    {
        protected PagedPageModel(IConfiguration configuration)
        {
            RowsPerPage = configuration.GetValue<int>("AppSettings:rowsPerPage");
        }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int RowsPerPage { get; }
        public virtual string PageName => "Index";
        public PaginacionModel Paginacion =>
            new()
            {
                PageIndex = PageIndex,
                TotalPages = TotalPages,
                PageName = PageName
            };

        protected void CalculateTotalPages(int totalRecords)
        {
            TotalPages =
                    (int)Math.Ceiling(totalRecords / (double)RowsPerPage);
        }
    }
}
