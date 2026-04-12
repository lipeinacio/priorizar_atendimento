using Npgsql;
using PriorizarAtendimento.Api.Models;

namespace PriorizarAtendimento.Api.Services;

public class RepositorioDecisao
{
    private readonly DecisaoResposta _decisaoResposta;
    private readonly LeitorLegado _leitorLegado;
    private readonly DatabaseConnection _databaseConnection;

    public RepositorioDecisao(
        DecisaoResposta decisaoResposta,
        LeitorLegado leitorLegado,
        DatabaseConnection databaseConnection)
    {
        _decisaoResposta = decisaoResposta;
        _leitorLegado = leitorLegado;
        _databaseConnection = databaseConnection;
    }

    public async Task<ResultadoConsultaDecisao> ConsultarAsync(ConsultaDecisao request)
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

        try
        {
            await using var conexao = _databaseConnection.CriarConexao();
            await conexao.OpenAsync();

            const string sql = """
                select cliente_id,
                       nome_cliente,
                       classificacao,
                       prioridade,
                       acao_recomendada,
                       motivo
                from fn_resumo_cliente(
                    @p_cliente_id,
                    @p_nome_cliente,
                    @p_dias_atraso,
                    @p_mensagem_enviada,
                    @p_entrega_confirmada,
                    @p_interagiu,
                    @p_boleto_gerado,
                    @p_contato_atendido,
                    @p_cliente_fidelizado,
                    @p_linha_instavel,
                    @p_leitura_confirmada,
                    @p_acao_legado
                );
                """;

            await using var comando = new NpgsqlCommand(sql, conexao);
            comando.Parameters.AddWithValue("p_cliente_id", request.ClienteId);
            comando.Parameters.AddWithValue("p_nome_cliente", request.NomeCliente);
            comando.Parameters.AddWithValue("p_dias_atraso", request.DiasAtraso);
            comando.Parameters.AddWithValue("p_mensagem_enviada", request.MensagemEnviada);
            comando.Parameters.AddWithValue("p_entrega_confirmada", request.EntregaConfirmada);
            comando.Parameters.AddWithValue("p_interagiu", request.Interagiu);
            comando.Parameters.AddWithValue("p_boleto_gerado", request.BoletoGerado);
            comando.Parameters.AddWithValue("p_contato_atendido", request.ContatoAtendido);
            comando.Parameters.AddWithValue("p_cliente_fidelizado", request.ClienteFidelizado);
            comando.Parameters.AddWithValue("p_linha_instavel", request.LinhaInstavel);
            comando.Parameters.AddWithValue("p_leitura_confirmada", request.LeituraConfirmada);
            comando.Parameters.AddWithValue("p_acao_legado", retornoLegado.AcaoLegado);

            await using var reader = await comando.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ResultadoConsultaDecisao
                {
                    ClienteId = reader.GetInt32(0),
                    NomeCliente = reader.GetString(1),
                    Classificacao = reader.GetString(2),
                    Prioridade = reader.GetString(3),
                    AcaoRecomendada = reader.GetString(4),
                    Motivo = reader.GetString(5),
                    AcaoLegado = retornoLegado.AcaoLegado
                };
            }
        }
        catch
        {
            // Fallback temporario enquanto a integracao com banco ainda esta estabilizando.
        }

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