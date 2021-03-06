﻿Imports Comum
Imports System.IO
Imports System.Text
Imports System.Net.Sockets
Imports System.Reflection

Public Class RespostaRest
    Implements IResposta

    Private Property MensagemFixa As String = String.Empty

    Const Diretorio As String = Config.DiretorioServidor
    Public Property Mensagem As Byte() = Encoding.ASCII.GetBytes(MensagemFixa) Implements Comum.IResposta.Mensagem

    Public Property Cabecalho As String Implements Comum.IResposta.Cabecalho
    Public Property ClientSock As System.Net.Sockets.Socket Implements Comum.IResposta.ClientSock
    Public Property EnvioRequisicao As Comum.IRequisicao Implements Comum.IResposta.EnvioRequisicao
    Public Property TamanhoLido As Integer Implements Comum.IResposta.TamanhoLido
    Public Property TamanhoMensagem As Integer Implements Comum.IResposta.TamanhoMensagem
    Public Property TamanhoPacoteBytes As Integer = 2048 Implements Comum.IResposta.TamanhoPacoteBytes

    Public Sub New()
        'Carrega cabeçalhos
        Me.Cabecalho = File.ReadAllText(Config.Cabecalho)
    End Sub

    Public Sub DefineCabecalho(ByVal pParametro As Comum.HttpFields, ByVal pValor As String) Implements Comum.IResposta.DefineCabecalho
        Select Case pParametro
            Case HttpFields.ContentLength
                Me.Cabecalho = Me.Cabecalho.Replace("@Content-Length", pValor)
            Case HttpFields.Date
                Me.Cabecalho = Me.Cabecalho.Replace("@Date", pValor)
            Case HttpFields.ContentType
                Me.Cabecalho = Me.Cabecalho.Replace("@Content-Type", pValor)
        End Select
    End Sub

    Public Function TrataMensagem() As Byte() Implements Comum.IResposta.TrataMensagem
        Dim sendData() As Byte = New Byte((TamanhoPacoteBytes) - 1) {}

        'Verifica se a mensagem ultrapassa o limite do pacote
        If MensagemFixa.Length <= TamanhoLido Then
            Return Nothing
        End If

        If MensagemFixa.Substring(TamanhoLido).ToString.Length < sendData.Length - Cabecalho.Length Then
            sendData = Encoding.UTF8.GetBytes(Cabecalho & MensagemFixa.Substring(TamanhoLido))
        Else
            'Obter quanto bytes devo obter da mensagem
            sendData = Encoding.UTF8.GetBytes(Cabecalho & MensagemFixa.Substring(TamanhoLido, (sendData.Length - Cabecalho.Length)))
        End If

        'Incrementa leitura
        TamanhoLido = TamanhoLido + (sendData.Length - Cabecalho.Length)

        'Limpa cabeçalho após o primeiro envio
        Cabecalho = String.Empty

        If sendData.Length.Equals(0) Then
            Return Nothing
        End If

        Return sendData

    End Function

    Public Sub Write() Implements Comum.IResposta.Write
        Try
            'Carrega arquivo javascript
            CarregaMensagem()

            'Envia resposta
            Me.Cabecalho = Me.Cabecalho.Replace("@Cookie", String.Empty)
            DefineCabecalho(HttpFields.ContentLength, MensagemFixa.Length)
            DefineCabecalho(HttpFields.Date, FormatDateTime(Date.Now, DateFormat.LongDate))
            DefineCabecalho(HttpFields.ContentType, "text/xml")

            Dim sendData() As Byte = TrataMensagem()

            While sendData IsNot Nothing
                ClientSock.Send(sendData)
                sendData = TrataMensagem()
            End While
            '===============
        Finally
            If ClientSock IsNot Nothing Then
                'ClientSock.Close()
                ClientSock.Shutdown(SocketShutdown.Both)
            End If
        End Try
    End Sub

    Private Sub CarregaMensagem()
        Dim wrkLista() As String = EnvioRequisicao.RequisicaoString.Split(Environment.NewLine)
        If wrkLista(0).IndexOf("/") > 0 Then
            Dim wrkNomeArquivo As String = wrkLista(0).Substring(wrkLista(0).IndexOf("/"))
            wrkNomeArquivo = wrkNomeArquivo.Substring(1, wrkNomeArquivo.IndexOf(" ")).Trim

            Dim wrkAssembly As Assembly = Assembly.LoadFrom(Comum.Config.PathAplicacaoWeb)
            Dim wrkInstance As Object = wrkAssembly.CreateInstance("AplicacaoWeb.MetodosRest", True, BindingFlags.CreateInstance, Nothing, Nothing, Nothing, Nothing)
            Dim wrkIMetodosRest As IMetodosRest = CType(wrkInstance, IMetodosRest)

            If wrkNomeArquivo.ToLower.IndexOf("autocomplete.rest") >= 0 Then
                Dim wrkTabela As String = Web.HttpUtility.ParseQueryString(wrkNomeArquivo).Item(0)
                Dim wrkCampo As String = Web.HttpUtility.ParseQueryString(wrkNomeArquivo).Item("campo")
                Dim wrkCaractersDigitados As String = Web.HttpUtility.ParseQueryString(wrkNomeArquivo).Item("caracter")

                'Verifica se foi passado os paramentros esperados
                If wrkTabela IsNot Nothing AndAlso wrkCampo IsNot Nothing Then
                    If wrkCaractersDigitados Is Nothing Then wrkCaractersDigitados = String.Empty

                    MensagemFixa = wrkIMetodosRest.RetornaAutoCompleteXML(wrkTabela, wrkCampo, wrkCaractersDigitados)
                End If
            ElseIf wrkNomeArquivo.ToLower.Equals("categoria.rest") Then
                MensagemFixa = wrkIMetodosRest.RetornaCategorias
            ElseIf wrkNomeArquivo.ToLower.IndexOf("subcategoria.rest") >= 0 Then
                MensagemFixa = wrkIMetodosRest.RetornaSubCategorias(CInt(Web.HttpUtility.ParseQueryString(wrkNomeArquivo).Item(0)), True)
            ElseIf wrkNomeArquivo.ToLower.IndexOf("listaproduto.rest") >= 0 Then
                MensagemFixa = wrkIMetodosRest.RetornaListaProdutos(wrkNomeArquivo)
            ElseIf wrkNomeArquivo.ToLower.IndexOf("carregaproduto.rest") >= 0 Then
                MensagemFixa = wrkIMetodosRest.RetornaProduto(CInt(Web.HttpUtility.ParseQueryString(wrkNomeArquivo).Item(0)))
            ElseIf wrkNomeArquivo.ToLower.IndexOf("excluiproduto.rest") >= 0 Then
                MensagemFixa = wrkIMetodosRest.ProdutoExclui(CInt(Web.HttpUtility.ParseQueryString(wrkNomeArquivo).Item(0)))
            ElseIf wrkNomeArquivo.ToLower.IndexOf("produto.rest") >= 0 Then
                MensagemFixa = wrkIMetodosRest.ProdutoSalva(wrkNomeArquivo)
            End If

        End If

    End Sub

    Public Sub SetCookie(ByVal pCookie As Cookie) Implements Comum.IResposta.SetCookie
    End Sub
End Class
