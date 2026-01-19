# API de Clientes

Base URL: `https://localhost:{puerto}/api/clientes`

## Endpoints

| Metodo | URL                                    | Descripcion                |
| ------ | -------------------------------------- | -------------------------- |
| GET    | `/api/clientes`                        | Obtener todos los clientes |
| GET    | `/api/clientes/{id}`                   | Obtener cliente por ID     |
| GET    | `/api/clientes/buscar?termino={texto}` | Buscar clientes            |
| POST   | `/api/clientes`                        | Crear cliente              |
| PUT    | `/api/clientes/{id}`                   | Actualizar cliente         |
| DELETE | `/api/clientes/{id}`                   | Eliminar cliente           |

## Body POST

```json
{
  "nombres": "Juan Perez",
  "cedula": "1234567890",
  "direccion": "Av. Principal 123",
  "email": "juan@email.com",
  "telf": "0991234567"
}
```

## Body PUT

```json
{
  "id": 1,
  "nombres": "Juan Perez",
  "cedula": "1234567890",
  "direccion": "Av. Principal 123",
  "email": "juan@email.com",
  "telf": "0991234567"
}
```
