# Functions SQL

Estas functions representam a centralizacao progressiva da regra de negocio no banco.

## Ordem de uso

1. `fn_classificar_cliente`
   Usa os sinais operacionais e o retorno do legado VB6 para classificar o cliente.
2. `fn_priorizar_atendimento`
   Define prioridade, acão recomendada e motivo.
3. `fn_resumo_cliente`
   Consolida a visao final do caso em uma única consulta.

## Observacão

Nesta etapa, os arquivos ainda não estão conectados ao C#.

Eles existem para:

- formalizar a regra;
- preparar a futura integração com PostgreSQL;
- mostrar a evolução da inteligencia para o banco.
