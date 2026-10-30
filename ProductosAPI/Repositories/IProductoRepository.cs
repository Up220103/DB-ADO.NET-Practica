using MYBACK.Models;
namespace MYBACK.Repositories;

// Contrato: CRUD de productos en MySQL
public interface IProductoRepository
{
    List<Producto> ObtenerTodos();
    Producto? ObtenerPorId(int id);          // null si no existe
    int Insertar(Producto producto);         // regresa el id nuevo
    bool Actualizar(Producto producto);      // false si no existe el id
    bool Eliminar(int id);                   // false si no existe el id
    List<Producto> Buscar(string texto);     // busca por nombre o categoria
}
