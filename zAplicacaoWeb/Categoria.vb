Imports MySql.Data.MySqlClient

Public Class Categoria

    Public Property Id As Integer
    Public Property Descricao As String

    Public Shared Function Busca(ByVal pCategoria As Integer) As Categoria
        Dim conexaoMySQL As New MySqlConnection(Conexao.ConnectionString)
        Dim retorno As New Categoria

        Try
            Dim cmdMySQL As MySqlCommand = conexaoMySQL.CreateCommand
            Dim reader As MySqlDataReader
            cmdMySQL.CommandText = "SELECT * FROM Categoria WHERE Id = " & pCategoria
            conexaoMySQL.Open()
            reader = cmdMySQL.ExecuteReader

            If reader.Read Then
                retorno.Id = Utils.IntDB(reader.Item("Id"))
                retorno.Descricao = Utils.StringDB(reader.Item("Descricao"))
            End If
        Finally
            If conexaoMySQL.State.Equals(ConnectionState.Open) Then
                conexaoMySQL.Close()
            End If
        End Try

        Return retorno
    End Function

    Public Shared Function Busca() As List(Of Categoria)
        Dim conexaoMySQL As New MySqlConnection(Conexao.ConnectionString)
        Dim retorno As New List(Of Categoria)

        Try
            Dim cmdMySQL As MySqlCommand = conexaoMySQL.CreateCommand
            Dim reader As MySqlDataReader
            cmdMySQL.CommandText = "SELECT * FROM Categoria"
            conexaoMySQL.Open()
            reader = cmdMySQL.ExecuteReader

            While reader.Read
                Dim item As New Categoria
                item.Id = Utils.IntDB(reader.Item("Id"))
                item.Descricao = Utils.StringDB(reader.Item("Descricao"))
                retorno.Add(item)
            End While
        Finally
            If conexaoMySQL.State.Equals(ConnectionState.Open) Then
                conexaoMySQL.Close()
            End If
        End Try

        Return retorno
    End Function

End Class
