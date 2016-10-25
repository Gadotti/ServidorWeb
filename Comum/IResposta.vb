Imports System.Net.Sockets

Public Interface IResposta
    Property ClientSock As Socket
    Property TamanhoPacoteBytes As Integer
    Property TamanhoMensagem As Integer
    Property Cabecalho As String
    Property EnvioRequisicao As IRequisicao

    Property Mensagem As Byte()
    Property TamanhoLido As Integer

    Sub DefineCabecalho(ByVal pParametro As HttpFields, ByVal pValor As String)
    Function TrataMensagem() As Byte()
    Sub Write()

    Sub SetCookie(ByVal pCookie As Cookie)

End Interface
