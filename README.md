

# Priorizar Atendimento

<p align="center">
  Priorização operacional com API em C#, regra legada em VB6 e centralização progressiva da inteligência no PostgreSQL.
</p>

<div align="center">
  <img src="https://img.shields.io/badge/c%23-512BD4?style=for-the-badge&logo=csharp&logoColor=white">
  <img src="https://img.shields.io/badge/.net%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white">
  <img src="https://img.shields.io/badge/postgresql-4169E1?style=for-the-badge&logo=postgresql&logoColor=white">
  <img src="https://img.shields.io/badge/vb6-5C2D91?style=for-the-badge&logo=visualstudio&logoColor=white">
</div>

<br>

## Contexto

Este projeto simula um cenário corporativo real, onde a operação precisa decidir quem atender primeiro, qual ação tomar e quando escalar um caso, convivendo com três camadas ao mesmo tempo:

- entrada operacional (dados brutos);
- regra herdada em VB6;
- evolução da inteligência para PostgreSQL.

O objetivo é representar um ambiente com **sistema legado, baixa documentação e necessidade de investigação constante**, aproximando-se de um contexto real de desenvolvimento.

---

- [Projeto](#projeto)
- [Funcionalidades](#funcionalidades)
- [Como rodar localmente](#como-rodar-localmente)
- [Estrutura](#estrutura)
- [Tecnologias e Ferramentas](#tecnologias-e-ferramentas)
- [Licença](#licença)

---

## Projeto

O objetivo do Priorizar Atendimento é representar a evolução incremental de um sistema interno de decisão operacional, mostrando como uma aplicação moderna pode:

- ler uma base operacional de clientes;
- considerar comportamento herdado em um módulo legado VB6;
- expor decisões por API em C#;
- migrar gradualmente a inteligência de negócio para functions SQL no PostgreSQL.

Em vez de simular um projeto "limpo desde o início", a proposta aqui é mostrar um fluxo mais próximo de ambiente real:

- parte da regra ainda mora no legado;
- a API moderna precisa respeitar essa regra;
- a decisão começa na aplicação e evolui para o banco;
- cada etapa pode ser testada e versionada separadamente.

---

## Funcionalidades

1. Listagem de clientes com status operacional consolidado
2. Endpoint de prioridade para indicar quem deve ser tratado primeiro
3. Consulta de status individual por cliente
4. Registro de interações operacionais por API
5. Leitura da base inicial a partir de CSV
6. Interpretação de regra legada por meio de `Regras.bas`
7. Functions SQL para classificar, priorizar e resumir o caso
8. Teste real de conexão com PostgreSQL
9. Repositório de decisão preparado para consumir a camada SQL com fallback em C#

---

## Como rodar localmente

### 1. Configure a connection string local

Crie o arquivo:

```text
src/api/PriorizarAtendimento.Api/appsettings.Local.json
```

Use este conteúdo como exemplo:

```json
{
  "Database": {
    "ConnectionString": "Host=localhost;Port=5432;Database=priorizar_atendimento;Username=postgres;Password=postgres"
  }
}
```

Observações importantes:

- `appsettings.Local.json` é o arquivo real da sua máquina
- `appsettings.Local.example.json` existe apenas como modelo
- `appsettings.Development.json` fica como fallback sem credencial real

### 2. Execute as functions SQL no PostgreSQL

Antes de usar o fluxo com banco, rode no PostgreSQL os arquivos:

```text
database/functions/fn_classificar_cliente.sql
database/functions/fn_priorizar_atendimento.sql
database/functions/fn_resumo_cliente.sql
```

### 3. Rode a API

Na raiz do projeto:

```powershell
dotnet build .\priorizar_atendimento.sln
dotnet run --project .\src\api\PriorizarAtendimento.Api\PriorizarAtendimento.Api.csproj
```

Abra no navegador:

```text
http://localhost:{{host}}/swagger
```

### 4. Fluxo esperado

- a API expõe os endpoints principais via Swagger
- a base operacional é lida de `samples/input/base_clientes_cobranca.csv`
- o legado VB6 influencia a ação recomendada
- o repositório de decisão tenta consultar a camada SQL
- em caso de falha, existe fallback controlado para a regra em C#

Se o PostgreSQL não estiver acessível, o endpoint `GET /api/banco/teste` retorna erro controlado e o fluxo de decisão continua com fallback local.

## Estrutura

```text
priorizar_atendimento/
├── database/
│   └── functions/                 # functions SQL da regra de negocio
├── samples/
│   └── input/                     # base operacional simulada em CSV
├── src/
│   ├── api/
│   │   └── PriorizarAtendimento.Api/
│   │       ├── Controllers/       # endpoints da API
│   │       ├── Models/            # contratos e modelos de transporte
│   │       ├── Services/          # leitura de base, legado, repositorio e conexao
│   │       ├── appsettings*.json  # configuracoes da aplicacao
│   │       └── Program.cs         # bootstrap da API
│   └── legacy/
│       └── Regras.bas             # fonte principal da regra legada em VB6
├── priorizar_atendimento.sln
└── README.md
```

## Tecnologias e Ferramentas

1. C# com ASP.NET Core
2. PostgreSQL
3. Npgsql
4. VB6 como legado simulado
5. SQL / PLpgSQL
6. CSV como entrada operacional inicial

## Licença

Este repositório está licenciado sob a [MIT License](./LICENSE).
