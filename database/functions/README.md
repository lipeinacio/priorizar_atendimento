# Functions SQL

Esta pasta concentra a evolução da regra de negócio para a camada de banco de dados.

Atualmente, a API ainda executa a maior parte da decisão em C#, mas estas functions representam a estrutura que será utilizada para centralizar a inteligência operacional no PostgreSQL.

---

## Objetivo

As functions desta pasta existem para:

- centralizar classificação e priorização no banco;
- reduzir a regra distribuída na API;
- manter consistência entre consulta, integração e análise operacional;
- preparar a transição da decisão local para uma decisão centralizada.

---

## Relação com o projeto

A arquitetura do projeto está organizada em três camadas principais:

- `Regras.bas`: fonte principal da regra legada em VB6;
- `C#`: camada moderna responsável por interpretar e aplicar a regra;
- `SQL`: destino da centralização progressiva da regra de negócio.

Essa estrutura reflete um cenário comum em sistemas corporativos, onde a regra de negócio evolui gradualmente do legado para camadas mais estruturadas.

---

## Functions atuais

### `fn_classificar_cliente.sql`

Responsável por classificar o cliente em categorias como:

- `REVISAO_OPERACIONAL`
- `TRATATIVA_HUMANA`
- `ACOMPANHAMENTO`
- `ACIONAMENTO_MODERADO`
- `ACIONAMENTO_FORTE`
- `MONITORAMENTO`

A classificação considera sinais operacionais e também o retorno proveniente do legado VB6.

---

### `fn_priorizar_atendimento.sql`

Responsável por determinar:

- prioridade;
- ação recomendada;
- motivo.

Essa function representa, no banco, a mesma intenção da regra atualmente implementada na API.

---

### `fn_resumo_cliente.sql`

Responsável por consolidar a visão final do caso em uma única consulta.

Ela agrega:

- classificação;
- prioridade;
- ação recomendada;
- motivo.
