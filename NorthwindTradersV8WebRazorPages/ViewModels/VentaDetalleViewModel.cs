namespace NorthwindTradersV8WebRazorPages.ViewModels
{
    public class VentaDetalleViewModel
    {
        public int? CategoriaID { get; set; }
        public int? ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
        public short Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal TasaIVA { get; set; } = 0.16m; //ConfiguracionFiscal.TasaIVA;
        public string? RowVersion { get; set; }
        // Base sin IVA (separando el impuesto del precio con IVA)
        // Precio unitario sin IVA después del descuento
        public decimal PrecioBaseSinIva => Math.Round(PrecioPorUnidadConIVADespuesDescuento / (1 + TasaIVA), 2, MidpointRounding.AwayFromZero);
        public decimal PrecioPorUnidadSinIVASinDescuento => Math.Round(UnitPrice / (1 + TasaIVA), 2, MidpointRounding.AwayFromZero);
        public decimal IVADelPrecioPorUnidadSinDescuento => Math.Round(UnitPrice - PrecioPorUnidadSinIVASinDescuento, 2, MidpointRounding.AwayFromZero);
        public decimal PrecioPorUnidadConIVADespuesDescuento => Math.Round(UnitPrice * (1 - Discount), 2, MidpointRounding.AwayFromZero);
        public decimal IVADelPrecioporUnidadDespuesDescuento => Math.Round(PrecioPorUnidadConIVADespuesDescuento - PrecioPorUnidadSinIVADespuesDescuento, 2, MidpointRounding.AwayFromZero);
        public decimal PrecioPorUnidadSinIVADespuesDescuento => Math.Round(PrecioPorUnidadConIVADespuesDescuento / (1 + TasaIVA), 2, MidpointRounding.AwayFromZero);
        public decimal AhorroPorUnidadSinIVA => Math.Round(PrecioPorUnidadSinIVASinDescuento - PrecioPorUnidadSinIVADespuesDescuento, 2, MidpointRounding.AwayFromZero);
        public decimal AhorroEnIVAPorUnidadDespuesDescuento => Math.Round(IVADelPrecioPorUnidadSinDescuento - IVADelPrecioporUnidadDespuesDescuento, 2, MidpointRounding.AwayFromZero);
        public decimal AhorroTotalPorUnidadConIVA => Math.Round(UnitPrice - PrecioPorUnidadConIVADespuesDescuento, 2, MidpointRounding.AwayFromZero);
        // Tasas expresadas en porcentaje
        public decimal TasaDescuentoPorcentaje => Math.Round(Discount * 100, 2, MidpointRounding.AwayFromZero);
        public decimal TasaIVAPorcentaje => Math.Round(TasaIVA * 100, 2, MidpointRounding.AwayFromZero);
        // Importe bruto (con IVA incluido)
        public decimal SubtotalDelImporteConIVAIncluido => Math.Round(UnitPrice * Quantity, 2, MidpointRounding.AwayFromZero);
        public decimal SubtotalDelImporteSinIVASinDescuento => Math.Round(PrecioPorUnidadSinIVASinDescuento * Quantity, 2, MidpointRounding.AwayFromZero);
        public decimal SubtotalDelImporteDelIVASinDescuento => Math.Round(IVADelPrecioPorUnidadSinDescuento * Quantity, 2, MidpointRounding.AwayFromZero);
        // Importe neto con descuento (todavía incluye IVA)
        public decimal SubtotalDelImporteConIVAConDescuento => Math.Round(SubtotalDelImporteConIVAIncluido * (1 - Discount), 2, MidpointRounding.AwayFromZero);
        public decimal SubtotalIVADespuesDelDescuento => Math.Round(SubtotalDelImporteConIVAConDescuento - SubtotalDelImporteSinIVAConDescuento, 2, MidpointRounding.AwayFromZero);
        public decimal SubtotalDelImporteSinIVAConDescuento => Math.Round(PrecioBaseSinIva * Quantity, 2, MidpointRounding.AwayFromZero);
        public decimal SubtotalDelAhorroSinIvaDespuesDescuento => Math.Round(AhorroPorUnidadSinIVA * Quantity, 2, MidpointRounding.AwayFromZero);
        public decimal SubtotalDelAhorroEnIVADespuesDescuento => Math.Round(AhorroEnIVAPorUnidadDespuesDescuento * Quantity, 2, MidpointRounding.AwayFromZero);
        public decimal SubtotalDelAhorroTotalDespuesDescuento => Math.Round(AhorroTotalPorUnidadConIVA * Quantity, 2, MidpointRounding.AwayFromZero);
        // Subtotal = Importe con descuento (ya incluye IVA)
        public decimal Subtotal => SubtotalDelImporteConIVAConDescuento; // ya viene redondeado
    }
}
