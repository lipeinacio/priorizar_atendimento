Attribute VB_Name = "Regras"
Option Explicit

' Simulacao de modulo legado em VB6.
' A ideia aqui nao e "codigo bonito", e sim representar uma regra antiga
' que ainda influencia a decisao da cobranca.

Public Function DefinirAcaoCobranca(ByVal diasAtraso As Integer, _
                                    ByVal enviouWhatsapp As Integer, _
                                    ByVal confirmouEntrega As Integer, _
                                    ByVal interagiu As Integer, _
                                    ByVal gerouBoleto As Integer, _
                                    ByVal contatoAtendido As Integer) As String

    If contatoAtendido = 1 Then
        DefinirAcaoCobranca = "TRATAR_HUMANO"
        Exit Function
    End If

    If gerouBoleto = 1 Then
        DefinirAcaoCobranca = "PAUSAR_48H"
        Exit Function
    End If

    If interagiu = 1 Then
        DefinirAcaoCobranca = "AGUARDAR_RETORNO"
        Exit Function
    End If

    If enviouWhatsapp = 0 Then
        DefinirAcaoCobranca = "NAO_ACIONADO"
        Exit Function
    End If

    If confirmouEntrega = 0 Then
        DefinirAcaoCobranca = "REVISAR_LINHA_TEMPLATE"
        Exit Function
    End If

    If diasAtraso >= 30 Then
        DefinirAcaoCobranca = "COBRANCA_FORTE"
    Else
        DefinirAcaoCobranca = "COBRANCA_LEVE"
    End If
End Function