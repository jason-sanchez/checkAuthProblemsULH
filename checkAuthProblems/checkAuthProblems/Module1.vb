Imports System.Configuration
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Net.Mail
Imports System.IO
Module Module1


    Dim errorfilepath As String = System.Configuration.ConfigurationManager.AppSettings("emailerror")
    Dim errorfilename As String = String.Format("CopyError_{0:yyyyMMdd_HH-mm-ss}.txt", Date.Now)
    Dim problemDir As String = System.Configuration.ConfigurationManager.AppSettings("ProblemsDirectory")

    Dim theMessage As MailMessage
    Dim fileCount As Integer = 0
    Dim strMessage As String = ""
    Dim globalError As Boolean = False
    Dim gblLogString As String = ""

    Sub Main()

        Try

            checkLocation()

            If fileCount > 0 Then
                mailIT("Auth Problem", strMessage)
            End If

        Catch ex As Exception
            Dim errorfile = New StreamWriter(errorfilepath & errorfilename, True)
            errorfile.Write("Main Error" & ex.Message & vbCrLf)
            errorfile.Close()
            Exit Sub
        End Try

    End Sub

    Private Sub checkLocation()
        Try
            strMessage = strMessage & vbCrLf
            strMessage = strMessage & "Location Authcode Status as of " & CStr(DateTime.Now) & vbCrLf

            Dim dirfproblems As DirectoryInfo = New DirectoryInfo(problemDir)

            Dim Fproblemfiles As FileInfo() = dirfproblems.GetFiles("*.*")

            strMessage = strMessage & "There is " & CStr(Fproblemfiles.Length) & " file in the backup directory." & vbCrLf

            fileCount = fileCount + Fproblemfiles.Length

        Catch ex As Exception
            Dim errorfile = New StreamWriter(errorfilepath & errorfilename, True)
            errorfile.Write("Main Error" & ex.Message & vbCrLf)
            errorfile.Close()
            Exit Sub
        End Try
    End Sub

    Private Sub mailIT(ByVal subject As String, strMessage As String)


        Dim SMTP As New SmtpClient(System.Configuration.ConfigurationManager.AppSettings("SMTPServer"))
        Dim message As New MailMessage(System.Configuration.ConfigurationManager.AppSettings("FromEmailAddress"), System.Configuration.ConfigurationManager.AppSettings("toEmailAddress"))
        Dim otherRecipients As String() = System.Configuration.ConfigurationManager.AppSettings("otherEmailAddresses").ToString().Split(",")
        Dim username As String = System.Configuration.ConfigurationManager.AppSettings("username")
        Dim password As String = System.Configuration.ConfigurationManager.AppSettings("password")
        Dim port As Integer = CInt(System.Configuration.ConfigurationManager.AppSettings("port"))
        SMTP.EnableSsl = True

        SMTP.Credentials = New Net.NetworkCredential(username, password)
        SMTP.Port = port

        message.Subject = subject
        message.Body = strMessage
        message.To.Clear()
        For Each Recipient As String In otherRecipients
            message.To.Add(New MailAddress(Recipient))
        Next
        Try
            SMTP.Send(message)
        Catch ex As Exception
            Dim errorfile = New StreamWriter(errorfilepath & errorfilename, True)
            errorfile.Write("Authcode Problem Email error" & ex.Message & vbCrLf)
            errorfile.Close()
        End Try



        'Dim emailSender As New SmtpClient(clientstring)

        '' ----- Build the content details.
        'Try
        '    theMessage = New MailMessage
        '    theMessage.From = New MailAddress(strServer)
        '    theMessage.To.Add(strToAddress)
        '    theMessage.CC.Add(strcc)
        '    theMessage.Subject = " Authcode Emailer Message " & CStr(Now)
        '    theMessage.Body = strMessage

        '    ' ----- Fill in the details and send.
        '    emailSender.Send(theMessage)
        'Catch ex As Exception
        '    Dim errorfile = New StreamWriter(errorfilepath & errorfilename, True)
        '    errorfile.Write("Main Error" & ex.Message & vbCrLf)
        '    errorfile.Close()
        '    Exit Sub
        'Finally
        'End Try

    End Sub

End Module
