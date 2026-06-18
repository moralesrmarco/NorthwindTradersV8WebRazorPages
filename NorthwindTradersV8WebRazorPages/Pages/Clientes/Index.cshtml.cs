using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Infrastructure;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes
{
    public class IndexModel : PagedPageModel
    {
        private readonly ClienteBLL clienteBLL;
        public DataTable Clientes { get; set; } = new DataTable();
        public IndexModel(IConfiguration configuration) : base(configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            clienteBLL = new ClienteBLL(connectionString);
        }
        public void OnGet(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            Clientes = clienteBLL.ObtenerClientesPaginados(PageIndex, RowsPerPage, out int totalRegistros);
            CalculateTotalPages(totalRegistros);
        }
    }
}
