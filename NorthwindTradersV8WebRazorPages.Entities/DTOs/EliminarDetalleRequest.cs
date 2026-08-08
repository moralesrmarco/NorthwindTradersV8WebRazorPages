namespace NorthwindTradersV8WebRazorPages.Entities.DTOs
{
    public class EliminarDetalleRequest
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string? RowVersion { get; set; }
        public string? VentaRowVersion { get; set; }
    }
}
