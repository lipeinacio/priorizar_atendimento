# Melhorias

Este arquivo registra melhorias que percebi ao longo do desenvolvimento.

Algumas são simples e podem ser feitas rápido, outras exigem mais planejamento. A ideia é não perder essas observações e ter um norte claro de evolução.

---

## 1. Centralizar a regra de decisão

Hoje a regra está espalhada em três lugares diferentes: no `Regras.bas`, no `DecisaoResposta.cs` e nas functions do PostgreSQL. Isso funciona para demonstração, mas traz um risco real: o mesmo cliente pode receber classificações diferentes dependendo de qual camada processou a decisão.

Por exemplo, a classificação "forte" considera `DiasAtraso >= 30 && MensagemEnviada`, mas a prioridade exige também `LeituraConfirmada`. Esse tipo de divergência silenciosa é difícil de rastrear em produção.

A ideia seria definir o PostgreSQL como fonte oficial da regra, usar o C# apenas como fallback controlado e manter o VB6 como referência de compatibilidade durante a transição. Uma interface simples já resolve a separação:

```csharp
public interface IMotorDecisao
{
    Task<ResultadoConsultaDecisao> ConsultarAsync(ConsultaDecisao request);
}
```

Com implementações separadas: `MotorDecisaoSql`, `MotorDecisaoFallback` e `MotorDecisaoComFallback`.

---

## 2. Melhorar os logs do fallback

O `catch` atual em `RepositorioDecisao.cs` está vazio.

```csharp
// hoje está assim
catch
{
    // Fallback temporario enquanto a integração com banco ainda está estabilizando.
}
```

O mínimo que precisa entrar no log: `clienteId`, origem da decisão, motivo da falha e se o fallback foi acionado. Algo assim:

```csharp
catch (Exception ex)
{
    _logger.LogWarning(ex,
        "Fallback acionado para cliente {ClienteId}. Origem={Origem} AcaoLegado={AcaoLegado}",
        request.ClienteId,
        "PostgreSQL",
        retornoLegado.AcaoLegado);
}
```

Sem isso fica muito difícil investigar falhas intermitentes.

---

## 3. Reduzir duplicação no controller

O `ClientesController.cs` monta o objeto `ConsultaDecisao` do zero em vários endpoints (`/clientes`, `/prioridade`, `/status/{clienteId}`). Qualquer campo novo precisa ser adicionado em todos esses lugares, o que aumenta a chance de esquecer algum.

Acredito que colocar um objeto privado ja resolva, mas falta implementar e testar, exemplo:

```csharp
private static ConsultaDecisao CriarConsulta(ClienteSituacaoTeste cliente)
{
    return new ConsultaDecisao
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
    };
}
```

---

## 4. Criar testes automatizados para as regras

O coração do projeto está nas regras de classificação e prioridade, mas não tem nenhum teste cobrindo isso.

Os cenários mais importantes para cobrir primeiro:

| Cenário        | Entrada                                 | Resultado esperado      |
| --------------- | --------------------------------------- | ----------------------- |
| Linha instável | `LinhaInstavel = true`                | `REVISAO_OPERACIONAL` |
| Contato humano  | `ContatoAtendido = true`              | `TRATATIVA_HUMANA`    |
| Boleto gerado   | `BoletoGerado = true`                 | `ACOMPANHAMENTO`      |
| Cobrança forte | `DiasAtraso >= 30` + mensagem enviada | `ACIONAMENTO_FORTE`   |
| Sem critério   | sem sinal de risco                      | `MONITORAMENTO`       |

Exemplo de teste:

```csharp
[Fact]
public void Deve_Classificar_Como_Revisao_Operacional_Quando_Linha_Estiver_Instavel()
{
    var service = new DecisaoResposta(new LeitorLegadoFake("COBRANCA_LEVE"));
    var cliente = new ClienteSituacaoTeste { LinhaInstavel = true, EntregaConfirmada = true };

    var resultado = service.ClassificarCliente(cliente);

    Assert.Equal("REVISAO_OPERACIONAL", resultado);
}
```

---

## 5. Tornar o fallback explícito na resposta

Hoje não tem como saber, olhando a resposta da API, se a decisão veio do banco ou do fallback em C#. Adicionar um campo `OrigemDecisao` no retorno resolve isso:

```csharp
public class ResultadoConsultaDecisao
{
    public string Classificacao { get; set; } = string.Empty;
    public string Prioridade { get; set; } = string.Empty;
    public string AcaoRecomendada { get; set; } = string.Empty;
    public string OrigemDecisao { get; set; } = string.Empty; // "POSTGRESQL" | "CSHARP_FALLBACK"
}
```

Isso ajuda bastante no monitoramento e dá mais segurança durante a transição entre camadas.

---

## 6. Melhorar a documentação operacional

O README está bom como visão geral, mas e bom uma estrutura melhor. Alguns arquivos curtos que fariam diferença:

- `FLUXO_DECISAO.md` — explicando o caminho da requisição pelas três camadas
- `MATRIZ_REGRAS.md` — tabela relacionando sinais do cliente com a ação esperada em cada camada
- `TROUBLESHOOTING.md` — os problemas mais comuns e como investigar

---

## 7. Desacoplar a leitura do CSV

O `DadosTeste.cs` lê o arquivo CSV a cada chamada. Para demonstração funciona, mas se o projeto crescer isso vira um problema. Criar uma interface de fonte de dados já prepara a evolução sem precisar mexer nos endpoints:

```csharp
public interface IClienteFonteDados
{
    Task<IReadOnlyCollection<ClienteSituacaoTeste>> ObterClientesAsync();
}
```

Com isso é fácil trocar por `ClienteFonteBanco` ou `ClienteFonteMock` sem quebrar nada.

---

## 8. Corrigir nomenclaturas

Tens uns nomes errados aqui e outros que precisam de ajuste ali.

- `CleinteList` → `ClienteListItem`
- `DadosTeste` → `ClienteFonteCsv`
- `RepositorioDecisao` → `MotorDecisaoComFallback`

---

## 9. Evoluir o bootstrap da aplicação

O `Program.cs` está bem enxuto, vale adicionar:

```csharp
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
```

Health check, tratamento padronizado de erro e logs estruturados são o mínimo para um serviço que vai para produção.

---

## 10. Falta versionar as functions do banco

Uma estrutura simples já resolve:

```
database/
  migrations/
    001_fn_classificar_cliente.sql
    002_fn_priorizar_atendimento.sql
    003_fn_resumo_cliente.sql
```
