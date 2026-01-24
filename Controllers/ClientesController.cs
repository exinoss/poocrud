using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using poocrud.Data;
using poocrud.Models;

namespace poocrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(ApplicationDbContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clientes>>> GetClientes()
        {
            try
            {
                var clientes = await _context.Clientes.ToListAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los clientes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un cliente por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Clientes>> GetCliente(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    return NotFound($"Cliente con ID {id} no encontrado");
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cliente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Clientes>> CreateCliente([FromBody] Clientes cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si ya existe un cliente con el mismo documento
                var existeCliente = await _context.Clientes
                    .AnyAsync(c => c.Documento == cliente.Documento);

                if (existeCliente)
                {
                    return Conflict($"Ya existe un cliente con el documento {cliente.Documento}");
                }

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el cliente");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un cliente existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] Clientes cliente)
        {
            try
            {
                if (id != cliente.Id)
                {
                    return BadRequest("El ID del cliente no coincide");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var clienteExistente = await _context.Clientes.FindAsync(id);

                if (clienteExistente == null)
                {
                    return NotFound($"Cliente con ID {id} no encontrado");
                }

                // Verificar si el documento ya existe en otro cliente
                var documentoDuplicado = await _context.Clientes
                    .AnyAsync(c => c.Documento == cliente.Documento && c.Id != id);

                if (documentoDuplicado)
                {
                    return Conflict($"Ya existe otro cliente con el documento {cliente.Documento}");
                }

                // Actualizar propiedades
                clienteExistente.Nombres = cliente.Nombres;
                clienteExistente.TipoDocumento = cliente.TipoDocumento;
                clienteExistente.Documento = cliente.Documento;
                clienteExistente.Direccion = cliente.Direccion;
                clienteExistente.Ciudad = cliente.Ciudad;
                clienteExistente.Email = cliente.Email;
                clienteExistente.Telf = cliente.Telf;
                clienteExistente.FechaNacimiento = cliente.FechaNacimiento;
                clienteExistente.Estado = cliente.Estado;

                await _context.SaveChangesAsync();

                return Ok(clienteExistente);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
                {
                    return NotFound($"Cliente con ID {id} no encontrado");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el cliente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un cliente
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    return NotFound($"Cliente con ID {id} no encontrado");
                }

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Cliente con ID {id} eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el cliente con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Busca clientes por nombre o documento
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Clientes>>> BuscarClientes([FromQuery] string? termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino))
                {
                    return await GetClientes();
                }

                var clientes = await _context.Clientes
                    .Where(c => c.Nombres.Contains(termino) || c.Documento.Contains(termino))
                    .ToListAsync();

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar clientes con término: {Termino}", termino);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
