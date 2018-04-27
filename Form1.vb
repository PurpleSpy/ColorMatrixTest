Public Class Form1
    Dim lastimg As Image = Nothing
    Dim crx As New Drawing.Imaging.ColorMatrix()
    Dim loaded As Boolean = False
    Dim lastadustment As Image = Nothing
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.SendToBack()
        Dim channels As String() = {"Red", "Green", "Blue", "Alpha", "Multiplier vs column"}



        For ypos As Integer = 0 To 4
            For xpos As Integer = 0 To 4
                Dim vg As New NumericUpDown
                vg.Name = "matrix" & ypos & xpos

                vg.Anchor = AnchorStyles.Top Or AnchorStyles.Left
                vg.DecimalPlaces = 4
                vg.Increment = 0.01
                vg.Minimum = -2
                vg.Maximum = 2
                vg.Top = vg.PreferredSize.Height * ypos
                vg.Left = vg.PreferredSize.Width * xpos
                vg.Width = vg.PreferredSize.Width
                vg.Height = vg.PreferredSize.Height
                vg.Value = crx.Item(xpos, ypos)
                Panel1.Controls.Add(vg)
                removehandlers(vg)
                AddHandler vg.ValueChanged, AddressOf numupdownch
                AddHandler vg.Enter, AddressOf getnameofnum
            Next
            Dim srip As New Label
            srip.Text = channels(ypos)
            srip.Left = (Panel1.Controls(Panel1.Controls.Count - 1).PreferredSize.Width * 5) + 5
            srip.Top = Panel1.Controls(Panel1.Controls.Count - 1).PreferredSize.Height * ypos + 5
            Panel1.Controls.Add(srip)
        Next
        If My.Application.CommandLineArgs.Count = 1 Then
            If IO.File.Exists(My.Application.CommandLineArgs(0)) Then
                applyImage(My.Application.CommandLineArgs(0))
            Else
                Try
                    Dim gx As New Uri(My.Application.CommandLineArgs(0))
                    Using svp As New Net.WebClient
                        Dim dmem As New IO.MemoryStream(svp.DownloadData(gx.OriginalString))
                        Dim sgp As Image = Image.FromStream(dmem)
                        applyImage(sgp)
                    End Using

                Catch ex As Exception

                End Try
            End If
        Else
            Using stx As New OpenFileDialog
                stx.Filter = "Image file *.png,*.jpg|*.png;*.jpg"
                stx.FilterIndex = 1
                stx.Title = "Open an image for processing"
                If stx.ShowDialog = DialogResult.OK Then
                    Try
                        applyImage(stx.FileName)
                    Catch ex As Exception
                        MessageBox.Show(stx.FileName & " could not be opened")
                        Me.Close()
                        Exit Sub
                    End Try

                Else
                    Me.Close()
                    Exit Sub
                End If
            End Using
        End If
        addhandlers()
        loaded = True
    End Sub
    Sub getnameofnum(sender As Object, e As EventArgs)
        Me.Text = sender.name
    End Sub

    Sub numupdownch(sender As Object, e As EventArgs)
        If Not loaded Or lastimg Is Nothing Then
            Exit Sub
        End If
        Dim drawmatrix As New Drawing.Imaging.ColorMatrix
        removehandlers(sender)
        For ypos As Integer = 0 To 4
            For xpos As Integer = 0 To 4
                Dim rx As NumericUpDown = Panel1.Controls("matrix" & ypos & xpos)
                drawmatrix(xpos, ypos) = rx.Value
            Next
        Next
        Dim bckimg As Image = New Bitmap(lastimg.Width, lastimg.Height)
        Using sce As Drawing.Graphics = Drawing.Graphics.FromImage(bckimg)
            Dim tx As New Imaging.ImageAttributes
            tx.SetColorMatrix(drawmatrix)

            sce.DrawImage(lastimg, New Rectangle(New Point(0, 0), bckimg.Size), 0, 0, bckimg.Width, bckimg.Height, GraphicsUnit.Pixel, tx)
            lastadustment = bckimg.Clone
            PictureBox1.Image = bckimg.Clone
            sce.Dispose()
            bckimg.Dispose()
        End Using
        addhandlers()
    End Sub

    Private Sub Form1_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim cst As String() = e.Data.GetData(DataFormats.FileDrop)

            If cst.Length = 1 Then
                Try
                    Dim cimg As Drawing.Image = Drawing.Image.FromFile(cst(0))
                    cimg.Dispose()
                    e.Effect = DragDropEffects.Copy
                    Exit Sub
                Catch ex As Exception

                End Try

            End If

        End If
        If e.Data.GetDataPresent(DataFormats.Bitmap) Then
            Dim cst As Image = e.Data.GetData(DataFormats.Bitmap)

            If cst IsNot Nothing Then
                Try
                    Dim cimg As Drawing.Image = cst
                    cimg.Dispose()
                    e.Effect = DragDropEffects.Copy
                    Exit Sub
                Catch ex As Exception

                End Try

            End If

        End If
        e.Effect = DragDropEffects.None
    End Sub

    Private Sub Form1_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        If e.Effect = DragDropEffects.Copy Then
            Try

                applyImage(e.Data.GetData(DataFormats.FileDrop)(0))


            Catch ex As Exception

            End Try
        End If
    End Sub



    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.V And e.Modifiers = Keys.Control Then
            If Clipboard.ContainsImage Then
                applyImage(Clipboard.GetImage)
            End If
        End If
    End Sub

    Public Sub applyImage(path As String)

        Dim csa As Image = Image.FromFile(path)
        applyImage(csa)


    End Sub

    Public Sub applyImage(pic As Image)
        lastimg = pic.Clone
        PictureBox1.Image = lastimg.Clone
    End Sub

    Public Sub resetMatrix()
        loaded = False
        Dim drawmatrix As New Drawing.Imaging.ColorMatrix

        For ypos As Integer = 0 To 4
            For xpos As Integer = 0 To 4
                Dim rx As NumericUpDown = Panel1.Controls("matrix" & ypos & xpos)
                rx.Value = drawmatrix(xpos, ypos)
                removehandlers(rx)
            Next


        Next
        loaded = True
        numupdownch(Panel1.Controls("matrix00"), Nothing)
        addhandlers()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        resetMatrix()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If lastimg IsNot Nothing Then
            applyImage(lastimg)
        End If
    End Sub




    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs)
        Dim send As TrackBar = sender
        Dim ctl As NumericUpDown = Me.Controls.Find("Matrix" & send.Tag, True)(0)
        ctl.Value = send.Value / 255

    End Sub

    Private Sub removehandlers(modifed As NumericUpDown)
        For i As Integer = 1 To 9

            Dim ctrack As TrackBar = Me.Controls.Find("Trackbar" & i, True)(0)
            RemoveHandler ctrack.Scroll, AddressOf TrackBar1_Scroll

            If modifed.Name.IndexOf(ctrack.Tag) > -1 Then
                ctrack.Value = modifed.Value * 255
            End If

        Next
    End Sub
    Private Sub addhandlers()
        For i As Integer = 1 To 9

            Dim ctrack As TrackBar = Me.Controls.Find("Trackbar" & i, True)(0)
            AddHandler ctrack.Scroll, AddressOf TrackBar1_Scroll
        Next
    End Sub

    Private Sub PictureBox1_MouseEnter(sender As Object, e As EventArgs) Handles PictureBox1.MouseEnter
        If lastimg IsNot Nothing Then
            sender.cursor = Cursors.Hand
        Else
            sender.cursor = Cursors.No
        End If
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If lastadustment IsNot Nothing Then


            Using srp As New SaveFileDialog
                srp.Filter = "Png *.png|*.png"
                srp.FilterIndex = 1
                If srp.ShowDialog = DialogResult.OK Then
                    Dim myEncoderParameters As New Drawing.Imaging.EncoderParameters(1)
                    Dim myEncoder As System.Drawing.Imaging.Encoder = System.Drawing.Imaging.Encoder.Quality
                    Dim myEncoderParameter As New System.Drawing.Imaging.EncoderParameter(myEncoder, 100&)
                    myEncoderParameters.Param(0) = myEncoderParameter
                    Dim pngencoder As Drawing.Imaging.ImageCodecInfo = GetEncoder(Drawing.Imaging.ImageFormat.Png)
                    lastadustment.Save(srp.FileName, pngencoder, myEncoderParameters)
                End If

            End Using
        End If
    End Sub
    Private Function GetEncoder(ByVal format As Drawing.Imaging.ImageFormat) As Drawing.Imaging.ImageCodecInfo

        Dim codecs As Drawing.Imaging.ImageCodecInfo() = Drawing.Imaging.ImageCodecInfo.GetImageDecoders()

        Dim codec As Drawing.Imaging.ImageCodecInfo
        For Each codec In codecs
            If codec.FormatID = format.Guid Then
                Return codec
            End If
        Next codec
        Return Nothing

    End Function
End Class
