using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using NorthwindTradersV8WebRazorPages.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Empleados", "PermisoEmpleados");
    options.Conventions.AuthorizeFolder("/Clientes", "PermisoClientes");
    options.Conventions.AuthorizeFolder("/Proveedores", "PermisoProveedores");
    options.Conventions.AuthorizeFolder("/ClientesProveedoresComun", "PermisoClientesProveedores");
    options.Conventions.AuthorizeFolder("/Categorias", "PermisoCategorias");
    options.Conventions.AuthorizeFolder("/Productos", "PermisoProductos");
    options.Conventions.AuthorizeFolder("/ProveedoresProductosComun", "PermisoProveedoresProductos");
    options.Conventions.AuthorizeFolder("/Ventas", "PermisoVentas");
    options.Conventions.AuthorizeFolder("/Graficas", "PermisoGraficas");
    options.Conventions.AuthorizeFolder("/Administracion", "PermisoAdministracion");
    options.Conventions.AuthorizeFolder("/Tableros/AltaDireccion", "PermisoTableroAltaDireccion");
    options.Conventions.AuthorizeFolder("/Tableros/Vendedores", "PermisoTableroVendedores");
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.Cookie.Name = "NorthwindTraders.Auth";

        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PermisoEmpleados", policy =>
        policy.RequireClaim("Permiso", Permisos.Empleados.ToString()));

    options.AddPolicy("PermisoClientes", policy =>
        policy.RequireClaim("Permiso", Permisos.Clientes.ToString()));

    options.AddPolicy("PermisoProveedores", policy =>
        policy.RequireClaim("Permiso", Permisos.Proveedores.ToString()));

    options.AddPolicy("PermisoClientesProveedores", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim("Permiso", Permisos.Clientes.ToString()) ||
            context.User.HasClaim("Permiso", Permisos.Proveedores.ToString()));
    });

    options.AddPolicy("PermisoCategorias", policy =>
        policy.RequireClaim("Permiso", Permisos.Categorias.ToString()));

    options.AddPolicy("PermisoProductos", policy =>
        policy.RequireClaim("Permiso", Permisos.Productos.ToString()));

    options.AddPolicy("PermisoProveedoresProductos", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim("Permiso", Permisos.Proveedores.ToString()) ||
            context.User.HasClaim("Permiso", Permisos.Productos.ToString()));
    });

    options.AddPolicy("PermisoVentas", policy =>
        policy.RequireClaim("Permiso", Permisos.Ventas.ToString()));

    options.AddPolicy("PermisoGraficas", policy =>
        policy.RequireClaim("Permiso", Permisos.Graficas.ToString()));

    options.AddPolicy("PermisoAdministracion", policy =>
        policy.RequireClaim("Permiso", Permisos.Administracion.ToString()));

    options.AddPolicy("PermisoTableroAltaDireccion", policy =>
        policy.RequireClaim("Permiso", Permisos.TableroAltaDireccion.ToString()));

    options.AddPolicy("PermisoTableroVendedores", policy =>
        policy.RequireClaim("Permiso", Permisos.TableroVendedores.ToString()));

    // Toda página requiere autenticación por defecto.
    options.FallbackPolicy =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();