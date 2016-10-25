Imports MySql.Data.MySqlClient
Imports Comum
Imports System.IO

Public Class Produto

    Public Property Id As Integer
    Public Property Nome As String
    Public Property SubCategoriaId As Integer
    Public Property CodigoBarrasImagem As String
    Public Property CodigoBarras As String
    Public Property Marca As String
    Public Property Unidade As String
    Public Property Tamanho As String
    Public Property SubCategoriaObj As SubCategoria

    Public ReadOnly Property CategoriaId As Integer
        Get
            Return SubCategoriaObj.CategoriaId
        End Get
    End Property

    Public ReadOnly Property CategoriaDescricao As String
        Get
            Return Categoria.Busca(CategoriaId).Descricao
        End Get
    End Property


    Public Shared Function BuscaDistinct(ByVal pCampo As String, Optional ByVal pCaracteresDigitados As String = "") As ArrayList
        Dim conexaoMySQL As New MySqlConnection(Conexao.ConnectionString)
        Dim retorno As New ArrayList

        Try
            Dim cmdMySQL As MySqlCommand = conexaoMySQL.CreateCommand
            Dim reader As MySqlDataReader
            cmdMySQL.CommandText = "SELECT DISTINCT " & pCampo & " FROM Produto"
            conexaoMySQL.Open()
            reader = cmdMySQL.ExecuteReader

            While reader.Read
                retorno.Add(Utils.StringDB(reader.Item(pCampo)))
            End While
        Finally
            If conexaoMySQL.State.Equals(ConnectionState.Open) Then
                conexaoMySQL.Close()
            End If
        End Try

        Return retorno
    End Function

    Public Shared Function Busca(ByVal pProdutoId As Integer) As Produto

        Dim conexaoMySQL As New MySqlConnection(Conexao.ConnectionString)
        Dim retorno As New Produto

        Try
            Dim cmdMySQL As MySqlCommand = conexaoMySQL.CreateCommand
            Dim reader As MySqlDataReader
            cmdMySQL.CommandText = "SELECT * FROM Produto WHERE Id = " & pProdutoId
            conexaoMySQL.Open()
            reader = cmdMySQL.ExecuteReader

            If reader.Read Then
                retorno.Id = Utils.IntDB(reader.Item("Id"))
                retorno.Nome = Utils.StringDB(reader.Item("Nome"))
                retorno.SubCategoriaId = Utils.IntDB(reader.Item("SubCategoriaId"))
                retorno.CodigoBarrasImagem = Utils.StringDB(reader.Item("CodigoBarrasImagem"))
                retorno.CodigoBarras = Utils.StringDB(reader.Item("CodigoBarras"))
                retorno.Marca = Utils.StringDB(reader.Item("Marca"))
                retorno.Unidade = Utils.StringDB(reader.Item("Unidade"))
                retorno.Tamanho = Utils.StringDB(reader.Item("Tamanho"))
                retorno.SubCategoriaObj = SubCategoria.BuscaSubCategoria(retorno.SubCategoriaId)
            End If
        Finally
            If conexaoMySQL.State.Equals(ConnectionState.Open) Then
                conexaoMySQL.Close()
            End If
        End Try

        Return retorno
    End Function

    Public Shared Function Busca(ByVal pRequisicao As String) As List(Of Produto)

        Dim conexaoMySQL As New MySqlConnection(Conexao.ConnectionString)
        Dim retorno As New List(Of Produto)
        Dim wrkPageSize As Integer = 0
        Dim wrkPageIndex As Integer = 0

        Try
            wrkPageSize = CInt(Web.HttpUtility.ParseQueryString(pRequisicao).Item(0))
            wrkPageIndex = CInt(Web.HttpUtility.ParseQueryString(pRequisicao).Item("pageindex"))

            Dim cmdMySQL As MySqlCommand = conexaoMySQL.CreateCommand
            Dim reader As MySqlDataReader
            cmdMySQL.CommandText = "SELECT * FROM Produto"
            conexaoMySQL.Open()
            reader = cmdMySQL.ExecuteReader

            While reader.Read
                Dim item As New Produto
                item.Id = Utils.IntDB(reader.Item("Id"))
                item.Nome = Utils.StringDB(reader.Item("Nome"))
                item.SubCategoriaId = Utils.IntDB(reader.Item("SubCategoriaId"))
                item.CodigoBarrasImagem = Utils.StringDB(reader.Item("CodigoBarrasImagem"))
                item.CodigoBarras = Utils.StringDB(reader.Item("CodigoBarras"))
                item.Marca = Utils.StringDB(reader.Item("Marca"))
                item.Unidade = Utils.StringDB(reader.Item("Unidade"))
                item.Tamanho = Utils.StringDB(reader.Item("Tamanho"))
                item.SubCategoriaObj = SubCategoria.BuscaSubCategoria(item.SubCategoriaId)

                retorno.Add(item)
            End While

        Finally
            If conexaoMySQL.State.Equals(ConnectionState.Open) Then
                conexaoMySQL.Close()
            End If
        End Try

        'Controla paginação
        If retorno.Count <= wrkPageSize Then
            Return retorno
        Else
            'Calcula o range
            Dim wrkPageSizeCalculado As Integer = wrkPageSize
            Dim wrkTotalCount As Integer = wrkPageSize * (wrkPageIndex + 1)
            If retorno.Count < wrkTotalCount Then
                wrkPageSizeCalculado = retorno.Count - (wrkPageSize * wrkPageIndex)
            End If

            'Cacula o index
            wrkPageIndex = wrkPageIndex * wrkPageSize

            'Retorna o range
            Return retorno.GetRange(wrkPageIndex, wrkPageSizeCalculado)
        End If
    End Function

    Public Shared Function Exclui(ByVal pProdutoId As Integer) As String
        Dim conexaoMySQL As New MySqlConnection(Conexao.ConnectionString)

        Dim wrkRetorno As String = "Exclusão realizada com sucesso."

        Try
            'Exclui as imagens relacionadas
            Dim objProduto As Produto = Produto.Busca(pProdutoId)
            If File.Exists(Path.Combine(Config.DiretorioImagens, objProduto.CodigoBarrasImagem)) Then
                File.Delete(Path.Combine(Config.DiretorioImagens, objProduto.CodigoBarrasImagem))
            End If
            '==============================

            Dim cmdMySQL As MySqlCommand = conexaoMySQL.CreateCommand
            cmdMySQL.CommandText = "DELETE FROM Produto WHERE Id = " & pProdutoId
            conexaoMySQL.Open()

            'Realiza o comando
            cmdMySQL.ExecuteNonQuery()

        Catch wrkErro As Exception
            wrkRetorno = "Erro ao excluir produto: " & wrkErro.Message.Replace("""", "").Replace("'", "")
        Finally
            If conexaoMySQL.State.Equals(ConnectionState.Open) Then
                conexaoMySQL.Close()
            End If
        End Try

        Return wrkRetorno
    End Function

    Public Shared Function Salva(ByVal pRequisicao As String) As String
        Dim conexaoMySQL As New MySqlConnection(Conexao.ConnectionString)
        Dim wrkId As Integer = 0
        Dim wrkNome As String = String.Empty
        Dim wrkSubCategoriaId As Integer = 0
        Dim wrkCodigoBarrasImagem As String = String.Empty
        Dim wrkCodigoBarrasImagemArray As String = String.Empty
        Dim wrkCodigoBarras As String = String.Empty
        Dim wrkMarca As String = String.Empty
        Dim wrkUnidade As String = String.Empty
        Dim wrkTamanho As String = String.Empty


        Dim wrkRetorno As String = "Atualização realizada com sucesso."

        Try
            'Obtem as variáveis
            wrkId = CInt(Web.HttpUtility.ParseQueryString(pRequisicao).Item(0))
            wrkNome = Web.HttpUtility.ParseQueryString(pRequisicao).Item("nome")
            wrkSubCategoriaId = CInt(Web.HttpUtility.ParseQueryString(pRequisicao).Item("subcategoriaid"))
            wrkCodigoBarrasImagemArray = Web.HttpUtility.ParseQueryString(pRequisicao).Item("codigobarrasimagemarray")
            wrkCodigoBarrasImagem = Web.HttpUtility.ParseQueryString(pRequisicao).Item("codigobarrasimagem")
            wrkCodigoBarras = Web.HttpUtility.ParseQueryString(pRequisicao).Item("codigobarras")
            wrkMarca = Web.HttpUtility.ParseQueryString(pRequisicao).Item("marca")
            wrkUnidade = Web.HttpUtility.ParseQueryString(pRequisicao).Item("unidade")
            wrkTamanho = Web.HttpUtility.ParseQueryString(pRequisicao).Item("tamanho")

            If Not String.IsNullOrEmpty(wrkCodigoBarrasImagemArray) Then
                'Exclui imagem antiga
                If Not String.IsNullOrEmpty(wrkCodigoBarrasImagem) AndAlso File.Exists(Path.Combine(Config.DiretorioImagens, wrkCodigoBarrasImagem)) Then
                    File.Delete(Path.Combine(Config.DiretorioImagens, wrkCodigoBarrasImagem))
                End If
                '====================

                'Obtem Base 64 da imagem
                wrkCodigoBarrasImagemArray = wrkCodigoBarrasImagemArray.Replace("data:image/jpeg;base64,", "").Replace(" ", "+")
                'Define nome único para ele
                Dim wrkNomeArquivo As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond & ".jpeg"
                'Cria arquivo no diretorio
                File.WriteAllBytes(Path.Combine(Config.DiretorioImagens, wrkNomeArquivo), Convert.FromBase64String(wrkCodigoBarrasImagemArray))

                'Grava nova imagem adicionada
                wrkCodigoBarrasImagem = wrkNomeArquivo
            End If

            Dim wrkSQL As String = String.Empty
            If wrkId.Equals(0) Then
                'Insert
                wrkSQL = "INSERT INTO Produto (nome, subcategoriaid, codigobarrasimagem, codigobarras, marca, unidade, tamanho) " &
                         "Values " &
                         "(@nome, @subcategoriaid, @codigobarrasimagem, @codigobarras, @marca, @unidade, @tamanho)"
            Else
                'Update
                wrkSQL = "UPDATE Produto SET " &
                         "nome = @nome, subcategoriaid = @subcategoriaid, " & IIf(String.IsNullOrEmpty(wrkCodigoBarrasImagem), "", " codigobarrasimagem = @codigobarrasimagem, ").ToString &
                         "codigobarras = @codigobarras, marca = @marca, unidade = @unidade, tamanho = @tamanho " &
                         "WHERE Id = @Id"
            End If

            Dim cmdMySQL As MySqlCommand = conexaoMySQL.CreateCommand
            cmdMySQL.CommandText = wrkSQL
            conexaoMySQL.Open()

            'Preenche os paramestros
            cmdMySQL.Parameters.Add("@nome", MySqlDbType.VarChar).Value = wrkNome
            cmdMySQL.Parameters.Add("@subcategoriaid", MySqlDbType.Int32).Value = wrkSubCategoriaId
            If Not String.IsNullOrEmpty(wrkCodigoBarrasImagem) Then
                cmdMySQL.Parameters.Add("@codigobarrasimagem", MySqlDbType.VarChar).Value = wrkCodigoBarrasImagem
            End If
            cmdMySQL.Parameters.Add("@codigobarras", MySqlDbType.VarChar).Value = wrkCodigoBarras
            cmdMySQL.Parameters.Add("@marca", MySqlDbType.VarChar).Value = wrkMarca
            cmdMySQL.Parameters.Add("@unidade", MySqlDbType.VarChar).Value = wrkUnidade
            cmdMySQL.Parameters.Add("@tamanho", MySqlDbType.VarChar).Value = wrkTamanho
            If wrkId > 0 Then
                cmdMySQL.Parameters.Add("@Id", MySqlDbType.Int32).Value = wrkId
            End If

            'Realiza o comando
            cmdMySQL.ExecuteNonQuery()

        Catch wrkErro As Exception
            wrkRetorno = "Erro ao atualizar produto: " & wrkErro.Message.Replace("""", "").Replace("'", "")
        Finally
            If conexaoMySQL.State.Equals(ConnectionState.Open) Then
                conexaoMySQL.Close()
            End If
        End Try

        Return wrkRetorno
    End Function
End Class
