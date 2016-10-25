Imports System.Xml
Imports Comum

Module Module1

    Sub Main()
        'Dim wrkItens As List(Of Mapa) = Mapa.GetMapas

        'For Each wrkMapa In wrkItens
        '    Debug.Print("Padrão '{0}', Classe '{1}'", wrkMapa.Padrao, wrkMapa.Classe)
        'Next

        teste("josesilva", "e423342")

    End Sub

    Private Function teste(ByVal pUsuario As String, ByVal pSenha As String) As Boolean
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
    End Function

End Module
