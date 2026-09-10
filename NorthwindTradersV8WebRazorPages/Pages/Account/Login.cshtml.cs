using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using System.Security.Claims;

namespace NorthwindTradersV8WebRazorPages.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly UsuarioBLL usuarioBLL;
        private readonly PermisoBLL permisoBLL;
        [BindProperty]
        public string Usuario { get; set; } = string.Empty;
        [BindProperty]
        public string Password { get; set; } = string.Empty;
        [BindProperty]
        public bool Recordarme { get; set; }
        public LoginModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("NorthwindConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            this.usuarioBLL = new UsuarioBLL(connectionString);
            this.permisoBLL = new PermisoBLL(connectionString);
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Usuario))
            {
                ModelState.AddModelError(
                    nameof(Usuario),
                    "Debe proporcionar el usuario.");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(
                    nameof(Password),
                    "Debe proporcionar la contraseña.");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // =====================================
            // GENERAR HASH SHA-256 DEL PASSWORD
            // =====================================

            string passwordHash =
                PasswordHelper.GenerarHash(Password);


            // =====================================
            // VALIDAR USUARIO
            // =====================================

            int idUsuario =
                usuarioBLL.ValidarUsuario(
                    Usuario,
                    passwordHash,
                    out string nombreUsuarioAutenticado);


            // =====================================
            // VALIDAR RESULTADO
            // =====================================

            if (idUsuario == 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El usuario o la contraseña son incorrectos.");

                return Page();
            }

            HashSet<int> permisosIds =
                permisoBLL.ObtenerPermisosPorUsuarioId(idUsuario);

            // =====================================
            // CREAR LOS CLAIMS DEL USUARIO
            // =====================================

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    idUsuario.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    nombreUsuarioAutenticado),

                new Claim(
                    "Usuario",
                    Usuario)
            };

            // =====================================
            // AGREGAR CLAIMS DE PERMISOS
            // =====================================

            foreach (int permisoId in permisosIds)
            {
                claims.Add(
                    new Claim(
                        "Permiso",
                        permisoId.ToString()));
            }

            // =====================================
            // CREAR LA IDENTIDAD
            // =====================================

            var claimsIdentity =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);


            // =====================================
            // CREAR EL PRINCIPAL
            // =====================================

            var claimsPrincipal =
                new ClaimsPrincipal(
                    claimsIdentity);
            // =====================================
            // PROPIEDADES DE AUTENTICACIÓN
            // =====================================
            var authProperties =
                new AuthenticationProperties
                {
                    IsPersistent = Recordarme
                };
            // =====================================
            // CREAR COOKIE
            // =====================================
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            return RedirectToPage("/Index");
        }
    }
}