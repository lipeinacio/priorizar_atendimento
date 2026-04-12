namespace PriorizarAtendimento.Api.Models;

public class CleinteList
{
    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public int DiasAtraso { get; set; }
    public string StatusAtual { get; set; } = string.Empty;
}