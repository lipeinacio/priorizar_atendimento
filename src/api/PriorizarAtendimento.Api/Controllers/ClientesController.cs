using Microsoft.AspNetCore.Mvc;
using PriorizarAtendimento.Api.Models;
using PriorizarAtendimento.Api.Services;

namespace PriorizarAtendimento.Api.Controllers;

[ApiController]
[Route("api")]
public class ClientesController : ControllerBase
{
    private readonly DecisaoResposta _decisionService;
    private readonly DadosTeste _mockDataService;

    public ClientesController(
        DecisaoResposta decisionService,
        DadosTeste mockDataService)
    {
        _decisionService = decisionService;
        _mockDataService = mockDataService;
    }

    [HttpGet("clientes")]
    public ActionResult<IEnumerable<CleinteList>> ListarClientes()
    {
        var clientes = _mockDataService.ObterClientes()
            .Select(cliente =>
            {
                var decisao = _decisionService.Avaliar(cliente);

                return new CleinteList
                {
                    ClienteId = cliente.ClienteId,
                    NomeCliente = cliente.NomeCliente,
                    DiasAtraso = cliente.DiasAtraso,
                    StatusAtual = decisao.AcaoRecomendada
                };
            });

        return Ok(clientes);
    }

    [HttpGet("status/{clienteId:int}")]
    public ActionResult<Status> ObterStatus(int clienteId)
    {
        var cliente = _mockDataService.ObterClientes().FirstOrDefault(item => item.ClienteId == clienteId);
        if (cliente is null)
        {
            return NotFound();
        }

        var decisao = _decisionService.Avaliar(cliente);

        return Ok(new Status
        {
            ClienteId = cliente.ClienteId,
            NomeCliente = cliente.NomeCliente,
            StatusAtual = _decisionService.ClassificarCliente(cliente),
            AcaoRecomendada = decisao.AcaoRecomendada,
            Motivo = decisao.Motivo
        });
    }

    [HttpGet("prioridade")]
    public ActionResult<IEnumerable<PrioridadeAtendimento>> ObterPrioridade()
    {
        var prioridades = _mockDataService.ObterClientes()
            .Select(cliente =>
            {
                var decisao = _decisionService.Avaliar(cliente);

                return new PrioridadeAtendimento
                {
                    ClienteId = cliente.ClienteId,
                    NomeCliente = cliente.NomeCliente,
                    Classificacao = _decisionService.ClassificarCliente(cliente),
                    Prioridade = decisao.Prioridade,
                    AcaoRecomendada = decisao.AcaoRecomendada,
                    Motivo = decisao.Motivo
                };
            })
            .OrderByDescending(item => _decisionService.ObterPesoPrioridade(item.Prioridade))
            .ThenByDescending(item => item.ClienteId)
            .ToList();

        return Ok(prioridades);
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

}






