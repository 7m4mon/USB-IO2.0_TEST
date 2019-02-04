Imports System.Math


Public Class Form1

    'コンピュータ名
    '"."はローカルコンピュータを表す
    'コンピュータ名は省略可能（省略時は"."）
    Dim machineName As String = "."
    'カテゴリ名
    Dim categoryName As String = "Processor"
    'カウンタ名
    Dim counterName As String = "% Processor Time"
    'インスタンス名
    Dim instanceName As String = "_Total"
    '移動平均を取って表示をなめらかにする。
    Dim lastUseRate As Single = 0

    'PerformanceCounterオブジェクトの作成
    Dim pc As New System.Diagnostics.PerformanceCounter(
            categoryName, counterName, instanceName, machineName)

    'USBIOに接続されているバーLEDの状態（０～１０個表示）
    '&H0, &H1, &H3, &H7, &HF, &H1F, &H3F, &H7F, &HFF, &H1FF, &H3FF のビット逆順
    Dim ledLevel As Integer() = {
        &H0, &H200, &H300, &H380, &H3C0, &H3E0, &H3F0, &H3F8, &H3FC, &H3FE, &H3FF
    }

    ''タイマ割り込みでCPU使用率を取得し、ProgressBar1とUSB-IOに表示
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        Dim nowUseRate, aveUseRate As Single
        nowUseRate = pc.NextValue()
        aveUseRate = (nowUseRate + lastUseRate) / 2   '移動平均をとって動きをなめらかにする。
        lastUseRate = nowUseRate
        '計算された値を取得し、表示する
        Label1.Text = aveUseRate.ToString("0.00")

        Dim ledNum As Integer
        If CheckBox1.Checked Then
            ledNum = Math.Floor(5.5 * Math.Log10((aveUseRate + 0.001) / 100))  'logを取り、-10~0の範囲にする。小数点以下切り捨て
            ledNum += 11
        Else
            ledNum = aveUseRate / 5
        End If
        '0~10の範囲にする。
        ledNum = If(ledNum < 0, 0, ledNum)
        ledNum = If(ledNum > 10, 10, ledNum)
        ProgressBar1.Value = ledNum

        'USB-IOが接続されているときはLEDに表示。
        If USBIO.uio_find() = 0 Then
            Label4.Text = "接続OK"
            Dim outData(8) As Byte                  '宣言時に0で初期化される
            outData(0) = 1                          'Port 1
            outData(1) = ledLevel(ledNum) And &HFF  '下位8bit
            outData(2) = 2                          'Port 2
            outData(3) = ledLevel(ledNum) >> 8      '上位2bit
            USBIO.uio2_out(outData(0))
        Else
            Label4.Text = "接続NG"
        End If
    End Sub

    '' タイマーのインターバルを変更する
    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        Timer1.Interval = NumericUpDown1.Value
    End Sub

#Region "Settings"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NumericUpDown1.Value = My.Settings.interval
        CheckBox1.Checked = My.Settings.logScale
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        My.Settings.interval = NumericUpDown1.Value
        My.Settings.logScale = CheckBox1.Checked
    End Sub
#End Region
End Class
