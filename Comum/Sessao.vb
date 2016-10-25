Public Class Sessao
    Public Property Nome As String
    Public Property Valor As Object

    Public Sub New(ByVal pNome As String, ByVal pValor As Object)
        Nome = pNome
        Valor = Valor
    End Sub

    Public Shared Function GetAtributo(ByVal pNome As String) As Object
        '...
        Return Nothing
    End Function

    Public Shared Sub SetAtributo(ByVal pNome As String, ByVal pValor As Object)
        Dim wrkSessao As New Sessao(pNome, pValor)
        '....
    End Sub

End Class
