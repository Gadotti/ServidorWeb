Imports System.IO
Imports System.Xml
Imports System.Xml.Schema

Public Class frmPrincipal

    Private isValid As Boolean = True

    Private Sub btnValidar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidar.Click
        Try
            Windows.Forms.Cursor.Current = Cursors.WaitCursor

            Dim wrkConteudoParaXML As String
            Dim wrkArquivo As String = txtArquivo.Text

            'Limpa campo de texto
            txtMensagens.Text = String.Empty

            'Verifica se o arquivo existe
            If File.Exists(wrkArquivo) Then
                'Carrega o arquivo em modo XML
                Dim wrkDocumentoXML As New XmlDocument

                'Validar como XML
                '===================================================================
                Dim wrkConteudo As String = File.ReadAllText(wrkArquivo)
                Try
                    'Retirar a linha do DOCType
                    If wrkConteudo.ToUpper.IndexOf("<!DOCTYPE") >= 0 Then
                        wrkConteudoParaXML = wrkConteudo.Remove(wrkConteudo.IndexOf("<!DOCTYPE"), _
                                                          (wrkConteudo.IndexOf(">", wrkConteudo.IndexOf("<!DOCTYPE")) - wrkConteudo.IndexOf("<!DOCTYPE")) + 1)
                    Else
                        wrkConteudoParaXML = wrkConteudo
                    End If
                    '==========================

                    'Carrega e valida em formato XML
                    wrkDocumentoXML.LoadXml("<?xml version=""1.0"" encoding=""ISO-8859-1""?>" & wrkConteudoParaXML)
                    txtMensagens.Text = txtMensagens.Text & "Formato para XML OK." & Environment.NewLine
                    '================================

                    isValid = True

                Catch wrkErro As XmlException
                    'Documento erro encontrado
                    txtMensagens.Text = txtMensagens.Text & "Formato para XML incorreto. Erro: " & wrkErro.Message & Environment.NewLine
                    isValid = False
                End Try
                '===============================================================

                'Valida de acordo com o TDT específico
                If isValid Then
                    Dim wrkXmlReader As New XmlTextReader(wrkArquivo)
                    Dim wrkValidador As New XmlValidatingReader(wrkXmlReader)
                    wrkValidador.ValidationType = ValidationType.DTD

                    AddHandler wrkValidador.ValidationEventHandler, AddressOf MyValidationEventHandler

                    While wrkValidador.Read()
                        ' Could add code here to process the content.
                    End While
                    wrkValidador.Close()

                    ' Check whether the document is valid or invalid.
                    If isValid Then
                        txtMensagens.Text = "Document is valid"
                    Else
                        txtMensagens.Text = "Document is invalid"
                    End If
                End If

            Else
                MsgBox("Arquivo DTD não encontrado!", MsgBoxStyle.Critical)
            End If
        Catch wrkErro As Exception
            MsgBox(wrkErro.Message, MsgBoxStyle.Critical)
        Finally
            Windows.Forms.Cursor.Current = Cursors.Default
        End Try
    End Sub

    Public Sub MyValidationEventHandler(ByVal sender As Object, ByVal args As ValidationEventArgs)
        isValid = False
        txtMensagens.Text = "Validation event" & vbCrLf & args.Message
    End Sub


End Class
