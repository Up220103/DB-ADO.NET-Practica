using MYBACK.Models;
using MYBACK.Repositories;

namespace MYBACK.Endpoints;

// Rutas del CRUD de productos (apuntan a MySQL). 
public static class ProductoEndpoints
{
    public static void MapProductoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/productos");

        group.MapGet("/", (IProductoRepository repo) =>
            Results.Ok(repo.ObtenerTodos()));

        group.MapGet("/buscar", (string texto, IProductoRepository repo) =>
            Results.Ok(repo.Buscar(texto)));

        group.MapGet("/{id:int}", (int id, IProductoRepository repo) =>
            repo.ObtenerPorId(id) is Producto producto ? Results.Ok(producto) : Results.NotFound());

        group.MapPost("/", (Producto nuevo, IProductoRepository repo) =>
        {
            nuevo.Id = repo.Insertar(nuevo);
            return Results.Created($"/productos/{nuevo.Id}", nuevo);
        });

        group.MapPut("/{id:int}", (int id, Producto producto, IProductoRepository repo) =>
        {
            producto.Id = id;
            return repo.Actualizar(producto) ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", (int id, IProductoRepository repo) =>
            repo.Eliminar(id) ? Results.NoContent() : Results.NotFound());
    }
}
