using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV8WebRazorPages.ViewModels
{
    public class VentaInsertarViewModel
    {
        public int OrderID { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        public string? CustomerID { get; set; } = "";
        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un vendedor.")]
        public int? EmployeeID { get; set; }
        [Required(ErrorMessage = "Debe indicar la fecha de la venta.")]
        public DateTime? OrderDate { get; set; }
        public TimeSpan? OrderTime { get; set; }
        public DateTime? RequiredDate { get; set; }
        public TimeSpan? RequiredTime { get; set; }
        public DateTime? ShippedDate { get; set; }
        public TimeSpan? ShippedTime { get; set; }
        public int? ShipVia { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public decimal Freight { get; set; }
        public byte[]? RowVersion { get; set; }
        public decimal TasaIVA { get; set; }
        public List<VentaDetalleViewModel> Detalles { get; set; } = new();
    }
}
