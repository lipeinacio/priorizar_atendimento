create or replace function fn_resumo_cliente(
    p_cliente_id integer,
    p_nome_cliente text,
    p_dias_atraso integer,
    p_mensagem_enviada boolean,
    p_entrega_confirmada boolean,
    p_interagiu boolean,
    p_boleto_gerado boolean,
    p_contato_atendido boolean,
    p_cliente_fidelizado boolean,
    p_linha_instavel boolean,
    p_leitura_confirmada boolean,
    p_acao_legado text default null
)
returns table (
    cliente_id integer,
    nome_cliente text,
    classificacao text,
    prioridade text,
    acao_recomendada text,
    motivo text
)
language plpgsql
as
$$
begin
    return query
    select
        p_cliente_id,
        p_nome_cliente,
        fn_classificar_cliente(
            p_dias_atraso,
            p_mensagem_enviada,
            p_entrega_confirmada,
            p_interagiu,
            p_boleto_gerado,
            p_contato_atendido,
            p_cliente_fidelizado,
            p_linha_instavel,
            p_acao_legado
        ) as classificacao,
        prioridade_info.prioridade,
        prioridade_info.acao_recomendada,
        prioridade_info.motivo
    from fn_priorizar_atendimento(
        p_dias_atraso,
        p_mensagem_enviada,
        p_entrega_confirmada,
        p_interagiu,
        p_boleto_gerado,
        p_contato_atendido,
        p_cliente_fidelizado,
        p_linha_instavel,
        p_leitura_confirmada,
        p_acao_legado
    ) as prioridade_info;
end;
$$;