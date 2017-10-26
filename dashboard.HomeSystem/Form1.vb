Public Class Form1
    Dim Count
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        Label1.Text = Count
        Count = Count + 1
    End Sub
End Class
