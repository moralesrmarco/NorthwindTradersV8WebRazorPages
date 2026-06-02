using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV8WebRazorPages.Entities
{
    public class Producto
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public Proveedor? Proveedor { get; set; }
        public Categoria? Categoria { get; set; }
        public string? QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public byte[]? RowVersion { get; set; }

        public string RowVersionStr
        {
            get
            {
                if (RowVersion == null || RowVersion.Length < 8)
                    return string.Empty;

                return BitConverter.ToInt64(RowVersion, 0).ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    RowVersion = null;
                    return;
                }

                RowVersion = BitConverter.GetBytes(long.Parse(value));
            }
        }


        // del diagrama entidad-relación podemos ver que
        // un producto tiene muchaas ventasdetalle asociadas
        public List<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
    }
}
