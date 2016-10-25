Imports Comum
Imports System.IO
Imports System.Text
Imports System.Net.Sockets
Imports System.Web

Public Class RespostaGenerica
    Implements IResposta

    Private Property MensagemFixa As String = "<HTML>" &
     "<HEAD>" &
     "    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/> " &
     "    <TITLE>Resposta do servidor</TITLE>" &
     "</HEAD>" &
    "" &
     "<BODY BGCOLOR=""WHITE"">" &
    "	<CENTER>" &
    "		<FONT>Resposta do servidor</FONT>" &
    "		<HR/>" &
    "	</CENTER>" &
    "	Data atual do servidor: " & FormatDateTime(Date.Now, DateFormat.LongDate) &
    " </BODY>" &
    "</HTML>"

    Const Diretorio As String = Comum.Config.DiretorioServidor
    Public Property Mensagem As Byte() = Encoding.ASCII.GetBytes(MensagemFixa) Implements Comum.IResposta.Mensagem

    Public Property Cabecalho As String Implements Comum.IResposta.Cabecalho
    Public Property ClientSock As System.Net.Sockets.Socket Implements Comum.IResposta.ClientSock
    Public Property EnvioRequisicao As Comum.IRequisicao Implements Comum.IResposta.EnvioRequisicao
    Public Property TamanhoLido As Integer Implements Comum.IResposta.TamanhoLido
    Public Property TamanhoMensagem As Integer Implements Comum.IResposta.TamanhoMensagem
    Public Property TamanhoPacoteBytes As Integer = 2048 Implements Comum.IResposta.TamanhoPacoteBytes
    Private Property Usuario As String

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
            'Verifica se é para enviar a requisição
            If EnvioRequisicao IsNot Nothing AndAlso EnvioRequisicao.IsDebugger Then
                MensagemFixa = EnvioRequisicao.RequisicaoString.Replace(Environment.NewLine, "<br/>")
            End If

            CarregaMensagem()

            'Envia resposta
            Me.Cabecalho = Me.Cabecalho.Replace("@Cookie", String.Empty)
            DefineCabecalho(HttpFields.ContentLength, MensagemFixa.Length)
            DefineCabecalho(HttpFields.Date, FormatDateTime(Date.Now, DateFormat.LongDate))
            DefineCabecalho(HttpFields.ContentType, Comum.MIMEType.GetMimeType(EnvioRequisicao.RequisicaoString).MimeTypeString)

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
            Dim wrkArquivoRetorno As String = String.Empty
            Dim wrkNomeArquivo As String

            'Obtem nome do arquivo
            wrkNomeArquivo = wrkLista(0).Substring(wrkLista(0).IndexOf("/"))
            wrkNomeArquivo = wrkNomeArquivo.Substring(1, wrkNomeArquivo.IndexOf(" ")).Trim

            'Retira os request do endereço
            If wrkNomeArquivo.IndexOf("?") > 0 Then
                wrkArquivoRetorno = wrkNomeArquivo.Substring(wrkNomeArquivo.IndexOf("retorno=") + 8).Trim
                wrkNomeArquivo = wrkNomeArquivo.Substring(0, wrkNomeArquivo.IndexOf("?"))
            End If

            'Veririca se deve pbter o retorno do post
            If EnvioRequisicao.GetMetodo = Comum.Metodo.Post AndAlso
               String.IsNullOrEmpty(wrkArquivoRetorno) Then
                wrkArquivoRetorno = EnvioRequisicao.GetParametros.Find(Function(T) T.Parametro.Equals("Referer")).Valor.Trim
                wrkArquivoRetorno = wrkArquivoRetorno.Substring(wrkArquivoRetorno.IndexOf("retorno=") + 8).Trim
            End If

            If wrkNomeArquivo.ToLower.IndexOf("novologin.html") >= 0 Then
                CadastrarUsuario()
            ElseIf File.Exists(Path.Combine(Diretorio, wrkNomeArquivo)) Then
                'Carrega html a ser enviado
                MensagemFixa = File.ReadAllText(Path.Combine(Diretorio, wrkNomeArquivo))

                'Verifica autenticação do usuário
                If Not wrkNomeArquivo.ToLower.IndexOf("login.html") >= 0 Then
                    If Not UsuarioAutenticado() Then
                        'Envio da página de login
                        CarregaPaginaLogin(wrkNomeArquivo)
                    End If
                Else
                    'Trata envio de requisição de login
                    If EnvioRequisicao.GetMetodo = Comum.Metodo.Post Then
                        If Not TrataLogin() Then
                            'Redireciona para url de acesso negado
                            MensagemFixa = File.ReadAllText(Path.Combine(Diretorio, "AcessoNegado.html"))
                        Else
                            'Cria Cookie do usuário e redireciona para pagina desejada
                            SetaCookieUsuario()
                            MensagemFixa = File.ReadAllText(Path.Combine(Diretorio, HttpUtility.UrlDecode(wrkArquivoRetorno)))

                            'Insere tag meta
                            Dim wrkPaginaRetorno As String = HttpUtility.UrlDecode(wrkArquivoRetorno).Substring(HttpUtility.UrlDecode(wrkArquivoRetorno).LastIndexOf("/") + 1)
                            MensagemFixa = MensagemFixa.Replace("</head>", String.Format("<meta http-equiv=""refresh"" content=""0;url={0}"" />",
                                                          wrkPaginaRetorno) & Environment.NewLine & "</head>")
                        End If
                    End If
                End If
            Else
                MensagemFixa = File.ReadAllText(Path.Combine(Diretorio, "FileNotFound.html"))
            End If

            End If
    End Sub

    Private Sub SetaCookieUsuario()
        Dim wrkCookie As New Cookie("usuarioautenticado", Usuario)
        'wrkCookie.SetDuracao(..)
        SetCookie(wrkCookie)
    End Sub

    Private Function TrataLogin() As Boolean
        Dim wrkUsuario As String = String.Empty
        Dim wrkSenha As String = String.Empty

        'Busca Usuario e Senha
        Dim lista As String() = EnvioRequisicao.RequisicaoString.Split(Environment.NewLine)

        For ind = 0 To lista.Count - 1
            'Verifica se encontra os parametros
            If lista(ind).IndexOf("name=""usuario""") > 0 Then
                ind += 2
                wrkUsuario = lista(ind).Trim
            End If
            If lista(ind).IndexOf("name=""senha""") > 0 Then
                ind += 2
                wrkSenha = lista(ind).Trim
            End If

            'Se já encontrou ambos, sai do for
            If Not String.IsNullOrEmpty(wrkUsuario) AndAlso
                Not String.IsNullOrEmpty(wrkSenha) Then
                Exit For
            End If
        Next
        '======================

        'Valida usuario e senha
        Usuario = wrkUsuario
        Return Autenticacao.Autentica(wrkUsuario, wrkSenha)

    End Function

    Private Sub CadastrarUsuario()
        'Busca Usuario e Senha
        Dim lista As String() = EnvioRequisicao.RequisicaoString.Split(Environment.NewLine)
        Dim parametros As String = lista(lista.Count - 1)

        Dim wrkUsuario As String = Web.HttpUtility.ParseQueryString(parametros).Item(0)
        Dim wrkSenha As String = Web.HttpUtility.ParseQueryString(parametros).Item("txtNovoSenha")

        MensagemFixa = "<html>" &
                         "<head>" &
                         "    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/> " &
                         "    <title>Resposta do servidor</title>" &
                         "</head>" &
                         "<body bgcolor=""white"">" &
                        "	<center>" &
                        "		<font>{0}</font>" &
                        "		<hr/>" &
                        "	</center>" &
                        " </body>" &
                        "</html>"

        If Autenticacao.NovoUsuario(wrkUsuario.ToLower, wrkSenha.ToLower) Then
            MensagemFixa = String.Format(MensagemFixa, "Usuário cadastrado com sucesso.")
        Else
            MensagemFixa = String.Format(MensagemFixa, "Não foi possível realizar o cadastro do usuário.")
        End If

    End Sub

    Private Sub CarregaPaginaLogin(ByVal pRetorno As String)
        MensagemFixa = File.ReadAllText(Path.Combine(Diretorio, "AplicativoSemirario\Login.html"), Encoding.UTF8)

        'Substitui a página que irá retornar no meta
        If Not String.IsNullOrEmpty(pRetorno) Then
            Dim urlEncode As String = System.Web.HttpUtility.UrlEncode(pRetorno)

            MensagemFixa = MensagemFixa.Replace("</head>", String.Format("<meta http-equiv=""refresh"" content=""0;url=login.html?retorno={0}"" />",
                                                          urlEncode) & Environment.NewLine & "</head>")
        End If
    End Sub

    Private Function UsuarioAutenticado() As Boolean
        If EnvioRequisicao.GetCookies Is Nothing OrElse
            EnvioRequisicao.GetCookies.Find(Function(T) T.Nome.Equals("usuarioautenticado")) Is Nothing Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Sub SetCookie(ByVal pCookie As Cookie) Implements Comum.IResposta.SetCookie
        Dim wrkSetCookie As String = String.Format("Set-Cookie: {0}={1}", pCookie.Nome, pCookie.Valor)
        Me.Cabecalho = Me.Cabecalho.Replace("@Cookie", wrkSetCookie & Environment.NewLine & "@Cookie")
    End Sub

End Class
