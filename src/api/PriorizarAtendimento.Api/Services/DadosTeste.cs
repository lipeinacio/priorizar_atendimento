using PriorizarAtendimento.Api.Models;

namespace PriorizarAtendimento.Api.Services;

public class DadosTeste
{
    public List<ClienteSituacaoTeste> ObterClientes()
    {
        return new List<ClienteSituacaoTeste>
        {
            new()
            {
                ClienteId = 1001,
                NomeCliente = "Carlos Doidao",
                DiasAtraso = 45,
                MensagemEnviada = true,
                EntregaConfirmada = true,
                LeituraConfirmada = true,
                Interagiu = false,
                BoletoGerado = false,
                ContatoAtendido = false,
                ClienteFidelizado = false,
                LinhaInstavel = false
            },
            new()
            {
                ClienteId = 102,
                NomeCliente = "Mariana Maluca",
                DiasAtraso = 18,
                MensagemEnviada = true,
                EntregaConfirmada = true,
                LeituraConfirmada = false,
                Interagiu = true,
                BoletoGerado = false,
                ContatoAtendido = true,
                ClienteFidelizado = true,
                LinhaInstavel = false
            },
            new()
            {
                ClienteId = 1003,
                NomeCliente = "Rafael Zoeiro",
                DiasAtraso = 62,
                MensagemEnviada = true,
                EntregaConfirmada = false,
                LeituraConfirmada = false,
                Interagiu = false,
                BoletoGerado = false,
                ContatoAtendido = false,
                ClienteFidelizado = false,
                LinhaInstavel = true
            }
        };
    }
}
