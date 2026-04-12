namespace PriorizarAtendimento.Api.Models;

public class Status
{
    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string StatusAtual { get; set; } = string.Empty;
    public string AcaoRecomendada { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
}