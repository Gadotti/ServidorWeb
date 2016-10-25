Public Interface IRequisicao

    Property RequisicaoString As String
    Property ListaParametros As List(Of HttpParametros)
    Property IsDebugger As Boolean

    Function GetMetodo() As Metodo
    Function GetTipoConteudo() As TipoConteudo
    Function GetParametros() As List(Of HttpParametros)
    Function GetValor(ByVal pParametro As String) As String
    Function GetTotalArquivos() As Integer
    Function GetArquivo(ByVal pIndex As Integer) As Byte()
    Function GetCookies() As List(Of Cookie)
    Function GetSession() As Sessao

End Interface
