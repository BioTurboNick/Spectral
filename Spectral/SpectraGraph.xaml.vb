Imports System.Collections.ObjectModel

Public Class SpectraGraph
    Inherits UserControl

#Region "Dependency Properties"

    Public Shared ReadOnly StartWavelengthProperty As DependencyProperty = DependencyProperty.Register("StartWavelength", GetType(Double), GetType(SpectraGraph), New FrameworkPropertyMetadata(0.0))
    Public Shared ReadOnly EndWavelengthProperty As DependencyProperty = DependencyProperty.Register("EndWavelength", GetType(Double), GetType(SpectraGraph), New FrameworkPropertyMetadata(1000.0))
    Public Shared ReadOnly WavelengthStepProperty As DependencyProperty = DependencyProperty.Register("WavelengthStep", GetType(Double), GetType(SpectraGraph), New FrameworkPropertyMetadata(1.0))
    Public Shared ReadOnly ActiveLaserProperty As DependencyProperty = DependencyProperty.Register("ActiveLaser", GetType(Integer), GetType(SpectraGraph), New FrameworkPropertyMetadata(0))
    Public Shared ReadOnly BrightnessProperty As DependencyProperty = DependencyProperty.Register("Brightness", GetType(Double), GetType(SpectraGraph), New FrameworkPropertyMetadata(1.0))
    Public Shared ReadOnly MajorTickProperty As DependencyProperty = DependencyProperty.Register("MajorTick", GetType(Double), GetType(SpectraGraph), New FrameworkPropertyMetadata(0.0))
    Public Shared ReadOnly MinorTickProperty As DependencyProperty = DependencyProperty.Register("MinorTick", GetType(Double), GetType(SpectraGraph), New FrameworkPropertyMetadata(0.0))
    Public Shared ReadOnly EmissionSpectraProperty As DependencyProperty = DependencyProperty.Register("EmissionSpectra", GetType(ObservableCollection(Of Spectrum)), GetType(SpectraGraph), New FrameworkPropertyMetadata(New ObservableCollection(Of Spectrum)))
    Public Shared ReadOnly ExcitationSpectraProperty As DependencyProperty = DependencyProperty.Register("ExcitationSpectra", GetType(ObservableCollection(Of Spectrum)), GetType(SpectraGraph), New FrameworkPropertyMetadata(New ObservableCollection(Of Spectrum)))

    Public Property StartWavelength As Double
        Get
            Return CDbl(MyBase.GetValue(StartWavelengthProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(StartWavelengthProperty, value)
        End Set
    End Property

    Public Property EndWavelength As Double
        Get
            Return CDbl(MyBase.GetValue(EndWavelengthProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(EndWavelengthProperty, value)
        End Set
    End Property

    Public Property WavelengthStep As Double
        Get
            Return CDbl(MyBase.GetValue(WavelengthStepProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(WavelengthStepProperty, value)
        End Set
    End Property

    Public Property ActiveLaser As Integer
        Get
            Return CInt(MyBase.GetValue(ActiveLaserProperty))
        End Get
        Set(ByVal value As Integer)
            MyBase.SetValue(ActiveLaserProperty, value)
        End Set
    End Property

    Public Property Brightness As Double
        Get
            Return CDbl(MyBase.GetValue(BrightnessProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(BrightnessProperty, value)
        End Set
    End Property

    Public Property MajorTick As Double
        Get
            Return CDbl(MyBase.GetValue(MajorTickProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(MajorTickProperty, value)
        End Set
    End Property

    Public Property MinorTick As Double
        Get
            Return CDbl(MyBase.GetValue(MinorTickProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(MinorTickProperty, value)
        End Set
    End Property

    Public Property EmissionSpectra As ObservableCollection(Of Spectrum)
        Get
            Return DirectCast(MyBase.GetValue(EmissionSpectraProperty), ObservableCollection(Of Spectrum))
        End Get
        Set(ByVal value As ObservableCollection(Of Spectrum))
            MyBase.SetValue(EmissionSpectraProperty, value)
        End Set
    End Property

    Public Property ExcitationSpectra As ObservableCollection(Of Spectrum)
        Get
            Return DirectCast(MyBase.GetValue(ExcitationSpectraProperty), ObservableCollection(Of Spectrum))
        End Get
        Set(ByVal value As ObservableCollection(Of Spectrum))
            MyBase.SetValue(ExcitationSpectraProperty, value)
        End Set
    End Property

    Public Property Filters As New List(Of Filter)

#End Region

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub SpectraGraph_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Dim ColorList As List(Of Color) = ColorLogic.GenerateRGBColorList(StartWavelength, EndWavelength, WavelengthStep, Brightness)

        For Each ColorItem As Color In ColorList
            Dim NewGradientStop As New GradientStop
            Dim Counter As Integer

            NewGradientStop.Color = ColorItem
            NewGradientStop.Offset = Counter * WavelengthStep / (EndWavelength - StartWavelength)

            SpectrumBackground.GradientStops.Add(NewGradientStop)

            Counter += 1
        Next
        'Should hook the above up to a PropertyChangedCallback for StartWavelength, EndWavelength, WavelengthStep, and Brightness

        If DataContext IsNot Nothing Then
            Dim TempEmissionSpectra As New ObservableCollection(Of Spectrum)
            Dim TempExcitationSpectra As New ObservableCollection(Of Spectrum)

            For Each Spectrum As Spectrum In DirectCast(DataContext, ObservableCollection(Of Spectrum))
                Dim x As New Spectrum
                x = Spectrum.Clone

                If Spectrum.Type = Spectrum.SpectrumTypes.Em Then
                    TempEmissionSpectra.Add(x)
                ElseIf Spectrum.Type = Spectrum.SpectrumTypes.Ex Then
                    TempExcitationSpectra.Add(x)
                End If
            Next

            EmissionSpectra = TempEmissionSpectra
            ExcitationSpectra = TempExcitationSpectra

        End If

        Select Case ActiveLaser
            Case 405
                'Dim x As New Filter
                'x.Type = Filter.FilterTypes.Bandpass
                'x.ShortWavelength = 420
                'x.LongWavelength = 480
                'Filters.Add(x)

                'Current DAPI filter
                Dim x As New Filter
                x.Type = Filter.FilterTypes.Bandpass
                x.ShortWavelength = 475
                x.LongWavelength = 525
                Filters.Add(x)

                Dim y As New Filter
                y.Type = Filter.FilterTypes.Shortpass
                y.LongWavelength = 490
                Filters.Add(y)

            Case 458
                Dim x As New Filter
                x.Type = Filter.FilterTypes.Bandpass
                x.ShortWavelength = 475
                x.LongWavelength = 525
                Filters.Add(x)

                Dim y As New Filter
                y.Type = Filter.FilterTypes.Shortpass
                y.LongWavelength = 515
                Filters.Add(y)

                'Case 477
            Case 488
                Dim x As New Filter
                x.Type = Filter.FilterTypes.Longpass
                x.ShortWavelength = 475
                Filters.Add(x)

                'Dim y As New Filter
                'y.Type = Filter.FilterTypes.Longpass
                'y.ShortWavelength = 490
                'Filters.Add(y)

                Dim z As New Filter
                z.Type = Filter.FilterTypes.Shortpass
                z.LongWavelength = 545
                Filters.Add(z)

                ''Current GFP filter
                'Dim x As New Filter
                'x.Type = Filter.FilterTypes.Bandpass
                'x.ShortWavelength = 505
                'x.LongWavelength = 530
                'Filters.Add(x)

            Case 514
                Dim x As New Filter
                x.Type = Filter.FilterTypes.Bandpass
                x.ShortWavelength = 505
                x.LongWavelength = 530
                Filters.Add(x)

                Dim y As New Filter
                y.Type = Filter.FilterTypes.Shortpass
                y.LongWavelength = 545
                Filters.Add(y)

            Case 543
                'Dim x As New Filter
                'x.Type = Filter.FilterTypes.Bandpass
                'x.ShortWavelength = 560
                'x.LongWavelength = 585
                'Filters.Add(x)

                'Dim y As New Filter
                'y.Type = Filter.FilterTypes.Longpass
                'y.ShortWavelength = 545
                'Filters.Add(y)

                'Current MitoTracker filter
                Dim x As New Filter
                x.Type = Filter.FilterTypes.Bandpass
                x.ShortWavelength = 560
                x.LongWavelength = 615
                Filters.Add(x)

                'Case 542
                '    'Cheat for second spectra of 543

                '    Dim x As New Filter
                '    x.Type = Filter.FilterTypes.Bandpass
                '    x.ShortWavelength = 585
                '    x.LongWavelength = 615
                '    Filters.Add(x)

            Case 544
                'Cheat for third spectra of 543

                Dim x As New Filter
                x.Type = Filter.FilterTypes.Longpass
                x.ShortWavelength = 650
                Filters.Add(x)


                'Case 633
                '    Dim x As New Filter
                '    x.Type = Filter.FilterTypes.Longpass
                '    x.ShortWavelength = 650
                '    Filters.Add(x)

        End Select

        ProcessEmissionSpectra()

    End Sub

    Public Shared Function FindCubicSpline(ByVal X() As Double, ByVal Y() As Double) As CubicSpline
        Dim R, S As Double
        Dim N As Integer = X.Count - 1

        Dim B(N), C(N), D(N) As Double
        Dim Spline As New CubicSpline

        Dim M2 As Integer = N - 1

        S = 0

        For I As Integer = 0 To M2
            D(I) = X(I + 1) - X(I)
            R = (Y(I + 1) - Y(I)) / D(I)
            C(I) = R - S
            S = R
        Next

        R = 0
        S = 0
        C(0) = 0
        C(N) = 0

        For I As Integer = 1 To M2
            C(I) = C(I) + (R * C(I - 1))
            B(I) = (X(I - 1) - X(I + 1)) * 2 - R * S
            S = D(I)
            R = S / B(I)
        Next

        For I As Integer = M2 To 1 Step -1
            C(I) = (D(I) * C(I + 1) - C(I)) / B(I)
        Next

        For I As Integer = 0 To M2
            B(I) = (Y(I + 1) - Y(I)) / D(I) - (2 * C(I) + C(I + 1)) * D(I)
            D(I) = (C(I + 1) - C(I)) / D(I)
            C(I) = 3 * C(I)
        Next

        Spline.B = B
        Spline.C = C
        Spline.D = D
        Spline.X = X
        Spline.Y = Y

        Return Spline

    End Function

    Public Structure CubicSpline
        'Should probably make some code to make sure these are all the same size
        Public B() As Double
        Public C() As Double
        Public D() As Double
        Public X() As Double
        Public Y() As Double
    End Structure

    Public Sub ExcitationSpectraFilter(ByVal sender As Object, ByVal e As FilterEventArgs)
        If DirectCast(e.Item, Spectrum).Type = Spectrum.SpectrumTypes.Ex Then
            e.Accepted = True
        Else
            e.Accepted = False
        End If
    End Sub

    Public Sub EmissionSpectraFilter(ByVal sender As Object, ByVal e As FilterEventArgs)
        If DirectCast(e.Item, Spectrum).Type = Spectrum.SpectrumTypes.Em Then
            e.Accepted = True
        Else
            e.Accepted = False
        End If
    End Sub

    Private Sub ProcessEmissionSpectra()
        'ApplyExcitationFilters() Would only apply to non-laser sources, will start with laser sources only

        'FilterSpectraByFluorophor()

        For Each ExcitationSpectrum As Spectrum In ExcitationSpectra
            Dim EmissionSpectrum As Spectrum = EmissionSpectra(ExcitationSpectra.IndexOf(ExcitationSpectrum))
            ScaleEmissionSpectrumByExcitation(ActiveLaser, ExcitationSpectrum, EmissionSpectrum)
        Next

        'ApplyEmissionFilters()

    End Sub

    Private Sub ScaleEmissionSpectrumByExcitation(ByVal LaserWavelength As Integer, ByVal ExcitationSpectrum As Spectrum, ByVal EmissionSpectrum As Spectrum)
        'Will need to be able to interpolate

        Dim ExcitationFactor As Double

        For Each DataPoint As PlotPoint In ExcitationSpectrum.Data
            If DataPoint.X = LaserWavelength Then
                ExcitationFactor = DataPoint.Y
                Exit For
            End If
        Next

        If LaserWavelength = 0 Then ExcitationFactor = 1

        For Each DataPoint As PlotPoint In EmissionSpectrum.Data
            DataPoint.Y *= ExcitationFactor
        Next
    End Sub

    Private Sub ApplyEmissionFilters()
        For Each Filter As Filter In Filters
            For Each Spectrum As Spectrum In EmissionSpectra
                For Each DataPoint As PlotPoint In Spectrum.Data

                    Select Case Filter.Type
                        Case Filter.FilterTypes.Bandpass
                            If DataPoint.X < Filter.ShortWavelength OrElse DataPoint.X > Filter.LongWavelength Then
                                DataPoint.Y = 0
                            End If

                        Case Filter.FilterTypes.Longpass
                            If DataPoint.X < Filter.ShortWavelength Then
                                DataPoint.Y = 0
                            End If

                        Case Filter.FilterTypes.Shortpass
                            If DataPoint.X > Filter.LongWavelength Then
                                DataPoint.Y = 0
                            End If

                    End Select

                Next
            Next
        Next
        
    End Sub
End Class



'Find Maximum of Cubic Spline Curve function

'Find Minimum of Cubic Spline Curve function
