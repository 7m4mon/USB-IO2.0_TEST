Public Class Form1
    ''' <summary>
    ''' キーが押されたらキーコードを取得して表示
    ''' </summary>
    Private Sub Form1_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress
        Dim key As String
        key = e.KeyChar.ToString
        Label1.Text = key
        Dim asc_code As Integer
        asc_code = Asc(key)
        Label2.Text = asc_code
        Label3.Text = Hex(asc_code)
        WriteUsbIo2(1, CByte(asc_code))
    End Sub

    ''' <summary>
    ''' USB-IO2.0の出力を設定します。
    ''' </summary>
    ''' <param name="port">出力ポート(1～2)</param>
    ''' <param name="code">出力データ(0～255)</param>
    Sub WriteUsbIo2(ByVal port As Byte, ByVal code As Byte)
        Dim outData(8) As Byte     '宣言時にで0で初期化される
        outData(0) = port
        outData(1) = code
        Dim check As Integer
        check = USBIO.uio_find()
        If check = 0 Then
            Try
                check = USBIO.uio2_out(outData(0))
                If check <> 0 Then
                    MsgBox("書き込みできませんでした", MsgBoxStyle.Critical)
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        Else
            MsgBox("1:必要なドライバが無い、2:USB-IOが繋がってない" & vbCrLf & check, MsgBoxStyle.Critical)
        End If
    End Sub

End Class
