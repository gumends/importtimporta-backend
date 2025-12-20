namespace Domain.Models.Endereco
{
    public class Endereco
    {
        public int Id { get; set; }
        public int Cep { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
    }
}
