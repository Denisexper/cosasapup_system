using System.ComponentModel.DataAnnotations;

namespace system_cosasapup.Models
{
    public class pagos
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal monto { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime fechaPago { get; set; }

        [Required(ErrorMessage = "Seleccione un método de pago")]
        public string metodoPago { get; set; }

        public string observaciones { get; set; }

        // Relación con pegues
        public int PegueId { get; set; }
        public pegues Pegue { get; set; }
    }
}