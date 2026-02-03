using ClosedXML.Excel;
using DemoBackend.Data;
using DemoBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // =========================
        // GET: api/clientes
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _context.Clientes
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return Ok(clientes);
        }

        // =========================
        // GET: api/clientes/5
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound($"No existe cliente con id {id}");

            return Ok(cliente);
        }

        // =========================
        // POST: api/clientes
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromBody] Cliente cliente)
        {
            cliente.Status = 1; // Activo por defecto

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetClienteById),
                new { id = cliente.Id },
                cliente
            );
        }

        // =========================
        // PUT: api/clientes/5
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] Cliente cliente)
        {
            if (id != cliente.Id)
                return BadRequest("Id no coincide");

            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // DELETE LÓGICO: api/clientes/5
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            cliente.Status = 99; // Eliminado lógico
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // EXPORTAR EXCEL
        // GET: api/clientes/export
        // =========================
        [HttpGet("export")]
        public async Task<IActionResult> ExportarExcel()
        {
            var clientes = await _context.Clientes
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Clientes");

            // Encabezados
            worksheet.Cell(1, 1).Value = "Nombre";
            worksheet.Cell(1, 2).Value = "Email";
            worksheet.Cell(1, 3).Value = "Teléfono";
            worksheet.Cell(1, 4).Value = "Status";

            var header = worksheet.Range("A1:D1");
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Data
            int row = 2;
            foreach (var c in clientes)
            {
                worksheet.Cell(row, 1).Value = c.Nombre;
                worksheet.Cell(row, 2).Value = c.Email;
                worksheet.Cell(row, 3).Value = c.Telefono;
                worksheet.Cell(row, 4).Value = c.Status == 99 ? "Eliminado" : "Activo";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Clientes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
    }
}
