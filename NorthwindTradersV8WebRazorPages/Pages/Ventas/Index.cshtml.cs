using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Infrastructure;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Ventas
{
    public class IndexModel : PagedPageModel
    {
        private readonly VentaBLL ventaBLL;
        public DataTable Ventas { get; set; } = new();
        public IndexModel(IConfiguration configuration) : base(configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            ventaBLL = new VentaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public void OnGet(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            Ventas = ventaBLL.ObtenerVentasPaginadas(PageIndex, RowsPerPage, out int totalRegistros);
            CalculateTotalPages(totalRegistros);
        }
    }
}
