Public Class Utils

    Public Shared Function StringDB(ByVal valor As Object) As String
        If valor Is Nothing Then
            Return String.Empty
        End If

        If IsDBNull(valor) Then
            Return String.Empty
        End If

        Return valor.ToString()

    End Function

    Public Shared Function IntDB(ByVal valor As Object) As Integer
        If valor Is Nothing Then
            Return 0
        End If

        If IsDBNull(valor) Then
            Return 0
        End If

        Return CInt(valor)

    End Function

End Class
