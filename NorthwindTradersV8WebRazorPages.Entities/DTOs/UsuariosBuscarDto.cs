namespace NorthwindTradersV8WebRazorPages.Entities.DTOs;

public class UsuariosBuscarDto
{
    public int? IdIni { get; set; }
    public int? IdFin { get; set; }
    public string? Paterno { get; set; }
    public string? Materno { get; set; }
    public string? Nombres { get; set; }
    public string? NombreUsuario { get; set; }
}
