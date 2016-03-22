Public Class Spectrum
    Public Property Name As String
    Public Property Type As SpectrumTypes
    Public Property Data As New List(Of PlotPoint)

    Enum SpectrumTypes As Integer
        Ex
        Em
    End Enum

    Public Function Clone() As Spectrum
        Dim ClonedSpectrum As Spectrum = DirectCast(MemberwiseClone(), Spectrum)

        Dim ClonedData As New List(Of PlotPoint)

        For Each Item As PlotPoint In _Data
            ClonedData.Add(Item.Clone)
        Next

        ClonedSpectrum._Data = ClonedData

        Return ClonedSpectrum
    End Function

End Class

Public Class Filter
    Public Property Type As FilterTypes
    Public Property ShortWavelength As Integer
    Public Property LongWavelength As Integer

    'Will want to allow filter transmission curves to be used later. For now, using cut-offs is fine

    Public Enum FilterTypes As Integer
        Bandpass
        Longpass
        Shortpass
    End Enum
End Class