Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports Comum
Imports System.Threading

'Imports System.Xml 'Deletar depois
'Imports System.Xml.XPath

Module Principal

    Private tarefa1, tarefa2, tarefa3 As Thread
    Public server As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP)

    Sub Main()

        server.Bind(New IPEndPoint(IPAddress.Any, 7070))
        server.Listen(100)

        'Faz multiplas threads para escutar as requisições
        tarefa1 = New Thread(AddressOf EscutaRequisicao)
        tarefa2 = New Thread(AddressOf EscutaRequisicao)
        tarefa3 = New Thread(AddressOf EscutaRequisicao)

        'Inicia todas
        tarefa1.Start()
        tarefa2.Start()
        tarefa3.Start()

    End Sub

    Public Sub EscutaRequisicao()
        While True
            'Dim server As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP)
            Try
                'server.Bind(New IPEndPoint(IPAddress.Any, 7070))
                'server.Listen(100)
                Dim wrkSocket As Socket = server.Accept()
                Try
                    Dim wrkConteudo As New StringBuilder
                    Dim wrkTimeoutPrincipal As Date = Now.AddSeconds(30)


                    'Aguardo o socket ficar available
                    While wrkSocket.Available.Equals(0) And DateTime.Now < wrkTimeoutPrincipal
                        System.Threading.Thread.Sleep(1)
                    End While
                    '================================

                    'Verifica se estourou o timeout
                    If wrkSocket.Available.Equals(0) Then
                        Console.WriteLine("Limite de TimeOut excedido para o Socket estar disponível")
                        Exit Try
                    End If
                    '==============================

                    'Recebe a mensagem
                    While (wrkSocket.Available)
                        Dim conteudo() As Byte = New Byte(wrkSocket.Available) {}
                        wrkSocket.Receive(conteudo)
                        wrkConteudo.Append(ASCIIEncoding.UTF8.GetString(conteudo))
                    End While
                    '=================

                    'Verifica se obteve requisição
                    Dim wrkConteudoString As String = wrkConteudo.ToString
                    If String.IsNullOrEmpty(wrkConteudoString) Then
                        Exit Try
                    End If

                    If wrkConteudoString.IndexOf("favicon.ico") > 0 Then
                        Exit Try
                    End If
                    '=============================

                    'Verifica se encontra a biblioteca
                    If Not File.Exists(Config.PathRequisicao) Then
                        Exit Try
                    End If
                    '=================================

                    'Defini parametros dos contrutores
                    Dim wrkParametros() As Object = {wrkConteudoString}

                    'Carrega o assembly e instancia a classe Requisicao
                    'Dim wrkAssembly As Assembly = Assembly.LoadFrom(PathRequisicao)
                    Dim wrkAssembly As Assembly = LeAssemby()
                    Dim wrkInstance As Object = wrkAssembly.CreateInstance("Requisicao.RequisicaoGenerica", True, BindingFlags.CreateInstance, Nothing, wrkParametros, Nothing, Nothing)
                    Dim wrkIRequisicao As IRequisicao = CType(wrkInstance, IRequisicao)
                    '=======================================

                    'Obtem a classe a ser requisitada
                    Dim wrkClasse As String = Mapa.GetMapa(wrkConteudoString).Classe
                    '================================

                    'Carrega o assembly e instancia a classe Requisicao
                    'wrkAssembly = Assembly.LoadFrom(PathRequisicao)
                    'wrkAssembly = LeAssemby()
                    wrkInstance = wrkAssembly.CreateInstance("Requisicao." & wrkClasse, True, BindingFlags.CreateInstance, Nothing, Nothing, Nothing, Nothing)
                    Dim wrkIResposta As IResposta = CType(wrkInstance, IResposta)
                    '=================================================

                    'Obtem e atribui os cookies
                    Dim wrkCookies As List(Of Comum.Cookie) = wrkIRequisicao.GetCookies
                    If wrkCookies IsNot Nothing Then
                        For Each wrkItem In wrkCookies
                            wrkIResposta.SetCookie(wrkItem)
                        Next
                    End If
                    '==========================

                    'Atribui as variáveis e envia resposta
                    wrkIResposta.EnvioRequisicao = wrkIRequisicao
                    wrkIResposta.ClientSock = wrkSocket
                    wrkIResposta.Write()
                    '=====================================

                Catch wrkErro As Exception
                    Console.WriteLine(wrkErro.ToString)

                Finally
                    wrkSocket.Shutdown(SocketShutdown.Both)
                End Try
            Finally
                'server.Close()
            End Try
        End While
    End Sub

    Private Function LeAssemby()
        Dim assemStream() As Byte = Nothing
        Using fs As FileStream = New FileStream(Config.PathRequisicao, FileMode.Open)
            assemStream = New Byte((fs.Length) - 1) {}
            fs.Read(assemStream, 0, CType(fs.Length, Integer))
        End Using

        Return System.Reflection.Assembly.Load(assemStream)
    End Function

    '    Dim wrkRequisicao As New Requisicao(wrkConteudo.ToString)

    '    'Apresentação dos valores de requisição
    '    Console.WriteLine(Environment.NewLine)
    '    Console.WriteLine("Metodo {0}", wrkRequisicao.GetMetodo.ToString)
    '    Console.WriteLine("Tipo Conteudo {0}", wrkRequisicao.GetTipoConteudo.ToString)
    '    Console.WriteLine("Total de Arquivos {0}", wrkRequisicao.GetTotalArquivos.ToString)
    '    Console.WriteLine("Total de parametos {0}", wrkRequisicao.GetParametros.Count)
    '    'Console.WriteLine("Base64 do Arquivo 1 {0}", Encoding.ASCII.GetString(wrkRequisicao.GetArquivo(0)))


    '    'Atrbui a requisição obtida
    '    Dim wrkResposta As New Resposta
    '    wrkResposta.EnvioRequisicao = wrkRequisicao

    '    'Atribui o socket
    '    wrkResposta.ClientSock = s

    '    'Envia resposta
    '    wrkResposta.Write()
    'Testa chamada de um método
    'Dim typ As Type = obj.GetType()
    'Dim retorno As String = typ.InvokeMember("GetMetodo", BindingFlags.InvokeMethod, Nothing, obj, Nothing).ToString()

End Module
