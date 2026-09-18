using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Administracion;

public class AdministracionUsuariosModel : PageModel
{
    private readonly UsuarioBLL usuarioBLL;
    [BindProperty(SupportsGet = true)] 
    public UsuariosBuscarDto Filtro { get; set; } = new();
    [BindProperty] 
    public Usuario Usuario { get; set; } = new();
    [BindProperty] 
    public string? ConfirmarPassword { get; set; }
    public DataTable Usuarios { get; private set; } = new();
    public string Modo { get; private set; } = "crear";

    public AdministracionUsuariosModel(IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("NorthwindConnection") ?? throw new InvalidOperationException("Connection string not found");
        usuarioBLL = new UsuarioBLL(cs);
    }

    public void OnGet(int? id, string? modo)
    {
        Modo = modo is "editar" or "eliminar" 
            ? modo 
            : "crear";
        if (id.HasValue) 
            Usuario = usuarioBLL.ObtenerPorId(id.Value) ?? new Usuario();
        CargarUsuarios();
    }

    public IActionResult OnPostGuardar()
    {
        Modo = Usuario.Id == 0 ? "crear" : "editar";
        ValidarUsuario();
        if (!ModelState.IsValid) 
        { 
            CargarUsuarios(); 
            return Page(); 
        }
        try
        {
            if (Usuario.Id != 0 && string.IsNullOrWhiteSpace(Usuario.Password))
            {
                var usuarioActual = usuarioBLL.ObtenerPorId(Usuario.Id);
                if (usuarioActual == null) 
                { 
                    ModelState.AddModelError(string.Empty, "El usuario ya no existe."); 
                    CargarUsuarios(); 
                    return Page(); 
                }
                Usuario.Password = usuarioActual.Password;
            }
            else
            {
            Usuario.Password = PasswordHelper.GenerarHash(Usuario.Password.Trim());
            }
            var registros = Usuario.Id == 0
                ? usuarioBLL.Insertar(Usuario)
                : usuarioBLL.Actualizar(Usuario);

            if (registros > 0)
            {
                TempData["Exito"] = "El usuario se guardó correctamente.";
            }
            else if (registros == -1)
            {
                TempData["Error"] = "El usuario fue eliminado previamente por otro usuario de la red.";
            }
            else if (registros == -2)
            {
                TempData["Error"] = "El usuario fue modificado previamente por otro usuario de la red.";
            }
            else
            {
                TempData["Error"] = "No fue posible guardar el usuario.";
            }

            return RedirectToPage();
        }
        catch (Exception ex) 
        { 
            ModelState.AddModelError(string.Empty, ex.Message); 
            CargarUsuarios(); 
            return Page(); 
        }
    }

    public IActionResult OnPostEliminar()
    {
        Modo = "eliminar";

        try
        {
            var registros = usuarioBLL.Eliminar(Usuario);

            if (registros > 0)
            {
                TempData["Exito"] = "El usuario se eliminó correctamente.";
            }
            else if (registros == -1)
            {
                TempData["Error"] = "El usuario fue eliminado previamente por otro usuario de la red.";
            }
            else if (registros == -2)
            {
                TempData["Error"] = "El usuario fue modificado previamente por otro usuario de la red.";
            }
            else
            {
                TempData["Error"] = "No fue posible eliminar el usuario.";
            }

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            Usuario = usuarioBLL.ObtenerPorId(Usuario.Id) ?? Usuario;
            CargarUsuarios();
            return Page();
        }
    }
    private void ValidarUsuario()
    {
        if (string.IsNullOrWhiteSpace(Usuario.Nombres)) 
            ModelState.AddModelError("Usuario.Nombres", "El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(Usuario.NombreUsuario)) 
            ModelState.AddModelError("Usuario.NombreUsuario", "El usuario es obligatorio.");
        if (Usuario.Id == 0 && string.IsNullOrWhiteSpace(Usuario.Password)) 
            ModelState.AddModelError("Usuario.Password", "La contraseña es obligatoria.");
        if (!string.IsNullOrWhiteSpace(Usuario.Password) && Usuario.Password != ConfirmarPassword) 
            ModelState.AddModelError("ConfirmarPassword", "Las contraseñas no coinciden.");
        if (!string.IsNullOrWhiteSpace(Usuario.NombreUsuario) && usuarioBLL.ExisteNombreUsuario(Usuario.NombreUsuario.Trim(), Usuario.Id == 0 ? null : Usuario.Id)) 
            ModelState.AddModelError("Usuario.NombreUsuario", "El usuario ya existe.");
    }

    private void CargarUsuarios() => Usuarios = usuarioBLL.Buscar(Filtro);
}
