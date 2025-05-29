namespace system_cosasapup.Models
{
    public class pegues
    {
        public int PegueId { get; set; }
        public string comunidad { get; set; }
        public string direccion { get; set; }
        public string dueño { get; set; }
        public string codigo { get; set; }
        public bool estado { get; set; }
        //relacion de uno a muchos
        public List<pagos> pagos { get; set; }  

    }
}
