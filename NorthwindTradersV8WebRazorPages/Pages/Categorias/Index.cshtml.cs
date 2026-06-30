using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.DAL;
using NorthwindTradersV8WebRazorPages.Infrastructure;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Categorias
{
    public class IndexModel : PagedPageModel
    {
        private readonly CategoriaBLL categoriaBLL;
        public DataTable Categorias { get; set; } = new DataTable();
        public IndexModel(IConfiguration configuration) : base(configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection") ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            categoriaBLL = new CategoriaBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }
        public void OnGet(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            Categorias = categoriaBLL.ObtenerCategoriasPaginadas(PageIndex, RowsPerPage, out int totalRegistros);
            CalculateTotalPages(totalRegistros);
        }
    }
}
