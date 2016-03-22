Imports System.Collections.ObjectModel

Class MainWindow
    Public Spectra As New ObservableCollection(Of Spectrum)

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.DataContext = Spectra

        'Load example spectra
        'Dim x, y, z As New Spectrum
        'x.Data = TestDataR()
        'y.Data = TestDataG()
        'z.Data = TestDataB()
        'Spectra.Add(y)
        'Spectra.Add(x)
        'Spectra.Add(z)

        LoadSpecta()

    End Sub

    Private Sub LoadSpecta()
        Dim FileLineList As List(Of String) = LoadFile(My.Settings.SpectraLocation)

        Dim SpectraFirstDataLine As Integer

        Dim SpectraCount As Integer = FileLineList(0).Split({CChar(",")}).Count - 1

        Dim FileMatrix(FileLineList.Count - 1, SpectraCount) As String

        For RowCounter As Integer = 0 To FileLineList.Count - 1
            Dim LineElements As String() = FileLineList(RowCounter).Split({CChar(",")})

            For ColumnCounter As Integer = 0 To SpectraCount
                FileMatrix(RowCounter, ColumnCounter) = LineElements(ColumnCounter)
            Next
        Next

        Do
            Dim RowCounter As Integer

            If RowCounter > FileMatrix.GetUpperBound(0) Then
                Exit Do
            ElseIf IsNumeric(FileMatrix(RowCounter, 0)) Then
                SpectraFirstDataLine = RowCounter
                Exit Do
            End If

            RowCounter += 1
        Loop


        For ColumnCounter As Integer = 1 To SpectraCount

            Dim NewSpectrum As New Spectrum
            NewSpectrum.Name = FileMatrix(0, ColumnCounter)

            Dim TypeString As String = FileMatrix(1, ColumnCounter)

            Select Case TypeString
                Case "Ex", "Ab"
                    NewSpectrum.Type = Spectrum.SpectrumTypes.Ex
                Case "Em"
                    NewSpectrum.Type = Spectrum.SpectrumTypes.Em
            End Select

            For RowCounter As Integer = SpectraFirstDataLine To FileLineList.Count - 1
                Dim YString As String
                Dim X, Y As Double

                YString = FileMatrix(RowCounter, ColumnCounter)

                If YString = "" Then Continue For

                X = CDbl(FileMatrix(RowCounter, 0))
                Y = CDbl(YString)

                Dim DataPoint As New PlotPoint(X, Y)

                NewSpectrum.Data.Add(DataPoint)

            Next

            Spectra.Add(NewSpectrum)
        Next

    End Sub

    Public Function IsNumeric(ByVal Input As String) As Boolean
        Dim Output As Double
        Return Double.TryParse(Input, Output)
    End Function

    Private Function TestDataR() As List(Of PlotPoint)
        Dim FileLineList As List(Of String) = LoadFile(My.Settings.CMFLocation)
        Dim Points As New List(Of PlotPoint)

        For RowCounter As Integer = 1 To FileLineList.Count - 1
            Dim LineElements(0 To 12) As String
            Dim X, Y As Double

            LineElements = FileLineList(RowCounter).Split({CChar(",")})

            X = CDbl(LineElements(0))
            Y = CDbl(LineElements(1))

            Dim DataPoint As New PlotPoint(X, Y)

            Points.Add(DataPoint)
        Next

        Return Points

    End Function

    Private Function TestDataG() As List(Of PlotPoint)
        Dim FileLineList As List(Of String) = LoadFile(My.Settings.CMFLocation)
        Dim Points As New List(Of PlotPoint)

        For RowCounter As Integer = 1 To FileLineList.Count - 1
            Dim LineElements(0 To 12) As String
            Dim X, Y As Double

            LineElements = FileLineList(RowCounter).Split({CChar(",")})

            X = CDbl(LineElements(0))
            Y = CDbl(LineElements(2))

            Dim DataPoint As New PlotPoint(X, Y)

            Points.Add(DataPoint)
        Next

        Return Points

    End Function

    Private Function TestDataB() As List(Of PlotPoint)
        Dim FileLineList As List(Of String) = LoadFile(My.Settings.CMFLocation)
        Dim Points As New List(Of PlotPoint)

        For RowCounter As Integer = 1 To FileLineList.Count - 1
            Dim LineElements(0 To 12) As String
            Dim X, Y As Double

            LineElements = FileLineList(RowCounter).Split({CChar(",")})

            X = CDbl(LineElements(0))
            Y = CDbl(LineElements(3))

            Dim DataPoint As New PlotPoint(X, Y)

            Points.Add(DataPoint)
        Next

        Return Points

    End Function

    Public Shared Function LoadFile(ByVal FileLocation As String) As List(Of String)
        Dim OpenFileStream As New IO.FileStream(FileLocation, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
        Dim OpenFileReader As New IO.StreamReader(OpenFileStream)
        Dim FileLineList As New List(Of String)

        Do
            FileLineList.Add(OpenFileReader.ReadLine)
        Loop Until OpenFileReader.EndOfStream

        OpenFileReader.Close()
        OpenFileStream.Close()

        Return FileLineList
    End Function
End Class
