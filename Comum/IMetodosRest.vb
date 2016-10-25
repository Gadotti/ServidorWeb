Imports System.Net.Sockets

Public Interface IMetodosRest
    Function RetornaListaProdutos(ByVal pRequisicao As String) As String
    Function RetornaProduto(ByVal pProdutoId As Integer) As String
    Function RetornaAutoCompleteXML(ByVal pTabela As String, ByVal pCampo As String, Optional ByVal pCaracteresDigitados As String = "") As String
    Function RetornaCategorias() As String
    Function RetornaSubCategorias(ByVal pCategoriaId As Integer, Optional ByRef uppercase As Boolean = False) As String
    Function ProdutoExclui(ByVal pProdutoId As Integer) As String
    Function ProdutoSalva(ByVal pRequisicao As String) As String
End Interface
