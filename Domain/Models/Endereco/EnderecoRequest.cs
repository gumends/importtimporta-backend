namespace Domain.Models.Endereco
{
    public class EnderecoRequest
    {
        public int Cep { get; set; }
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
    }
}
