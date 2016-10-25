Imports Comum
Public Class MetodosRest
    Implements IMetodosRest

    Public Function RetornaAutoCompleteXML(ByVal pTabela As String, ByVal pCampo As String, Optional ByVal pCaracteresDigitados As String = "") As String Implements Comum.IMetodosRest.RetornaAutoCompleteXML
        Dim wrkArrayLista As ArrayList = Nothing
        'Select Case pTabela.ToLower
        '    Case "produto"
        wrkArrayLista = Produto.BuscaDistinct(pCampo, pCaracteresDigitados)
        '    Case "categoria"
        '    Case "subcategoria"
        'End Select

        Dim xml As New Text.StringBuilder
        xml.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
        xml.Append("<items>")

        If wrkArrayLista IsNot Nothing Then
            For Each item As String In wrkArrayLista
                xml.Append(String.Format("<item value=""{0}"" />", item))
            Next
        End If
        xml.Append("</items>")

        Return xml.ToString
    End Function

    Public Function RetornaCategorias() As String Implements Comum.IMetodosRest.RetornaCategorias
        Dim objListaCategoria As List(Of Categoria) = Categoria.Busca()

        Dim xml As New Text.StringBuilder
        xml.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
        xml.Append("<items>")

        If objListaCategoria IsNot Nothing OrElse Not objListaCategoria.Count.Equals(0) Then
            For Each item As Categoria In objListaCategoria
                xml.Append(String.Format("<item value=""{0}"">{1}</item>", item.Id, item.Descricao))
            Next
        End If
        xml.Append("</items>")

        Return xml.ToString
    End Function

    Public Function RetornaListaProdutos(ByVal pRequisicao As String) As String Implements Comum.IMetodosRest.RetornaListaProdutos
        Dim objListaProduto As List(Of Produto) = Produto.Busca(pRequisicao)

        Dim xml As New Text.StringBuilder
        xml.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
        xml.Append("<items>")

        If objListaProduto IsNot Nothing OrElse Not objListaProduto.Count.Equals(0) Then
            For Each item As Produto In objListaProduto
                xml.Append("<item>")
                xml.Append(String.Format("<Id>{0}</Id>", item.Id))
                xml.Append(String.Format("<Nome>{0}</Nome>", item.Nome))
                xml.Append(String.Format("<CategoriaId>{0}</CategoriaId>", item.CategoriaId))
                xml.Append(String.Format("<CategoriaDescricao>{0}</CategoriaDescricao>", item.CategoriaDescricao))
                xml.Append(String.Format("<SubCategoriaId>{0}</SubCategoriaId>", item.SubCategoriaId))
                xml.Append(String.Format("<SubCategoriaDescricao>{0}</SubCategoriaDescricao>", item.SubCategoriaObj.Descricao))
                xml.Append(String.Format("<CodigoBarrasImagem>{0}</CodigoBarrasImagem>", item.CodigoBarrasImagem))
                xml.Append(String.Format("<CodigoBarras>{0}</CodigoBarras>", item.CodigoBarras))
                xml.Append(String.Format("<Marca>{0}</Marca>", item.Marca))
                xml.Append(String.Format("<Unidade>{0}</Unidade>", item.Unidade))
                xml.Append(String.Format("<Tamanho>{0}</Tamanho>", item.Tamanho))
                xml.Append("</item>")
            Next
        End If
        xml.Append("</items> ")

        Return xml.ToString
    End Function

    Public Function RetornaProduto(ByVal pProdutoId As Integer) As String Implements Comum.IMetodosRest.RetornaProduto
        Dim item As Produto = Produto.Busca(pProdutoId)

        Dim xml As New Text.StringBuilder
        xml.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
        xml.Append("<items>")

        xml.Append("<item>")
        xml.Append(String.Format("<Id>{0}</Id>", item.Id))
        xml.Append(String.Format("<Nome>{0}</Nome>", item.Nome))
        xml.Append(String.Format("<CategoriaId>{0}</CategoriaId>", item.CategoriaId))
        xml.Append(String.Format("<CategoriaDescricao>{0}</CategoriaDescricao>", item.CategoriaDescricao))
        xml.Append(String.Format("<SubCategoriaId>{0}</SubCategoriaId>", item.SubCategoriaId))
        xml.Append(String.Format("<SubCategoriaDescricao>{0}</SubCategoriaDescricao>", item.SubCategoriaObj.Descricao))
        xml.Append(String.Format("<CodigoBarrasImagem>{0}</CodigoBarrasImagem>", item.CodigoBarrasImagem))
        xml.Append(String.Format("<CodigoBarras>{0}</CodigoBarras>", item.CodigoBarras))
        xml.Append(String.Format("<Marca>{0}</Marca>", item.Marca))
        xml.Append(String.Format("<Unidade>{0}</Unidade>", item.Unidade))
        xml.Append(String.Format("<Tamanho>{0}</Tamanho>", item.Tamanho))
        xml.Append("</item>")

        xml.Append("</items>")

        Return xml.ToString
    End Function

    Public Function RetornaSubCategorias(ByVal pCategoriaId As Integer, Optional ByRef uppercase As Boolean = False) As String Implements Comum.IMetodosRest.RetornaSubCategorias
        Dim objListaSubCategoria As List(Of SubCategoria) = SubCategoria.Busca(pCategoriaId)

        Dim xml As New Text.StringBuilder
        xml.Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
        xml.Append("<items>")

        If objListaSubCategoria IsNot Nothing OrElse Not objListaSubCategoria.Count.Equals(0) Then
            For Each item As SubCategoria In objListaSubCategoria
                If uppercase Then
                    xml.Append(String.Format("<item value=""{0}"">{1}</item>", item.Id, item.Descricao.ToUpper))
                Else
                    xml.Append(String.Format("<item value=""{0}"">{1}</item>", item.Id, item.Descricao))
                End If

            Next
        End If
        xml.Append("</items>")

        Return xml.ToString
    End Function

    Public Function ProdutoExclui(ByVal pProdutoId As Integer) As String Implements Comum.IMetodosRest.ProdutoExclui
        Return Produto.Exclui(pProdutoId)
    End Function

    Public Function ProdutoSalva(ByVal pRequisicao As String) As String Implements Comum.IMetodosRest.ProdutoSalva
        Return Produto.Salva(pRequisicao)
    End Function
End Class
