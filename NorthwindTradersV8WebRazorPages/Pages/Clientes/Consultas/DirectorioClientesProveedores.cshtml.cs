using Microsoft.AspNetCore.Mvc;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using NorthwindTradersV8WebRazorPages.Infrastructure;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes.Consultas
{
    public class ClientesProveedoresModel : PagedPageModel
    {
        private readonly ClienteBLL clienteBLL;
        public int TotalRecords { get; private set; }
        public int TotalClientes { get; private set; }
        public int TotalProveedores { get; private set; }
        public override string PageName => "/Clientes/Consultas/DirectorioClientesProveedores";
        [BindProperty(SupportsGet = true)]
        public bool MostrarClientes { get; set; } = true;
        [BindProperty(SupportsGet = true)]
        public bool MostrarProveedores { get; set; } = true;
        [BindProperty(SupportsGet = true)]
        public bool Buscar { get; set; }
        public PaginacionModel PaginacionDirectorio { get; set; } = new();
        public string Tipo
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "DirectorioClientesProveedores";

                if (MostrarClientes)
                    return "DirectorioClientes";

                if (MostrarProveedores)
                    return "DirectorioProveedores";

                return "DirectorioClientesProveedores";
            }
        }
        public string TituloDirectorio
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "Directorio de clientes y proveedores";

                if (MostrarClientes)
                    return "Directorio de clientes";

                if (MostrarProveedores)
                    return "Directorio de proveedores";

                return "Directorio de clientes y proveedores";
            }
        }
        public string MensajeResultados
        {
            get
            {
                if (Tipo == "DirectorioClientes")
                    return $"Se encontraron {TotalClientes} cliente(s).";

                if (Tipo == "DirectorioProveedores")
                    return $"Se encontraron {TotalProveedores} proveedor(es).";

                return $"Se encontraron {TotalClientes} cliente(s) y {TotalProveedores} proveedor(es), Total: {TotalRecords} registro(s)";
            }
        }
        public ClientesProveedoresModel(IConfiguration configuration) : base(configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            clienteBLL = new ClienteBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
        }

        // Propiedades para la vista
        public List<ClienteProveedorDto> ClientesProveedores { get; set; } = new();

        public void OnGet(int pageIndex = 1)
        {
            PageIndex = pageIndex;
            // Primera carga de la página
            if (!Buscar)
            {
                ClientesProveedores = new();
                TotalRecords = 0;
                TotalClientes = 0;
                TotalProveedores = 0;
                CalculateTotalPages(0);
                return;
            }
            // Debe seleccionar al menos una opción
            if (!MostrarClientes && !MostrarProveedores)
            {
                TempData["Error"] = StringsCommons.ErrorCriterioSelec;

                ClientesProveedores = new();
                TotalRecords = 0;
                TotalClientes = 0;
                TotalProveedores = 0;
                CalculateTotalPages(0);
                Buscar = false;
                return;
            }

            ClientesProveedores = clienteBLL.ObtenerClientesProveedoresPaginados(Tipo, PageIndex, RowsPerPage, out int totalRegistros, out int totalClientes, out int totalProveedores);
            TotalRecords = totalRegistros;
            TotalClientes = totalClientes;
            TotalProveedores = totalProveedores;
            CalculateTotalPages(totalRegistros);
            PaginacionDirectorio = new PaginacionModel
            {
                PageIndex = PageIndex,
                TotalPages = TotalPages,
                PageName = PageName,
                Buscar = Buscar,
                MostrarClientes = MostrarClientes,
                MostrarProveedores = MostrarProveedores
            };
        }
    }
}
