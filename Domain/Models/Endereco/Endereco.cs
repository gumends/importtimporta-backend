namespace Domain.Models.Endereco
{
    public class Endereco
    {
        public Guid Id { get; set; }
        public int Cep { get; set; }
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; } = string.Empty;
        public Guid IdUsuario { get; set; }
    }
}
