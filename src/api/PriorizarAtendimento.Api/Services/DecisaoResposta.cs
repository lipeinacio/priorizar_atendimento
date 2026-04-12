using PriorizarAtendimento.Api.Models;

namespace PriorizarAtendimento.Api.Services;

public class DecisaoResposta
{
    public string ClassificarCliente(ClienteSituacaoTeste snapshot)
    {
        if (snapshot.LinhaInstavel || !snapshot.EntregaConfirmada)
        {
            return "REVISAO_OPERACIONAL";
        }

        if (snapshot.ContatoAtendido || snapshot.Interagiu)
        {
            return "TRATATIVA_HUMANA";
        }

        if (snapshot.BoletoGerado)
        {
            return "ACOMPANHAMENTO";
        }

        if (snapshot.DiasAtraso >= 30 && snapshot.MensagemEnviada)
        {
            return snapshot.ClienteFidelizado ? "ACIONAMENTO_MODERADO" : "ACIONAMENTO_FORTE";
        }

        return "MONITORAMENTO";
    }

    public Decisao Avaliar(ClienteSituacaoTeste snapshot)
    {
        if (snapshot.LinhaInstavel || !snapshot.EntregaConfirmada)
        {
            return new Decisao
            {
                AcaoRecomendada = "REVISAR_OPERACAO",
                Prioridade = "ALTA",
                Motivo = "Mensagem sem entrega confirmada ou linha instavel."
            };
        }

        if (snapshot.ContatoAtendido || snapshot.Interagiu)
        {
            return new Decisao
            {
                AcaoRecomendada = "ENCAMINHAR_ATENDIMENTO",
                Prioridade = "MEDIA",
                Motivo = "Cliente com sinal de interacao humana."
            };
        }

        if (snapshot.BoletoGerado)
        {
            return new Decisao
            {
                AcaoRecomendada = "PAUSAR_COBRANCA",
                Prioridade = "MEDIA",
                Motivo = "Ja existe indicio de conversao recente."
            };
        }

        if (snapshot.DiasAtraso >= 30 && snapshot.MensagemEnviada && snapshot.LeituraConfirmada)
        {
            return new Decisao
            {
                AcaoRecomendada = "CONTINUAR_COBRANCA",
                Prioridade = snapshot.ClienteFidelizado ? "MEDIA" : "ALTA",
                Motivo = "Cliente elegivel para nova tentativa automatizada."
            };
        }

        return new Decisao
        {
            AcaoRecomendada = "MONITORAR",
            Prioridade = "BAIXA",
            Motivo = "Caso sem criterio suficiente para acao imediata."
        };
    }
}
