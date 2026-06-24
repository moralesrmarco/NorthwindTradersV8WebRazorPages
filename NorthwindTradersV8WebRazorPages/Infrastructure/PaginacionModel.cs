namespace NorthwindTradersV8WebRazorPages.Infrastructure
{
    public class PaginacionModel
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public string PageName { get; set; } = string.Empty;
        public bool Buscar { get; set; }
        public bool MostrarClientes { get; set; }
        public bool MostrarProveedores { get; set; }
        public string? CiudadPaisSeleccionado { get; set; }
    }
}
