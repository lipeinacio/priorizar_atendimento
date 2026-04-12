create or replace function fn_priorizar_atendimento(
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
    prioridade text,
    acao_recomendada text,
    motivo text
)
language plpgsql
as
$$
begin
    if p_linha_instavel = true
       or p_entrega_confirmada = false
       or p_acao_legado = 'REVISAR_LINHA_TEMPLATE' then
        return query
        select
            'ALTA'::text,
            'REVISAR_OPERACAO'::text,
            'Mensagem sem entrega confirmada, linha instavel ou retorno do legado indicando revisao de linha.'::text;
        return;
    end if;

    if p_contato_atendido = true
       or p_interagiu = true
       or p_acao_legado = 'TRATAR_HUMANO' then
        return query
        select
            'MEDIA'::text,
            'ENCAMINHAR_ATENDIMENTO'::text,
            'Cliente com sinal de interacao humana ou retorno herdado para tratativa manual.'::text;
        return;
    end if;

    if p_boleto_gerado = true
       or p_acao_legado = 'PAUSAR_48H' then
        return query
        select
            'MEDIA'::text,
            'PAUSAR_COBRANCA'::text,
            'Ja existe indicio de conversao recente ou o legado mandou pausar temporariamente.'::text;
        return;
    end if;

    if p_acao_legado = 'COBRANCA_LEVE' then
        return query
        select
            'BAIXA'::text,
            'CONTATO_MODERADO'::text,
            'Modulo legado VB6 classificou o caso como cobranca leve.'::text;
        return;
    end if;

    if (p_dias_atraso >= 30 and p_mensagem_enviada = true and p_leitura_confirmada = true)
       or p_acao_legado = 'COBRANCA_FORTE' then
        return query
        select
            case
                when p_cliente_fidelizado = true then 'MEDIA'::text
                else 'ALTA'::text
            end,
            'CONTINUAR_COBRANCA'::text,
            'Cliente elegivel para nova tentativa automatizada ou retorno do legado indicando cobranca forte.'::text;
        return;
    end if;

    return query
    select
        'BAIXA'::text,
        'MONITORAR'::text,
        'Caso sem criterio suficiente para acao imediata.'::text;
end;
$$;