using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes
{
    public class IndexModel : PageModel
    {
        private readonly ClienteBLL clienteBLL;
        public DataTable Clientes { get; set; } = new DataTable();
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        private int PageSize;
        public IndexModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            PageSize = configuration.GetValue<int>("AppSettings:pageSize");
            clienteBLL = new ClienteBLL(connectionString);
        }
        public void OnGet(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            Clientes = clienteBLL.ObtenerClientesPaginados(PageIndex, PageSize, out int totalRegistros);
            TotalPages = (int)Math.Ceiling(totalRegistros / (double)PageSize);
        }
    }
}
