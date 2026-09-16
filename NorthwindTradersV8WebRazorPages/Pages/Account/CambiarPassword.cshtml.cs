using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;

namespace NorthwindTradersV8WebRazorPages.Pages.Account
{
    [Authorize]
    public class CambiarPasswordModel : PageModel
    {
        private readonly string connectionString;
        private readonly UsuarioBLL usuarioBLL;
        private const string SessionIntentosCambiarPassword =
            "CambiarPassword_Intentos";
        public CambiarPasswordModel(IConfiguration configuration)
        {
            connectionString =
                configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'NorthwindConnection'.");

            usuarioBLL = new UsuarioBLL(connectionString);
        }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        public IActionResult OnPost([FromBody] CambiarPasswordInput input)
        {
            // =====================================================
            // Obtener usuario del usuario autenticado
            // =====================================================

            string? usuario = User.FindFirst("Usuario")?.Value;

            if (string.IsNullOrWhiteSpace(usuario))
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "No se pudo identificar al usuario autenticado."
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }


            // =====================================================
            // Validar información recibida
            // =====================================================

            if (input == null)
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "No se recibieron los datos."
                });
            }

            if (string.IsNullOrWhiteSpace(input.ContrasenaActual))
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "Debe ingresar su contraseña actual."
                });
            }

            if (string.IsNullOrWhiteSpace(input.NuevaContrasena))
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "La nueva contraseña es obligatoria."
                });
            }

            if (string.IsNullOrWhiteSpace(input.ConfirmarContrasena))
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "La confirmación de la contraseña es obligatoria."
                });
            }


            // =====================================================
            // Validar que las contraseñas coincidan
            // =====================================================

            if (input.NuevaContrasena != input.ConfirmarContrasena)
            {
                return new JsonResult(new
                {
                    ok = false,
                    mensaje = "La nueva contraseña y la confirmación de la contraseña no coinciden."
                });
            }


            // =====================================================
            // Validar contraseña actual
            // =====================================================

            string passwordActualHasheada =
                PasswordHelper.GenerarHash(input.ContrasenaActual.Trim());

            byte numRegs =
                usuarioBLL.ValidarContraseñaActual(
                    usuario,
                    passwordActualHasheada);


            if (numRegs == 0)
            {
                int intentos = HttpContext.Session.GetInt32(
                    SessionIntentosCambiarPassword) ?? 0;

                intentos++;

                HttpContext.Session.SetInt32(
                    SessionIntentosCambiarPassword,
                    intentos);

                if (intentos >= 3)
                {
                    HttpContext.Session.Remove(
                        SessionIntentosCambiarPassword);

                    return new JsonResult(new
                    {
                        ok = false,
                        cerrar = true,
                        mensaje = "Demasiados intentos fallidos.\n\nPor favor, inténtelo de nuevo más tarde."
                    });
                }

                return new JsonResult(new
                {
                    ok = false,
                    cerrar = false,
                    mensaje = "La contraseña actual es incorrecta."
                });
            }

            HttpContext.Session.Remove(
                SessionIntentosCambiarPassword);

            // =====================================================
            // Actualizar contraseña
            // =====================================================

            string nuevaContrasenaHasheada =
                PasswordHelper.GenerarHash(input.NuevaContrasena.Trim());

            byte registrosActualizados =
                usuarioBLL.ActualizarContraseña(
                    usuario,
                    nuevaContrasenaHasheada);


            if (registrosActualizados > 0)
            {
                return new JsonResult(new
                {
                    ok = true,
                    mensaje = "Contraseña cambiada correctamente."
                });
            }


            return new JsonResult(new
            {
                ok = false,
                mensaje = "No se pudo cambiar la contraseña. Verifique que su cuenta esté activa."
            });
        }

        public class CambiarPasswordInput
        {
            public string ContrasenaActual { get; set; } = string.Empty;

            public string NuevaContrasena { get; set; } = string.Empty;

            public string ConfirmarContrasena { get; set; } = string.Empty;
        }
    }
}