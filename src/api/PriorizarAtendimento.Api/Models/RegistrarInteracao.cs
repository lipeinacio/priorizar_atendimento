namespace PriorizarAtendimento.Api.Models;

public class RegistrarInteracao
{
    public int ClienteId { get; set; }
    public string Canal { get; set; } = string.Empty;
    public string TipoInteracao { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
}