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
}






