using PriorizarAtendimento.Api.Models;

namespace PriorizarAtendimento.Api.Services;

public class DadosTeste
{
    private readonly string _arquivoBaseClientes;

    public DadosTeste(IWebHostEnvironment environment)
    {
        _arquivoBaseClientes = Path.GetFullPath(
            Path.Combine(environment.ContentRootPath, "..", "..", "..", "samples", "input", "base_clientes_cobranca.csv"));
    }

    public List<ClienteSituacaoTeste> ObterClientes()
    {
        if (!File.Exists(_arquivoBaseClientes))
        {
            throw new FileNotFoundException("Arquivo base de clientes nao encontrado.", _arquivoBaseClientes);
        }

        var linhas = File.ReadAllLines(_arquivoBaseClientes)
            .Skip(1)
            .Where(linha => !string.IsNullOrWhiteSpace(linha));

        return linhas.Select(MapearLinha).ToList();
    }

    private static ClienteSituacaoTeste MapearLinha(string linha)
    {
        var colunas = linha.Split(',');

        return new ClienteSituacaoTeste
        {
            ClienteId = int.Parse(colunas[0]),
            NomeCliente = colunas[1],
            DiasAtraso = int.Parse(colunas[2]),
            MensagemEnviada = bool.Parse(colunas[3]),
            EntregaConfirmada = bool.Parse(colunas[4]),
            LeituraConfirmada = bool.Parse(colunas[5]),
            Interagiu = bool.Parse(colunas[6]),
            BoletoGerado = bool.Parse(colunas[7]),
            ContatoAtendido = bool.Parse(colunas[8]),
            ClienteFidelizado = bool.Parse(colunas[9]),
            LinhaInstavel = bool.Parse(colunas[10])
        };
    }
}



