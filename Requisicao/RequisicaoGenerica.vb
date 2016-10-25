Imports Comum

Public Class RequisicaoGenerica
    Implements IRequisicao

    Public Property IsDebugger As Boolean Implements IRequisicao.IsDebugger
    Public Property ListaParametros As System.Collections.Generic.List(Of HttpParametros) Implements IRequisicao.ListaParametros
    Public Property RequisicaoString As String Implements IRequisicao.RequisicaoString

    Public Sub New(ByVal pRequisicao As String)
        RequisicaoString = pRequisicao
        GetParametros()

        Dim wrkLista() As String = RequisicaoString.Split(Environment.NewLine)
        If wrkLista(0).ToLower.IndexOf("debugger") > 0 Then
            IsDebugger = True
        Else
            IsDebugger = False
        End If
    End Sub

    Public Function GetArquivo(ByVal pIndex As Integer) As Byte() Implements IRequisicao.GetArquivo
        Dim wrkContador As Integer = 0
        Dim wrkPosicao As Integer = 0

        'Verifica se acha o primeiro file
        wrkPosicao = RequisicaoString.ToLower.IndexOf("filename=")

        'Se não encontrou o primeiro arquivo, sai da rotina
        If wrkPosicao <= 0 AndAlso pIndex.Equals(0) Then
            Return Nothing
        ElseIf pIndex > 0 Then
            'Percorre a string para encontrar os proximos
            While wrkPosicao >= 0
                wrkContador += 1
                wrkPosicao = wrkPosicao + 10
                wrkPosicao = RequisicaoString.ToLower.IndexOf("filename=", wrkPosicao + 10)

                'Verifica se é o index solicitado
                If wrkPosicao > 0 AndAlso pIndex.Equals(wrkContador) Then
                    Exit While
                End If
            End While
        End If

        'Verifica se encontrou o arquivo
        If wrkPosicao <= 0 Then
            Return Nothing
        End If

        'Obtem string base64 do arquivo
        Dim wrkArquivo As String = String.Empty
        Dim wrkLinhas() As String = RequisicaoString.Substring(wrkPosicao).Split(Environment.NewLine)
        For wrkInd As Integer = 0 To wrkLinhas.Count - 1
            If String.IsNullOrEmpty(wrkLinhas(wrkInd).Trim) Then
                'Proxima linha representará em base64 o arquivos
                wrkArquivo = wrkLinhas(wrkInd + 1)
                Exit For
            End If
        Next

        'Verifica se encontrou o arquivos
        If Not String.IsNullOrEmpty(wrkArquivo.Trim) Then
            Return Text.Encoding.ASCII.GetBytes(wrkArquivo)
        Else
            Return Nothing
        End If

        Return Nothing
    End Function

    Public Function GetMetodo() As Metodo Implements IRequisicao.GetMetodo
        Select Case RequisicaoString.Substring(0, 4).Trim
            Case "POST"
                Return Metodo.Post
            Case "GET"
                Return Metodo.Get
            Case "PUT"
                Return Metodo.Put
            Case "DELE"
                Return Metodo.Delete
        End Select

        'If RequisicaoString.Substring(0, 4).Equals("POST") Then
        '    Return Metodo.Post
        'Else
        '    Return Metodo.Get
        'End If
    End Function

    Public Function GetParametros() As System.Collections.Generic.List(Of HttpParametros) Implements IRequisicao.GetParametros
        Dim wrkLista() As String = RequisicaoString.Split(Environment.NewLine)
        ListaParametros = New List(Of HttpParametros)

        'Passa por cada linha da requisião
        For Each wrkLinha In wrkLista
            'Verifica se não é o fim do cabeçalho
            If String.IsNullOrEmpty(wrkLinha.Trim) Then
                Exit For
            End If

            'Verifica se não é a primeira linha
            If wrkLinha.IndexOf(":") > 0 Then
                'Cerra o item
                Dim wrkItemParametro As New HttpParametros
                wrkItemParametro.Parametro = wrkLinha.Substring(0, wrkLinha.IndexOf(":")).Trim
                wrkItemParametro.Valor = wrkLinha.Substring(wrkLinha.IndexOf(":") + 1).Trim

                'Adiciona item à lista
                ListaParametros.Add(wrkItemParametro)
            End If
        Next

        Return ListaParametros
    End Function

    Public Function GetTipoConteudo() As TipoConteudo Implements IRequisicao.GetTipoConteudo
        If GetValor("Content-Type").IndexOf("multipart") >= 0 Then
            Return TipoConteudo.MultipartFormData
        Else
            Return TipoConteudo.Application
        End If
    End Function

    Public Function GetTotalArquivos() As Integer Implements IRequisicao.GetTotalArquivos
        Dim wrkContador As Integer = 0
        Dim wrkPosicao As Integer = 0

        'Verifica se acha o primeiro file
        wrkPosicao = RequisicaoString.ToLower.IndexOf("filename=")

        'Percorre a string para encontrar os proximos
        While wrkPosicao >= 0
            wrkContador += 1
            wrkPosicao = wrkPosicao + 10
            wrkPosicao = RequisicaoString.ToLower.IndexOf("filename=", wrkPosicao + 10)
        End While

        'Retorna contagem
        Return wrkContador
    End Function

    Public Function GetValor(ByVal pParametro As String) As String Implements IRequisicao.GetValor
        If ListaParametros.Find(Function(T) T.Parametro.ToLower.Equals(pParametro.ToLower)) IsNot Nothing Then
            Return ListaParametros.Find(Function(T) T.Parametro.ToLower.Equals(pParametro.ToLower)).Valor
        Else
            Return String.Empty
        End If
    End Function

    Public Function GetCookies() As List(Of Cookie) Implements IRequisicao.GetCookies
        Dim wrkCookies As String = GetValor("Cookie")

        'Verifica se existe cookies
        If wrkCookies.Equals(String.Empty) Then
            Return Nothing
        End If
        '==========================

        Dim wrkListaCookie As New List(Of Cookie)

        'Passa por cada cookies
        For Each wrkItem In wrkCookies.Split(";")
            Dim wrkNomeValor() As String = wrkItem.Split("=")
            Dim wrkCookie As New Cookie(wrkNomeValor(0).Trim, wrkNomeValor(1).Trim)
            wrkListaCookie.Add(wrkCookie)
        Next

        'Realiza o retorno da lista
        Return wrkListaCookie

    End Function

    Public Function GetSession() As Sessao Implements IRequisicao.GetSession
        '...
        Return Nothing
    End Function

End Class
