using System.ComponentModel.DataAnnotations;

namespace system_cosasapup.Models;
public class Usuario
{
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    public string Correo { get; set; }

    [Required]
    public string Clave { get; set; }
}
