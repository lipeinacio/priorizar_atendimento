namespace PriorizarAtendimento.Api.Models;

public class TesteConexaoBanco
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public int Resultado { get; set; }
}