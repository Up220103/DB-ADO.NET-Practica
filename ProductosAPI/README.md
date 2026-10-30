# ProductosAPI — CRUD de productos con ADO.NET + MySQL

Backend en .NET 10 (Minimal API) que administra productos guardados en MySQL
usando **ADO.NET puro** (`MySqlConnection`, `MySqlCommand`, `MySqlDataReader`).
Se prueba desde **Swagger**.

## Estructura

```
ProductosAPI/
├── Program.cs                         -> arranque: registra servicios, Swagger, CORS y rutas
├── Data/ConexionDb.cs                 -> CONEXION: lee la cadena y abre la conexion a MySQL
├── Models/Producto.cs                 -> la entidad (id, nombre, categoria, precio, stock, imagen)
├── Repositories/IProductoRepository.cs-> la interface (el contrato)
├── Repositories/ProductoRepository.cs -> CONSULTAS, INSERT, ACTUALIZAR, ELIMINAR, BUSCAR con SQL
├── Endpoints/ProductoEndpoints.cs     -> las rutas HTTP (GET, POST, PUT, DELETE)
└── Database/productos.sql             -> script para crear la base y la tabla
```

## Como correrlo

1. Ejecutar `Database/productos.sql` en MySQL Workbench.
2. Guardar la cadena de conexion (no se sube a Git):
   ```
   dotnet user-secrets set "ConnectionStrings:ProductosDb" "Server=localhost;Port=3306;Database=productos_db;User=root;Password=TU_PASSWORD;"
   ```
3. `dotnet run` y abrir http://localhost:5060/swagger

## Rutas

| Metodo | Ruta                          | Metodo del repositorio | Respuesta            |
|--------|-------------------------------|------------------------|----------------------|
| GET    | `/productos`                  | `ObtenerTodos()`       | 200 + lista          |
| GET    | `/productos/{id}`             | `ObtenerPorId(id)`     | 200 / 404            |
| GET    | `/productos/buscar?texto=lap` | `Buscar(texto)`        | 200 + lista          |
| POST   | `/productos`                  | `Insertar(producto)`   | 201 + producto con id|
| PUT    | `/productos/{id}`             | `Actualizar(producto)` | 204 / 404            |
| DELETE | `/productos/{id}`             | `Eliminar(id)`         | 204 / 404            |

## Flujo explicado con mis palabras

Cuando alguien hace una peticion (por ejemplo desde Swagger), el camino siempre es el mismo:

**Swagger → Endpoint → Repositorio → ConexionDb → MySQL**, y de regreso la respuesta en JSON.

1. **Program.cs** es lo primero que corre. Ahi se "registran" las piezas:
   `ConexionDb` como Singleton (una sola para toda la app, solo guarda la cadena) y
   `IProductoRepository` → `ProductoRepository` como Scoped (uno nuevo por peticion).
   Asi, cuando una ruta pide un `IProductoRepository`, .NET le entrega el objeto ya armado
   (inyeccion de dependencias). Tambien se activa Swagger y se llama `MapProductoEndpoints()`.

2. **ProductoEndpoints** define las rutas. Cada ruta solo recibe los datos (id en la URL o
   el producto en el body como JSON), llama al metodo del repositorio y decide que codigo
   HTTP devolver: si el metodo regreso `null` o `false` es un 404; si salio bien, 200/201/204.

3. **ProductoRepository** es donde esta el ADO.NET. Todos los metodos siguen los mismos pasos:
   - `_db.AbrirConexion()` abre la conexion a MySQL.
   - `new MySqlCommand("SQL...", conexion)` prepara la sentencia.
   - `Parameters.AddWithValue("@campo", valor)` mete los valores como parametros
     (nunca se pegan en el texto del SQL, asi se evita SQL injection).
   - Se ejecuta segun lo que se necesite:
     - `ExecuteReader()` para **SELECT** (consultas, buscar por id, buscar por texto):
       con `while (lector.Read())` se recorre fila por fila y cada fila se convierte
       en un `Producto` dentro de `LeerLista`.
     - `ExecuteNonQuery()` para **UPDATE** y **DELETE**: regresa cuantas filas cambio;
       si es 0 quiere decir que el id no existia → `false`.
     - `ExecuteScalar()` para el **INSERT**: despues del insert se hace
       `SELECT LAST_INSERT_ID()` para saber que id le dio MySQL al producto nuevo.
   - El `using` cierra la conexion solo al terminar el metodo.

4. **ConexionDb** solo se encarga de leer la cadena `ProductosDb` de user-secrets y abrir
   conexiones. Si falta la cadena avisa con un error claro.

Ejemplo completo — crear un producto: en Swagger se hace `POST /productos` con el JSON →
el endpoint recibe el `Producto` → llama `repo.Insertar()` → abre conexion, ejecuta el
INSERT con parametros y obtiene el id → el endpoint le pone ese id al producto y responde
**201 Created** con el producto completo.
