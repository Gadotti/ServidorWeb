Imports Comum
Imports System.IO
Imports System.Text
Imports System.Net.Sockets

Public Class RespostaGrafica
    Implements IResposta

    Const Diretorio As String = Comum.Config.DiretorioServidor
    Public Property Mensagem As Byte() Implements Comum.IResposta.Mensagem
    Public Property Cabecalho As String Implements Comum.IResposta.Cabecalho
    Public Property ClientSock As System.Net.Sockets.Socket Implements Comum.IResposta.ClientSock
    Public Property EnvioRequisicao As Comum.IRequisicao Implements Comum.IResposta.EnvioRequisicao
    Public Property TamanhoLido As Integer Implements Comum.IResposta.TamanhoLido
    Public Property TamanhoMensagem As Integer Implements Comum.IResposta.TamanhoMensagem
    Public Property TamanhoPacoteBytes As Integer = 2048 Implements Comum.IResposta.TamanhoPacoteBytes

    Public Sub New()
        'Carrega cabeçalhos
        Me.Cabecalho = File.ReadAllText(Comum.Config.Cabecalho)
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

        'Teste de envio sem o cabeçalho
        'Cabecalho = String.Empty

        'Verifica se a mensagem ultrapassa o limite do pacote
        If Mensagem.Length <= TamanhoLido Then
            Return Nothing
        End If

        Dim wrkMensagemRestante() As Byte = New Byte(Mensagem.Length - TamanhoLido - 1) {}
        Array.Copy(Mensagem, TamanhoLido, wrkMensagemRestante, 0, (Mensagem.Length - TamanhoLido))

        If wrkMensagemRestante.Length < sendData.Length - Cabecalho.Length Then
            'sendData = Encoding.ASCII.GetBytes(Cabecalho & Encoding.ASCII.GetString(wrkMensagemRestante))
            If Cabecalho.Length.Equals(0) Then
                sendData = wrkMensagemRestante
            End If
        Else
            'Obter quanto bytes devo obter da mensagem
            Dim wrkMensagemTemp() As Byte = New Byte(sendData.Length - Cabecalho.Length - 1) {}
            Array.Copy(wrkMensagemRestante, wrkMensagemTemp, (sendData.Length - Cabecalho.Length))

            'sendData = Encoding.ASCII.GetBytes(Cabecalho & Encoding.ASCII.GetString(wrkMensagemTemp))
            If Cabecalho.Length.Equals(0) Then
                sendData = wrkMensagemTemp
            Else
                Array.Copy(Encoding.ASCII.GetBytes(Cabecalho), sendData, Encoding.ASCII.GetBytes(Cabecalho).Length)
                Array.Copy(wrkMensagemTemp, 0, sendData, Encoding.ASCII.GetBytes(Cabecalho).Length, wrkMensagemTemp.Length)
            End If

        End If

        'Incrementa leitura
        TamanhoLido = TamanhoLido + (sendData.Length - Cabecalho.Length)

        'Limpa cabeçalho após o primeiro envio
        Cabecalho = String.Empty

        Return sendData
    End Function

    Public Sub Write() Implements Comum.IResposta.Write
        Try
            If CarregaArquivo() IsNot Nothing Then
                'Envia resposta
                Me.Cabecalho = Me.Cabecalho.Replace("@Cookie", String.Empty)
                DefineCabecalho(HttpFields.ContentLength, CarregaArquivo.Length)
                DefineCabecalho(HttpFields.Date, FormatDateTime(Date.Now, DateFormat.LongDate))
                DefineCabecalho(HttpFields.ContentType, Comum.MIMEType.GetMimeType(EnvioRequisicao.RequisicaoString).MimeTypeString)

                Mensagem = CarregaArquivo()

                Dim sendData() As Byte = TrataMensagem()

                While sendData IsNot Nothing
                    ClientSock.Send(sendData)
                    sendData = TrataMensagem()
                End While
                '===============
            End If
        Finally
            If ClientSock IsNot Nothing Then
                'ClientSock.Close()
                ClientSock.Shutdown(SocketShutdown.Both)
            End If
        End Try
    End Sub

    Private Function CarregaArquivo() As Byte()
        Dim wrkLista() As String = EnvioRequisicao.RequisicaoString.Split(Environment.NewLine)
        If wrkLista(0).IndexOf("/") > 0 Then
            Dim wrkNomeArquivo As String = wrkLista(0).Substring(wrkLista(0).IndexOf("/"))
            wrkNomeArquivo = wrkNomeArquivo.Substring(1, wrkNomeArquivo.IndexOf(" ")).Trim

            If File.Exists(Path.Combine(Diretorio, wrkNomeArquivo)) Then
                Return File.ReadAllBytes(Path.Combine(Diretorio, wrkNomeArquivo))
            Else
                Return Nothing
            End If

        End If

        Return Nothing

    End Function

    Public Sub SetCookie(ByVal pCookie As Cookie) Implements Comum.IResposta.SetCookie
        '...
    End Sub

End Class
