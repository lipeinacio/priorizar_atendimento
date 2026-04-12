namespace PriorizarAtendimento.Api.Models;

public class ClienteSituacaoTeste
{
    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public int DiasAtraso { get; set; }
    public bool MensagemEnviada { get; set; }
    public bool EntregaConfirmada { get; set; }
    public bool LeituraConfirmada { get; set; }
    public bool Interagiu { get; set; }
    public bool BoletoGerado { get; set; }
    public bool ContatoAtendido { get; set; }
    public bool ClienteFidelizado { get; set; }
    public bool LinhaInstavel { get; set; }
}