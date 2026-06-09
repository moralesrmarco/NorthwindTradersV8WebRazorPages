using System.ComponentModel.DataAnnotations;

namespace NorthwindTradersV8WebRazorPages.Entities.DTOs
{
    public class EmpleadosBuscarDto
    {
        public int? IdIni { get; set; }
        public int? IdFin { get; set; }
        [StringLength(10)] 
        public string? Nombres { get; set; }
        [StringLength(20)] 
        public string? Apellidos { get; set; }
        [StringLength(30)] 
        public string? Titulo { get; set; }
        [StringLength(60)]
        public string? Domicilio { get; set; }
        [StringLength(15)]
        public string? Ciudad { get; set; }
        [StringLength(15)]
        public string? Region { get; set; }
        [StringLength(10)]
        public string? CodigoP { get; set; }
        public string? Pais { get; set; }
        [StringLength(24)]
        public string? Telefono { get; set; }
    }
}
