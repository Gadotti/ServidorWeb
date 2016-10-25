Public Class Cookie

    Public Property Nome As String
    Public Property Valor As String

    Public Sub New(ByVal pNome As String, ByVal pValor As String)
        Nome = pNome
        Valor = pValor
    End Sub

    ''' <summary>
    ''' Seta duração do cookie em segudos
    ''' </summary>
    ''' <param name="pDuracao">Valor expresso em segundos</param>
    ''' <remarks></remarks>
    Public Sub SetDuracao(ByVal pDuracao As Integer)
        '...
    End Sub

End Class
