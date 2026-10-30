using MySql.Data.MySqlClient;
using MYBACK.Data;
using MYBACK.Models;
namespace MYBACK.Repositories;

// CRUD en la tabla "producto" con ADO.NET.
// Cada metodo: abre conexion -> prepara comando SQL -> agrega parametros -> ejecuta.
// "using" cierra la conexion sola al terminar.
public class ProductoRepository : IProductoRepository
{
    private readonly ConexionDb _db;

    public ProductoRepository(ConexionDb db)
    {
        _db = db;
    }

    public List<Producto> ObtenerTodos()
    {
        using var conexion = _db.AbrirConexion();
        var comando = new MySqlCommand("SELECT * FROM producto ORDER BY id", conexion);
        return LeerLista(comando);
    }

    public Producto? ObtenerPorId(int id)
    {
        using var conexion = _db.AbrirConexion();
        var comando = new MySqlCommand("SELECT * FROM producto WHERE id = @id", conexion);
        comando.Parameters.AddWithValue("@id", id);
        return LeerLista(comando).FirstOrDefault(); 
    }

    public List<Producto> Buscar(string texto)
    {
        using var conexion = _db.AbrirConexion();
        var comando = new MySqlCommand(
            @"SELECT * FROM producto
              WHERE nombre LIKE @texto OR categoria LIKE @texto
              ORDER BY id", conexion);
        comando.Parameters.AddWithValue("@texto", $"%{texto}%");
        return LeerLista(comando);
    }

    public int Insertar(Producto producto)
    {
        using var conexion = _db.AbrirConexion();
        var comando = new MySqlCommand(
            @"INSERT INTO producto (nombre, categoria, precio, stock, imagen)
              VALUES (@nombre, @categoria, @precio, @stock, @imagen);
              SELECT LAST_INSERT_ID();", conexion);
        AgregarParametros(comando, producto);
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public bool Actualizar(Producto producto)
    {
        using var conexion = _db.AbrirConexion();
        var comando = new MySqlCommand(
            @"UPDATE producto SET nombre = @nombre, categoria = @categoria,
                     precio = @precio, stock = @stock, imagen = @imagen
              WHERE id = @id", conexion);
        AgregarParametros(comando, producto);
        comando.Parameters.AddWithValue("@id", producto.Id);
        return comando.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var conexion = _db.AbrirConexion();
        var comando = new MySqlCommand("DELETE FROM producto WHERE id = @id", conexion);
        comando.Parameters.AddWithValue("@id", id);
        return comando.ExecuteNonQuery() > 0;
    }

    private static List<Producto> LeerLista(MySqlCommand comando)
    {
        var lista = new List<Producto>();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Producto
            {
                Id        = lector.GetInt32("id"),
                Nombre    = lector.GetString("nombre"),
                Categoria = lector.GetString("categoria"),
                Precio    = lector.GetDecimal("precio"),
                Stock     = lector.GetInt32("stock"),
                Imagen    = lector["imagen"] as string, 
            });
        }
        return lista;
    }

    private static void AgregarParametros(MySqlCommand comando, Producto p)
    {
        comando.Parameters.AddWithValue("@nombre", p.Nombre);
        comando.Parameters.AddWithValue("@categoria", p.Categoria);
        comando.Parameters.AddWithValue("@precio", p.Precio);
        comando.Parameters.AddWithValue("@stock", p.Stock);
        comando.Parameters.AddWithValue("@imagen", p.Imagen);
    }
}
