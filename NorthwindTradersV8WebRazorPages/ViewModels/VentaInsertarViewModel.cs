namespace NorthwindTradersV8WebRazorPages.ViewModels
{
    public class VentaInsertarViewModel
    {
        public string? CustomerID { get; set; } = "";
        public int? EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public decimal? Freight { get; set; }
        public List<VentaDetalleViewModel> Detalles { get; set; } = new();
    }
}
