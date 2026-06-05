using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos
{
    public class IndexModel : PageModel
    {
        private readonly ProductoBLL productoBLL;
        public DataTable Productos { get; set; }
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        private const int PageSize = 20;
        public IndexModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found"); 

            productoBLL = new ProductoBLL(connectionString);
        }
        public void OnGet(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            Productos = productoBLL.ObtenerProductosPaginados(PageIndex, PageSize, out int totalRegistros);
            TotalPages = (int)Math.Ceiling(totalRegistros / (double)PageSize);
        }
    }
}
