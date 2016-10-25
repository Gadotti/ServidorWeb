Imports MySql.Data.MySqlClient

Public Class SubCategoria

    Public Property Id As Integer
    Public Property CategoriaId As Integer
    Public Property Descricao As String

    Public Shared Function BuscaSubCategoria(ByVal pSubCategoria As Integer) As SubCategoria
        Dim conexaoMySQL As New MySqlConnection(Conexao.ConnectionString)
        Dim retorno As New SubCategoria

        Try
            Dim cmdMySQL As MySqlCommand = conexaoMySQL.CreateCommand
            Dim reader As MySqlDataReader
            cmdMySQL.CommandText = "SELECT * FROM SubCategoria WHERE Id = " & pSubCategoria
            conexaoMySQL.Open()
            reader = cmdMySQL.ExecuteReader

            If reader.Read Then
                retorno.Id = Utils.IntDB(reader.Item("Id"))
                retorno.Descricao = Utils.StringDB(reader.Item("Descricao"))
                retorno.CategoriaId = Utils.IntDB(reader.Item("CategoriaId"))
            End If
        Finally
            If conexaoMySQL.State.Equals(ConnectionState.Open) Then
                conexaoMySQL.Close()
            End If
        End Try

        Return retorno
    End Function

    Public Shared Function Busca(ByVal pCategoriaId As Integer) As List(Of SubCategoria)
        Dim conexaoMySQL As New MySqlConnection(Conexao.ConnectionString)
        Dim retorno As New List(Of SubCategoria)

        Try
            Dim cmdMySQL As MySqlCommand = conexaoMySQL.CreateCommand
            Dim reader As MySqlDataReader
            cmdMySQL.CommandText = "SELECT * FROM SubCategoria WHERE CategoriaId = " & pCategoriaId
            conexaoMySQL.Open()
            reader = cmdMySQL.ExecuteReader

            While reader.Read
                Dim item As New SubCategoria
                item.Id = Utils.IntDB(reader.Item("Id"))
                item.CategoriaId = Utils.IntDB(reader.Item("CategoriaId"))
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
