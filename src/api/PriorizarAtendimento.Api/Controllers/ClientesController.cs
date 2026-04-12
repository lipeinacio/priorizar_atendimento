using Microsoft.AspNetCore.Mvc;
using PriorizarAtendimento.Api.Models;
using PriorizarAtendimento.Api.Services;

namespace PriorizarAtendimento.Api.Controllers;

[ApiController]
[Route("api")]
public class ClientesController : ControllerBase
{
    private readonly RepositorioDecisao _decisaoRepository;
    private readonly DadosTeste _mockDataService;

    public ClientesController(
        RepositorioDecisao decisaoRepository,
        DadosTeste mockDataService)
    {
        _decisaoRepository = decisaoRepository;
        _mockDataService = mockDataService;
    }

    [HttpGet("clientes")]
    public ActionResult<IEnumerable<CleinteList>> ListarClientes()
    {
        var clientes = _mockDataService.ObterClientes()
            .Select(cliente =>
            {
                var resultado = _decisaoRepository.Consultar(new ConsultaDecisao
                {
                    ClienteId = cliente.ClienteId,
                    NomeCliente = cliente.NomeCliente,
                    DiasAtraso = cliente.DiasAtraso,
                    MensagemEnviada = cliente.MensagemEnviada,
                    EntregaConfirmada = cliente.EntregaConfirmada,
                    LeituraConfirmada = cliente.LeituraConfirmada,
                    Interagiu = cliente.Interagiu,
                    BoletoGerado = cliente.BoletoGerado,
                    ContatoAtendido = cliente.ContatoAtendido,
                    ClienteFidelizado = cliente.ClienteFidelizado,
                    LinhaInstavel = cliente.LinhaInstavel
                });

                return new CleinteList
                {
                    ClienteId = cliente.ClienteId,
                    NomeCliente = cliente.NomeCliente,
                    DiasAtraso = cliente.DiasAtraso,
                    StatusAtual = resultado.AcaoRecomendada
                };
            });

        return Ok(clientes);
    }

    [HttpGet("prioridade")]
    public ActionResult<IEnumerable<PrioridadeAtendimento>> ObterPrioridade()
    {
        var prioridades = _mockDataService.ObterClientes()
            .Select(cliente =>
            {
                var resultado = _decisaoRepository.Consultar(new ConsultaDecisao
                {
                    ClienteId = cliente.ClienteId,
                    NomeCliente = cliente.NomeCliente,
                    DiasAtraso = cliente.DiasAtraso,
                    MensagemEnviada = cliente.MensagemEnviada,
                    EntregaConfirmada = cliente.EntregaConfirmada,
                    LeituraConfirmada = cliente.LeituraConfirmada,
                    Interagiu = cliente.Interagiu,
                    BoletoGerado = cliente.BoletoGerado,
                    ContatoAtendido = cliente.ContatoAtendido,
                    ClienteFidelizado = cliente.ClienteFidelizado,
                    LinhaInstavel = cliente.LinhaInstavel
                });

                return new PrioridadeAtendimento
                {
                    ClienteId = cliente.ClienteId,
                    NomeCliente = cliente.NomeCliente,
                    Classificacao = resultado.Classificacao,
                    Prioridade = resultado.Prioridade,
                    AcaoRecomendada = resultado.AcaoRecomendada,
                    Motivo = resultado.Motivo
                };
            })
            .OrderByDescending(item => ObterPeso(item.Prioridade))
            .ThenByDescending(item => item.ClienteId)
            .ToList();

        return Ok(prioridades);
    }

    [HttpGet("status/{clienteId:int}")]
    public ActionResult<Status> ObterStatus(int clienteId)
    {
        var cliente = _mockDataService.ObterClientes().FirstOrDefault(item => item.ClienteId == clienteId);
        if (cliente is null)
        {
            return NotFound();
        }

        var resultado = _decisaoRepository.Consultar(new ConsultaDecisao
        {
            ClienteId = cliente.ClienteId,
            NomeCliente = cliente.NomeCliente,
            DiasAtraso = cliente.DiasAtraso,
            MensagemEnviada = cliente.MensagemEnviada,
            EntregaConfirmada = cliente.EntregaConfirmada,
            LeituraConfirmada = cliente.LeituraConfirmada,
            Interagiu = cliente.Interagiu,
            BoletoGerado = cliente.BoletoGerado,
            ContatoAtendido = cliente.ContatoAtendido,
            ClienteFidelizado = cliente.ClienteFidelizado,
            LinhaInstavel = cliente.LinhaInstavel
        });

        return Ok(new Status
        {
            ClienteId = cliente.ClienteId,
            NomeCliente = cliente.NomeCliente,
            StatusAtual = resultado.Classificacao,
            AcaoRecomendada = resultado.AcaoRecomendada,
            Motivo = resultado.Motivo
        });
    }

    [HttpPost("interacoes")]
    public ActionResult<InteracaoRegistrada> RegistrarInteracao([FromBody] RegistrarInteracao request)
    {
        var resposta = new InteracaoRegistrada
        {
            ClienteId = request.ClienteId,
            Canal = request.Canal,
            TipoInteracao = request.TipoInteracao,
            DataHora = request.DataHora,
            Mensagem = "Interacao registrada e pronta para reprocessamento."
        };

        return Ok(resposta);
    }

    private static int ObterPeso(string prioridade)
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





