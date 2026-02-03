using System.ComponentModel.DataAnnotations;

namespace DemoBackend.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        public int Status { get; set; } = 1; // 1 = Activo, 99 = Eliminado
    }
}
