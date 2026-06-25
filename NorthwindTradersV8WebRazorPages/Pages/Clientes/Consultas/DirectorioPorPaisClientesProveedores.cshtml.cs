using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.BLL.Services;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using NorthwindTradersV8WebRazorPages.Infrastructure;

namespace NorthwindTradersV8WebRazorPages.Pages.Clientes.Consultas
{
    public class DirectorioPorPaisClientesProveedoresModel : PagedPageModel
    {
        private readonly ClienteBLL clienteBLL;
        private readonly ClienteService clienteService;
        public int TotalRecords { get; private set; }
        public int TotalClientes { get; private set; }
        public int TotalProveedores { get; private set; }
        public override string PageName => "/Clientes/Consultas/DirectorioPorPaisClientesProveedores";
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
                    return "DirectorioPorPaisClientesProveedores";

                if (MostrarClientes)
                    return "DirectorioPorPaisClientes";

                if (MostrarProveedores)
                    return "DirectorioPorPaisProveedores";

                return "DirectorioPorPaisClientesProveedores";
            }
        }
        public string TituloDirectorio
        {
            get
            {
                if (MostrarClientes && MostrarProveedores)
                    return "Directorio de clientes y proveedores por país";

                if (MostrarClientes)
                    return "Directoriode clientes por país";

                if (MostrarProveedores)
                    return "Directorio de proveedores por país";

                return "Directorio de clientes y proveedores por país";
            }
        }
        public string MensajeResultados
        {
            get
            {
                if (Tipo == "DirectorioPorPaisClientes")
                    return $"Se encontraron {TotalClientes} cliente(s).";

                if (Tipo == "DirectorioPorPaisProveedores")
                    return $"Se encontraron {TotalProveedores} proveedor(es).";

                return $"Se encontraron {TotalClientes} cliente(s) y {TotalProveedores} proveedor(es), Total: {TotalRecords} registro(s)";
            }
        }
        [BindProperty(SupportsGet = true)]
        public string? PaisSeleccionado { get; set; }

        public List<SelectListItem> Paises { get; set; } = [];
        // Propiedades para la vista
        public List<ClienteProveedorDto> ClientesProveedores { get; set; } = new();

        public DirectorioPorPaisClientesProveedoresModel(IConfiguration configuration) : base(configuration)
        {
            var connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            bool ejecutarTiempoDemora = configuration.GetValue<bool>("AppSettings:ejecutarTiempoDemora");
            int tiempoDemora = configuration.GetValue<int>("AppSettings:tiempoDemora");
            clienteBLL = new ClienteBLL(connectionString, ejecutarTiempoDemora, tiempoDemora);
            clienteService = new ClienteService(connectionString);
        }
        public void OnGet(int pageIndex = 1)
        {
            LlenarCombo();
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
            if (string.IsNullOrWhiteSpace(PaisSeleccionado) || (!MostrarClientes && !MostrarProveedores))
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

            ClientesProveedores = clienteBLL.ObtenerClientesProveedoresPorPaisPaginados(Tipo, PaisSeleccionado, PageIndex, RowsPerPage, out int totalRegistros, out int totalClientes, out int totalProveedores);
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
                MostrarProveedores = MostrarProveedores,
                PaisSeleccionado = PaisSeleccionado
            };
        }
        private void LlenarCombo()
        {
            Paises = clienteService
                    .ObtenerPaisesVwCliProvCbo()
                    .Select(x => new SelectListItem
                    {
                        Text = x.Key,
                        Value = x.Value
                    })
                    .ToList();
        }
    }
}
