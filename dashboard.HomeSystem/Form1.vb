Imports System.IO
Imports System.Net

Public Class Form1
    Dim count = 0
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        Const OFF_STATUS = "<!DOCTYPE HTML>" & vbCrLf & "<html>" & vbCrLf & "0</html>" & vbLf
        Const ON_STATUS = "<!DOCTYPE HTML>" & vbCrLf & "<html>" & vbCrLf & "1</html>" & vbLf
        Dim request = WRequest("http://192.168.1.16/RearRoomsHeater/Status", "GET", "")
        If request = ON_STATUS Then
            Label1.Text = "Heater On"
            Label2.Text = "Online"
        ElseIf request = OFF_STATUS Then
            Label1.Text = "Heater Off"
            Label2.Text = "Online"
        Else
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
End Class
