Imports System.IO

Public Class Mapa

    Public Property Padrao As String
    Public Property Classe As String

    Public Shared Function GetMapas() As List(Of Mapa)
        Dim wrkConteudo As String = File.ReadAllText(Config.DiretorioMapa)
        Dim wrkRetorno As New List(Of Mapa)

        'Passa por cada linha do arquivos
        For Each wrkLinha In wrkConteudo.Split(Environment.NewLine)
            'Obtem as propriedades
            Dim wrkItem As New Mapa
            wrkItem.Padrao = wrkLinha.Split("=")(0).Trim
            wrkItem.Classe = wrkLinha.Split("=")(1).Trim

            'Adiciona item à lista de retorno
            wrkRetorno.Add(wrkItem)
        Next

        Return wrkRetorno
    End Function

    Public Shared Function GetMapa(ByVal pRequisicao As String) As Mapa
        Dim wrkLista() As String = pRequisicao.Split(Environment.NewLine)
        If wrkLista(0).ToLower.IndexOf("/") > 0 Then

            Dim wrkExtensao As String = wrkLista(0).Substring(wrkLista(0).IndexOf(" ") + 1).Trim
            wrkExtensao = wrkExtensao.Substring(0, wrkExtensao.IndexOf(" "))
            If wrkExtensao.IndexOf("?") > 0 Then
                wrkExtensao = wrkExtensao.Substring(0, wrkExtensao.IndexOf("?"))
            End If
            wrkExtensao = IO.Path.GetExtension(wrkExtensao).Replace(".", "")

            Dim wrkMapa As Mapa = GetMapas.Find(Function(M) M.Padrao.Equals(wrkExtensao))
            If wrkMapa Is Nothing Then
                wrkMapa = GetMapas.Find(Function(M) M.Padrao.Equals("*"))
            End If

            Return wrkMapa
        Else
            Return Nothing
        End If
    End Function

End Class
