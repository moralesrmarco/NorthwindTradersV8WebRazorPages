using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindTradersV8WebRazorPages.BLL;
using NorthwindTradersV8WebRazorPages.Common;
using NorthwindTradersV8WebRazorPages.Entities;
using NorthwindTradersV8WebRazorPages.Entities.DTOs;
using System.Data;

namespace NorthwindTradersV8WebRazorPages.Pages.Administracion
    {
        [Authorize]
        public class AdministracionPermisosModel : PageModel
        {
            private readonly UsuarioBLL usuarioBLL;
            private readonly PermisoBLL permisoBLL;

            public DataTable Usuarios { get; set; } = new();

            public DataTable CatalogoPermisos { get; set; } = new();

            [BindProperty(SupportsGet = true)]
            public UsuariosBuscarDto Filtro { get; set; } = new();

            public AdministracionPermisosModel(IConfiguration configuration)
            {
                string connectionString = configuration
                    .GetConnectionString("NorthwindConnection")!;

                usuarioBLL = new UsuarioBLL(connectionString);
                permisoBLL = new PermisoBLL(connectionString);
            }
            public void OnGet()
                {
                    CargarUsuarios();
                    CargarCatalogoPermisos();
                }

            private void CargarUsuarios()
            {
                Usuarios = usuarioBLL.Buscar(Filtro);
            }

            private void CargarCatalogoPermisos()
            {
                CatalogoPermisos = permisoBLL.ObtenerPermisosDeCatalogo();
            }

            public IActionResult OnGetSeleccionarUsuario(int id)
            {
                Usuario? usuario = usuarioBLL.ObtenerPorId(id);

                if (usuario == null)
                {
                    return new JsonResult(new
                    {
                        exito = false,
                        mensaje = "El usuario no existe o fue eliminado previamente."
                    });
                }

                // Permisos que actualmente tiene el usuario
                DataTable permisos = permisoBLL.ObtenerPermisosConcedidos(id);

                var permisosConcedidos = new List<object>();

                foreach (DataRow row in permisos.Rows)
                {
                    permisosConcedidos.Add(new
                    {
                        permisoId = Convert.ToInt32(row["PermisoId"]),
                        descripcion = row["Descripción"]?.ToString() ?? string.Empty
                    });
                }

                // Todos los permisos del catálogo
                DataTable catalogo = permisoBLL.ObtenerPermisosDeCatalogo();

                var catalogoPermisos = new List<object>();

                foreach (DataRow row in catalogo.Rows)
                {
                    catalogoPermisos.Add(new
                    {
                        permisoId = Convert.ToInt32(row["PermisoId"]),
                        descripcion = row["Descripción"]?.ToString() ?? string.Empty
                    });
                }

                return new JsonResult(new
                {
                    exito = true,

                    usuario = new
                    {
                        id = usuario.Id,
                        nombreUsuario = usuario.NombreUsuario,
                        nombre = $"{usuario.Nombres} {usuario.Paterno} {usuario.Materno}".Trim()
                    },

                    catalogoPermisos = catalogoPermisos,

                    permisos = permisosConcedidos
                });
            }

        public IActionResult OnPostConcederPermiso(int idUsuario, int permisoId)
        {
            try
            {
                permisoBLL.InsertarPermiso(idUsuario, permisoId);

                return new JsonResult(new
                {
                    exito = true,
                    mensaje = "Permiso concedido correctamente."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }

        public IActionResult OnPostQuitarPermiso(int idUsuario, int permisoId)
        {
            try
            {
                permisoBLL.EliminarPermiso(idUsuario, permisoId);

                return new JsonResult(new
                {
                    exito = true,
                    mensaje = "Permiso eliminado correctamente."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }

        public IActionResult OnGetPermisosConcedidos(int idUsuario)
        {
            try
            {
                DataTable permisos =
                    permisoBLL.ObtenerPermisosConcedidos(idUsuario);

                var permisosConcedidos = new List<object>();

                foreach (DataRow row in permisos.Rows)
                {
                    permisosConcedidos.Add(new
                    {
                        permisoId = Convert.ToInt32(row["PermisoId"]),
                        descripcion = row["Descripción"]?.ToString() ?? string.Empty
                    });
                }

                return new JsonResult(new
                {
                    exito = true,
                    permisos = permisosConcedidos
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }

        public IActionResult OnPostConcederTodosPermisos(int idUsuario)
        {
            try
            {
                DataTable catalogo = permisoBLL.ObtenerPermisosDeCatalogo();

                DataTable concedidos = permisoBLL.ObtenerPermisosConcedidos(idUsuario);

                var permisosExistentes = new HashSet<int>();

                foreach (DataRow row in concedidos.Rows)
                {
                    permisosExistentes.Add(
                        Convert.ToInt32(row["PermisoId"])
                    );
                }

                var permisosAInsertar = new List<int>();

                foreach (DataRow row in catalogo.Rows)
                {
                    int permisoId = Convert.ToInt32(row["PermisoId"]);

                    if (!permisosExistentes.Contains(permisoId))
                    {
                        permisosAInsertar.Add(permisoId);
                    }
                }

                if (permisosAInsertar.Count > 0)
                {
                    permisoBLL.InsertarPermisos(
                        idUsuario,
                        permisosAInsertar
                    );
                }

                return new JsonResult(new
                {
                    exito = true,
                    mensaje = permisosAInsertar.Count > 0
                        ? "Se concedieron todos los permisos correctamente."
                        : "El usuario ya tiene todos los permisos concedidos."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }

        public IActionResult OnPostQuitarTodosPermisos(int idUsuario)
        {
            try
            {
                int filasAfectadas =
                    permisoBLL.EliminarPermisos(idUsuario);

                return new JsonResult(new
                {
                    exito = true,
                    filasAfectadas = filasAfectadas,
                    mensaje = filasAfectadas > 0
                        ? $"Se eliminaron {filasAfectadas} permisos concedidos."
                        : "No se encontraron permisos concedidos para eliminar."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }

    }
}