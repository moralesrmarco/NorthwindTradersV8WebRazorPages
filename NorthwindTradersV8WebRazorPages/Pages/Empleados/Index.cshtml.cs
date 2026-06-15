using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Empleados
{
    public class IndexModel : PageModel
    {
        private readonly EmpleadoBLL empleadoBLL;
        public DataTable Empleados { get; set; } = new DataTable();
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        private int PageSize;
        public IndexModel(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            PageSize = configuration.GetValue<int>("AppSettings:pageSize");
            empleadoBLL = new EmpleadoBLL(connectionString);
        }
        public void OnGet(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            Empleados = empleadoBLL.ObtenerEmpleadosPaginados(PageIndex, PageSize, out int totalRegistros);
            TotalPages = (int)Math.Ceiling(totalRegistros / (double)PageSize);
        }
    }
}
