using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Infrastructure;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Productos
{
    public class IndexModel : PagedPageModel
    {
        private readonly ProductoBLL productoBLL;
        public DataTable Productos { get; set; } = new DataTable();
        public IndexModel(IConfiguration configuration) : base(configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            productoBLL = new ProductoBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public void OnGet(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            Productos = productoBLL.ObtenerProductosPaginados(PageIndex, RowsPerPage, out int totalRegistros);
            CalculateTotalPages(totalRegistros);
        }
    }
}
