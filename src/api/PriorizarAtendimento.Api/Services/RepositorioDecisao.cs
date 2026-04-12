using PriorizarAtendimento.Api.Models;

namespace PriorizarAtendimento.Api.Services;

public class RepositorioDecisao
{
    private readonly DecisaoResposta _decisaoResposta;
    private readonly LeitorLegado _leitorLegado;

    public RepositorioDecisao(DecisaoResposta decisaoResposta, LeitorLegado leitorLegado)
    {
        _decisaoResposta = decisaoResposta;
        _leitorLegado = leitorLegado;
    }

    public ResultadoConsultaDecisao Consultar(ConsultaDecisao request)
    {
        var snapshot = new ClienteSituacaoTeste
        {
            ClienteId = request.ClienteId,
            NomeCliente = request.NomeCliente,
            DiasAtraso = request.DiasAtraso,
            MensagemEnviada = request.MensagemEnviada,
            EntregaConfirmada = request.EntregaConfirmada,
            LeituraConfirmada = request.LeituraConfirmada,
            Interagiu = request.Interagiu,
            BoletoGerado = request.BoletoGerado,
            ContatoAtendido = request.ContatoAtendido,
            ClienteFidelizado = request.ClienteFidelizado,
            LinhaInstavel = request.LinhaInstavel
        };

        var retornoLegado = _leitorLegado.Avaliar(snapshot);
        var classificacao = _decisaoResposta.ClassificarCliente(snapshot);
        var decisao = _decisaoResposta.Avaliar(snapshot);

        return new ResultadoConsultaDecisao
        {
            ClienteId = snapshot.ClienteId,
            NomeCliente = snapshot.NomeCliente,
            Classificacao = classificacao,
            Prioridade = decisao.Prioridade,
            AcaoRecomendada = decisao.AcaoRecomendada,
            Motivo = decisao.Motivo,
            AcaoLegado = retornoLegado.AcaoLegado
        };
    }
}
