CREATE DATABASE IF NOT EXISTS productos_db;
USE productos_db;

CREATE TABLE IF NOT EXISTS producto (
    id         INT AUTO_INCREMENT PRIMARY KEY,  -- llave propia de la tabla
    nombre     VARCHAR(100)  NOT NULL,
    categoria  VARCHAR(50)   NOT NULL,          -- ej. 'Electronica'
    precio     DECIMAL(10,2) NOT NULL,          -- en pesos
    stock      INT           NOT NULL DEFAULT 0,
    imagen     VARCHAR(255)  NULL               -- NULL = producto sin imagen
);

-- Datos de prueba
INSERT INTO producto (nombre, categoria, precio, stock) VALUES
    ('Laptop Lenovo',   'Electronica', 15999.00, 5),
    ('Mouse Logitech',  'Electronica',   349.50, 20),
    ('Cuaderno Scribe', 'Papeleria',      45.00, 100),
    ('Botella de agua', 'Hogar',         120.00, 30);
