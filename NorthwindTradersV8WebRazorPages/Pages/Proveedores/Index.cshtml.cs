using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Infrastructure;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Proveedores
{
    public class IndexModel : PagedPageModel
    {
        private readonly ProveedorBLL proveedorBLL;
        public DataTable Proveedores { get; set; } = new DataTable();
        public IndexModel(IConfiguration configuration) : base(configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            proveedorBLL = new ProveedorBLL(connectionString);
        }
        public void OnGet(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            Proveedores = proveedorBLL.ObtenerProveedoresPaginados(PageIndex, RowsPerPage, out int totalRegistros);
            CalculateTotalPages(totalRegistros);
        }
    }
}
