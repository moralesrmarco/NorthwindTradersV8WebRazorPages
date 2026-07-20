namespace NorthwindTradersV8WebRazorPages.Entities.DTOs
{
    public class VentaDto
    {
        public int OrderID { get; set; }
        public string? CustomerCompanyName { get; set; }
        public string? CustomerContactName { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string? EmployeeName { get; set; }
        public string? ShipperCompanyName { get; set; }
        public string? ShipName { get; set; }
        public string? RowVersionStr { get; set; }

    }
}
