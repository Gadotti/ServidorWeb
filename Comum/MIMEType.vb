Imports System.IO

Public Class MIMEType

    Public Property Extensao As String
    Public Property MimeTypeString As String

    Public Shared Function GetMimeTypes() As List(Of MIMEType)
        Dim wrkConteudo As String = File.ReadAllText(Config.DiretorioMIMETypes)
        Dim wrkRetorno As New List(Of MIMEType)

        'Passa por cada linha do arquivos
        For Each wrkLinha In wrkConteudo.Split(Environment.NewLine)
            'Obtem as propriedades
            Dim wrkItem As New MIMEType
            wrkItem.Extensao = wrkLinha.Split("=")(0).Trim
            wrkItem.MimeTypeString = wrkLinha.Split("=")(1).Trim

            'Adiciona item à lista de retorno
            wrkRetorno.Add(wrkItem)
        Next

        Return wrkRetorno
    End Function

    Public Shared Function GetMimeType(ByVal pRequisicao As String) As MIMEType
        Dim wrkLista() As String = pRequisicao.Split(Environment.NewLine)
        Dim wrkMapa As MIMEType

        If wrkLista(0).ToLower.IndexOf("/") > 0 Then
           
            Dim wrkExtensao As String = wrkLista(0).Substring(wrkLista(0).IndexOf(" ") + 1).Trim
            wrkExtensao = wrkExtensao.Substring(0, wrkExtensao.IndexOf(" "))
            If wrkExtensao.IndexOf("?") > 0 Then
                wrkExtensao = wrkExtensao.Substring(0, wrkExtensao.IndexOf("?"))
            End If
            wrkExtensao = IO.Path.GetExtension(wrkExtensao)

            wrkMapa = GetMimeTypes.Find(Function(M) M.Extensao.Equals(wrkExtensao))
            If wrkMapa Is Nothing Then
                wrkMapa = GetMimeTypes.Find(Function(M) M.Extensao.Equals("*"))
            End If

            Return wrkMapa
        Else
            Return Nothing
        End If
    End Function
End Class
