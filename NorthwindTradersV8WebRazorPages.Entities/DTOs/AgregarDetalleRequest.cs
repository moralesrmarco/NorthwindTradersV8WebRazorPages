namespace NorthwindTradersV8WebRazorPages.Entities.DTOs
{
    public class AgregarDetalleRequest
    {
        public int OrderID { get; set; }
        public string? RowVersion { get; set; }
        public VentaDetalleRequest Detalle { get; set; } = new();
    }
}
