using PriorizarAtendimento.Api.Models;

namespace PriorizarAtendimento.Api.Services;

public class DecisaoResposta
{
    private readonly LeitorLegado _leitorLegado;

    public DecisaoResposta(LeitorLegado leitorLegado)
    {
        _leitorLegado = leitorLegado;
    }

    public string ClassificarCliente(ClienteSituacaoTeste snapshot)
    {
        var retornoLegado = _leitorLegado.Avaliar(snapshot);

        if (snapshot.LinhaInstavel || !snapshot.EntregaConfirmada || retornoLegado.AcaoLegado == "REVISAR_LINHA_TEMPLATE")
        {
            return "REVISAO_OPERACIONAL";
        }

        if (snapshot.ContatoAtendido || snapshot.Interagiu || retornoLegado.AcaoLegado == "TRATAR_HUMANO")
        {
            return "TRATATIVA_HUMANA";
        }

        if (snapshot.BoletoGerado || retornoLegado.AcaoLegado == "PAUSAR_48H")
        {
            return "ACOMPANHAMENTO";
        }

        if (retornoLegado.AcaoLegado == "COBRANCA_LEVE")
        {
            return "ACIONAMENTO_MODERADO";
        }

        if (snapshot.DiasAtraso >= 30 && snapshot.MensagemEnviada || retornoLegado.AcaoLegado == "COBRANCA_FORTE")
        {
            return snapshot.ClienteFidelizado ? "ACIONAMENTO_MODERADO" : "ACIONAMENTO_FORTE";
        }

        return "MONITORAMENTO";
    }

    public Decisao Avaliar(ClienteSituacaoTeste snapshot)
    {
        var retornoLegado = _leitorLegado.Avaliar(snapshot);

        if (snapshot.LinhaInstavel || !snapshot.EntregaConfirmada || retornoLegado.AcaoLegado == "REVISAR_LINHA_TEMPLATE")
        {
            return new Decisao
            {
                AcaoRecomendada = "REVISAR_OPERACAO",
                Prioridade = "ALTA",
                Motivo = "Mensagem sem entrega confirmada, linha instavel ou retorno do legado indicando revisao de linha."
            };
        }

        if (snapshot.ContatoAtendido || snapshot.Interagiu || retornoLegado.AcaoLegado == "TRATAR_HUMANO")
        {
            return new Decisao
            {
                AcaoRecomendada = "ENCAMINHAR_ATENDIMENTO",
                Prioridade = "MEDIA",
                Motivo = "Cliente com sinal de interacao humana ou retorno herdado para tratativa manual."
            };
        }

        if (snapshot.BoletoGerado || retornoLegado.AcaoLegado == "PAUSAR_48H")
        {
            return new Decisao
            {
                AcaoRecomendada = "PAUSAR_COBRANCA",
                Prioridade = "MEDIA",
                Motivo = "Ja existe indicio de conversao recente ou o legado mandou pausar temporariamente."
            };
        }

        if (retornoLegado.AcaoLegado == "COBRANCA_LEVE")
        {
            return new Decisao
            {
                AcaoRecomendada = "CONTATO_MODERADO",
                Prioridade = "BAIXA",
                Motivo = "Modulo legado VB6 classificou o caso como cobranca leve."
            };
        }

        if ((snapshot.DiasAtraso >= 30 && snapshot.MensagemEnviada && snapshot.LeituraConfirmada) || retornoLegado.AcaoLegado == "COBRANCA_FORTE")
        {
            return new Decisao
            {
                AcaoRecomendada = "CONTINUAR_COBRANCA",
                Prioridade = snapshot.ClienteFidelizado ? "MEDIA" : "ALTA",
                Motivo = "Cliente elegivel para nova tentativa automatizada ou retorno do legado indicando cobranca forte."
            };
        }

        return new Decisao
        {
            AcaoRecomendada = "MONITORAR",
            Prioridade = "BAIXA",
            Motivo = "Caso sem criterio suficiente para acao imediata."
        };
    }

    public int ObterPesoPrioridade(string prioridade)
    {
        return prioridade switch
        {
            "ALTA" => 3,
            "MEDIA" => 2,
            "BAIXA" => 1,
            _ => 0
        };
    }
}