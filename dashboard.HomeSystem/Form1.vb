Imports System.IO
Imports System.Net

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Left = Screen.AllScreens(0).Bounds.Right - Width
    End Sub

    Private device0State = -1
    Const sec = 1000
    Const min = 60 * sec
    Const hour = 60 * min
    Const heatTime = 45 * min
    Private heaterTimer As Integer = heatTime

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        Const OFF_STATUS = "<!DOCTYPE HTML>" & vbCrLf & "<html>" & vbCrLf & "0</html>" & vbLf
        Const ON_STATUS = "<!DOCTYPE HTML>" & vbCrLf & "<html>" & vbCrLf & "1</html>" & vbLf
        Dim request = WRequest("http://192.168.1.16/RearRoomsHeater/Status", "GET", "")
        If request = ON_STATUS Then
            If device0State <> 1 Then
                If device0State = -1 Then
                    AppendToLogFile(1)
                Else
                    AppendToLogFile(3)
                End If
            End If
            device0State = 1
            Label1.Text = "Heater On"
            Label2.Text = "Online"
        ElseIf request = OFF_STATUS Then
            If device0State <> 0 Then
                If device0State = -1 Then
                    AppendToLogFile(1)
                Else
                    AppendToLogFile(2)
                End If
            End If
            device0State = 0
            Label1.Text = "Heater Off"
            Label2.Text = "Online"
        Else
            If device0State <> -1 Then
                AppendToLogFile(0)
            End If
            device0State = -1
            Label1.Text = ""
            Label2.Text = "Offline"
        End If
    End Sub

    Function WRequest(URL As String, method As String, POSTdata As String) As String
        Dim responseData As String = ""
        Try
            Dim cookieJar As New Net.CookieContainer()
            Dim hwrequest As Net.HttpWebRequest = Net.WebRequest.Create(URL)
            hwrequest.CookieContainer = cookieJar
            hwrequest.Accept = "*/*"
            hwrequest.AllowAutoRedirect = True
            hwrequest.UserAgent = "http_requester/0.1"
            hwrequest.Timeout = 60000
            hwrequest.Method = method
            If hwrequest.Method = "POST" Then
                hwrequest.ContentType = "application/x-www-form-urlencoded"
                Dim encoding As New Text.ASCIIEncoding() 'Use UTF8Encoding for XML requests
                Dim postByteArray() As Byte = encoding.GetBytes(POSTdata)
                hwrequest.ContentLength = postByteArray.Length
                Dim postStream As IO.Stream = hwrequest.GetRequestStream()
                postStream.Write(postByteArray, 0, postByteArray.Length)
                postStream.Close()
            End If
            Dim hwresponse As Net.HttpWebResponse = hwrequest.GetResponse()
            If hwresponse.StatusCode = Net.HttpStatusCode.OK Then
                Dim responseStream As IO.StreamReader =
                  New IO.StreamReader(hwresponse.GetResponseStream())
                responseData = responseStream.ReadToEnd()
            End If
            hwresponse.Close()
        Catch e As Exception
            responseData = "An error occurred: " & e.Message
        End Try
        Return responseData
    End Function

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label1_Click(sender, e)
    End Sub

    Function AppendToLogFile(logEvent As Integer) As Boolean
        Dim logString As String = Now.ToString("s")
        If logEvent = 0 Then
            logString = logString & vbTab & "Device went offline" & vbNewLine
        ElseIf logEvent = 1 Then
            logString = logString & vbTab & "Device came online" & vbNewLine
        ElseIf logEvent = 2 Then
            logString = logString & vbTab & "Device turned off" & vbNewLine
        ElseIf logEvent = 3 Then
            logString = logString & vbTab & "Device turned on" & vbNewLine
        ElseIf logEvent = 4 Then
            logString = logString & vbTab & "Device timer started" & vbNewLine
        ElseIf logEvent = 5 Then
            logString = logString & vbTab & "Device timer stopped" & vbNewLine
        End If
        My.Computer.FileSystem.WriteAllText("C:\Users\jeeva\OneDrive\Desktop\homeHeater.log", logString, True)
        Return True
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        WRequest("http://192.168.1.16/RearRoomsHeater/On", "GET", "")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        WRequest("http://192.168.1.16/RearRoomsHeater/Off", "GET", "")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Timer2.Interval = 100
        Timer2.Enabled = True
        AppendToLogFile(4)
        Button1_Click(sender, e)
    End Sub

    Private timerAborted As Boolean = False

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick

        If heaterTimer > 0 Then
            heaterTimer -= 100
            Label3.Text = Format(heaterTimer / (60 * 1000), "###.00") & "min"
        Else
            AppendToLogFile(5)
            Button2_Click(sender, e)
            Timer2.Enabled = False
            heaterTimer = heatTime
            Label3.Text = ""
            If Not timerAborted Then
                Beep()
                MsgBox("Hot water is ready")
            Else
                timerAborted = False
            End If
        End If

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        timerAborted = True
        heaterTimer = 0
    End Sub

End Class
