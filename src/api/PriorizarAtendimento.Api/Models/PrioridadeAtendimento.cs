namespace PriorizarAtendimento.Api.Models;

public class PrioridadeAtendimento
{
    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string Classificacao { get; set; } = string.Empty;
    public string Prioridade { get; set; } = string.Empty;
    public string AcaoRecomendada { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
}