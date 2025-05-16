namespace system_cosasapup.Models
{
    public class pegues
    {
        public int id { get; set; }
        public string comunidad { get; set; }
        public string direccion { get; set; }
        public string dueño { get; set; }
        public string codigo { get; set; }
        public bool estado { get; set; }
        public int idPago { get; set; }
        public pagos pagos { get; set; }

    }
}
