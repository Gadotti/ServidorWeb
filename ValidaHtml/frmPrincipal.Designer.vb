<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPrincipal
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnValidar = New System.Windows.Forms.Button
        Me.txtArquivo = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtMensagens = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'btnValidar
        '
        Me.btnValidar.Location = New System.Drawing.Point(154, 38)
        Me.btnValidar.Name = "btnValidar"
        Me.btnValidar.Size = New System.Drawing.Size(168, 23)
        Me.btnValidar.TabIndex = 0
        Me.btnValidar.Text = "Validar"
        Me.btnValidar.UseVisualStyleBackColor = True
        '
        'txtArquivo
        '
        Me.txtArquivo.Location = New System.Drawing.Point(126, 12)
        Me.txtArquivo.Name = "txtArquivo"
        Me.txtArquivo.Size = New System.Drawing.Size(341, 20)
        Me.txtArquivo.TabIndex = 1
        Me.txtArquivo.Text = ""
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(21, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(99, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Caminho + Arquivo:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 85)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(62, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Mensagens"
        '
        'txtMensagens
        '
        Me.txtMensagens.Location = New System.Drawing.Point(13, 101)
        Me.txtMensagens.Multiline = True
        Me.txtMensagens.Name = "txtMensagens"
        Me.txtMensagens.Size = New System.Drawing.Size(477, 175)
        Me.txtMensagens.TabIndex = 4
        '
        'frmPrincipal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(502, 288)
        Me.Controls.Add(Me.txtMensagens)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtArquivo)
        Me.Controls.Add(Me.btnValidar)
        Me.Name = "frmPrincipal"
        Me.Text = "Valida HTML"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnValidar As System.Windows.Forms.Button
    Friend WithEvents txtArquivo As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtMensagens As System.Windows.Forms.TextBox

End Class
