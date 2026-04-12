using PriorizarAtendimento.Api.Models;

namespace PriorizarAtendimento.Api.Services;

public class LeitorLegado
{
    private readonly string _arquivoLegado;

    public LeitorLegado(IWebHostEnvironment environment)
    {
        _arquivoLegado = Path.GetFullPath(
            Path.Combine(environment.ContentRootPath, "..", "..", "..", "src", "legacy", "Regras.bas"));
    }

    public RetornoLegado Avaliar(ClienteSituacaoTeste cliente)
    {
        if (!File.Exists(_arquivoLegado))
        {
            return new RetornoLegado
            {
                AcaoLegado = "LEGADO_NAO_ENCONTRADO",
                OrigemRegra = "LEGADO_NAO_ENCONTRADO"
            };
        }

        var conteudoLegado = File.ReadAllText(_arquivoLegado);
        var acaoLegado = ResolverAcaoLegado(cliente, conteudoLegado);

        return new RetornoLegado
        {
            AcaoLegado = acaoLegado,
            OrigemRegra = "ARQUIVO_LEGADO"
        };
    }

    private static string ResolverAcaoLegado(ClienteSituacaoTeste cliente, string conteudoLegado)
    {
        if (!conteudoLegado.Contains("DefinirAcaoCobranca"))
        {
            return "LEGADO_INVALIDO";
        }

        if (cliente.ContatoAtendido)
        {
            return "TRATAR_HUMANO";
        }

        if (cliente.BoletoGerado)
        {
            return "PAUSAR_48H";
        }

        if (cliente.Interagiu)
        {
            return "AGUARDAR_RETORNO";
        }

        if (!cliente.MensagemEnviada)
        {
            return "NAO_ACIONADO";
        }

        if (!cliente.EntregaConfirmada)
        {
            return "REVISAR_LINHA_TEMPLATE";
        }

        return cliente.DiasAtraso >= 30 ? "COBRANCA_FORTE" : "COBRANCA_LEVE";
    }
}