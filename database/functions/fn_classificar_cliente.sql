create or replace function fn_classificar_cliente(
    p_dias_atraso integer,
    p_mensagem_enviada boolean,
    p_entrega_confirmada boolean,
    p_interagiu boolean,
    p_boleto_gerado boolean,
    p_contato_atendido boolean,
    p_cliente_fidelizado boolean,
    p_linha_instavel boolean,
    p_acao_legado text default null
)
returns text
language plpgsql
as
$$
begin
    if p_linha_instavel = true
       or p_entrega_confirmada = false
       or p_acao_legado = 'REVISAR_LINHA_TEMPLATE' then
        return 'REVISAO_OPERACIONAL';
    end if;

    if p_contato_atendido = true
       or p_interagiu = true
       or p_acao_legado = 'TRATAR_HUMANO' then
        return 'TRATATIVA_HUMANA';
    end if;

    if p_boleto_gerado = true
       or p_acao_legado = 'PAUSAR_48H' then
        return 'ACOMPANHAMENTO';
    end if;

    if p_acao_legado = 'COBRANCA_LEVE' then
        return 'ACIONAMENTO_MODERADO';
    end if;

    if (p_dias_atraso >= 30 and p_mensagem_enviada = true)
       or p_acao_legado = 'COBRANCA_FORTE' then
        if p_cliente_fidelizado = true then
            return 'ACIONAMENTO_MODERADO';
        end if;

        return 'ACIONAMENTO_FORTE';
    end if;

    return 'MONITORAMENTO';
end;
$$;