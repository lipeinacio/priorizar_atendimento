namespace PriorizarAtendimento.Api.Models;

public class Decisao
{
    public string AcaoRecomendada { get; set; } = string.Empty;
    public string Prioridade { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
}