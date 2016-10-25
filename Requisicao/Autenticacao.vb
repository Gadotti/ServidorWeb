Imports System.IO
Imports System.Xml
Imports Comum

Public Class Autenticacao
    Public Property EnvioRequisicao As Comum.IRequisicao

    Public Sub New(ByVal pRequisicao As Comum.IRequisicao)
        EnvioRequisicao = pRequisicao
    End Sub

    'Public Function ValidaUsuario() As Boolean
    '    'Verifica se possui parametros enviados
    '    If EnvioRequisicao.ListaParametros Is Nothing Then
    '        Return False
    '    End If

    '    'Obtem usuario/senha e valida
    '    Dim wrkUsuario As String = EnvioRequisicao.ListaParametros.Find(Function(T) T.Parametro.ToLower.Equals("usuario")).Valor
    '    Dim wrkSenha As String = EnvioRequisicao.ListaParametros.Find(Function(T) T.Parametro.ToLower.Equals("senha")).Valor

    '    Return True
    'End Function

    Public Shared Function Autentica(ByVal pUsuario As String, ByVal pSenha As String) As Boolean
        Dim wrkXmlDocument As New XmlDocument
        wrkXmlDocument.Load(Config.ArquivoUsuarios)

        Dim wrkUsuario As String = String.Empty
        Dim wrkSenha As String = String.Empty

        For Each wrkNode In wrkXmlDocument.Item("usuarios").GetElementsByTagName("usuario")
            If wrkNode.Item("nome").InnerText.ToLower.Equals(pUsuario) Then
                wrkUsuario = pUsuario.ToLower.Trim
                wrkSenha = wrkNode.Item("senha").InnerText.ToString.ToLower.Trim
            End If
        Next

        If wrkUsuario.Equals(pUsuario.ToLower) AndAlso
            wrkSenha.Equals(pSenha.ToLower) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function NovoUsuario(ByVal pUsuario As String, ByVal pSenha As String) As Boolean
        Try
            Dim wrkXmlDocument As New XmlDocument
            wrkXmlDocument.Load(Config.ArquivoUsuarios)

            'Verifica se já existe usuário com este login cadastrado
            For Each wrkNode In wrkXmlDocument.Item("usuarios").GetElementsByTagName("usuario")
                If wrkNode.Item("nome").InnerText.ToLower.Equals(pUsuario) Then
                    Return False 'Já existe usuário com este mesmo nome
                End If
            Next
            '=======================================================

            Dim xmlElemUsuario As XmlElement = wrkXmlDocument.CreateElement("usuario")
            Dim xmlElemNome As XmlElement = wrkXmlDocument.CreateElement("nome")
            xmlElemNome.InnerText = pUsuario

            Dim xmlElemSenha As XmlElement = wrkXmlDocument.CreateElement("senha")
            xmlElemSenha.InnerText = pSenha

            xmlElemUsuario.AppendChild(xmlElemNome)
            xmlElemUsuario.AppendChild(xmlElemSenha)

            wrkXmlDocument.Item("usuarios").AppendChild(xmlElemUsuario)

            wrkXmlDocument.Save(Config.ArquivoUsuarios)

        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function



End Class
