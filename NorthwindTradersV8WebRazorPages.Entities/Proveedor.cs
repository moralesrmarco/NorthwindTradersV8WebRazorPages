namespace NorthwindTradersV8WebRazorPages.Entities
{
    public class Proveedor
    {
        public int SupplierID { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? ContactTitle { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public byte[]? RowVersion { get; set; }

        // del diagrama entidad-relación podemos ver que
        // un proveedor puede tener muchos productos asociados
        public List<Producto> Productos { get; set; } = new List<Producto>();
    }
}
