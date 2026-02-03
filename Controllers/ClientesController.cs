using DemoBackend.Data;
using DemoBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/clientes
        // Puedes probarlo directamente desde el navegador
        [HttpGet]
        public IActionResult GetClientes()
        {
            var clientes = _context.Clientes.ToList();
            return Ok(clientes);
        }

        // ✅ GET: api/clientes/5
        [HttpGet("{id}")]
        public IActionResult GetClienteById(int id)
        {
            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
                return NotFound($"No existe cliente con id {id}");

            return Ok(cliente);
        }

        // ✅ POST: api/clientes
        [HttpPost]
        public IActionResult CreateCliente([FromBody] Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetClienteById),
                new { id = cliente.Id },
                cliente
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Cliente cliente)
        {
            if (id != cliente.Id)
                return BadRequest();

            _context.Entry(cliente).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            cliente.Status = 99;
            await _context.SaveChangesAsync();

            return NoContent();
}




    }
}
