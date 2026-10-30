using MySql.Data.MySqlClient;

namespace MYBACK.Data;

public class ConexionDb
{
    private readonly string _cadena;

    public ConexionDb(IConfiguration configuracion)
    {
        _cadena = configuracion.GetConnectionString("ProductosDb")
            ?? throw new InvalidOperationException("Falta 'ProductosDb' en user-secrets");
    }

    // Crea y abre una conexion nueva
    public MySqlConnection AbrirConexion()
    {
        var conexion = new MySqlConnection(_cadena);
        conexion.Open();
        return conexion;
    }
}
