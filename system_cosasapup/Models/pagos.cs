namespace system_cosasapup.Models
{
    public class pagos
    {
        public int id { get; set; }
        public decimal monto { get; set; }
        public DateTime fechaPago { get; set; }
        public string metodoPago { get; set; }
        public string observaciones { get; set; }

        // Relación con la entidad pegues muchos  a uno
        public int PegueId { get; set; }
        public pegues Pegue { get; set; }
    }
}
