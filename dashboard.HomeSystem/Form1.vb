Imports System.IO
Imports System.Net

Public Class Form1
    Dim device0State = -1
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        Const OFF_STATUS = "<!DOCTYPE HTML>" & vbCrLf & "<html>" & vbCrLf & "0</html>" & vbLf
        Const ON_STATUS = "<!DOCTYPE HTML>" & vbCrLf & "<html>" & vbCrLf & "1</html>" & vbLf
        Dim request = WRequest("http://192.168.1.16/RearRoomsHeater/Status", "GET", "")
        If request = ON_STATUS Then
            If device0State <> 1 Then
                If device0State = -1 Then
                    AppendToLogFile(1)
                End If
                AppendToLogFile(3)
            End If
            device0State = 1
            Label1.Text = "Heater On"
            Label2.Text = "Online"
        ElseIf request = OFF_STATUS Then
            If device0State <> 0 Then
                If device0State = -1 Then
                    AppendToLogFile(1)
                End If
                AppendToLogFile(2)
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
        End If
        My.Computer.FileSystem.WriteAllText("C:\Users\jeeva\OneDrive\Desktop\homeHeater.log", logString, True)
        Return True
    End Function

End Class
