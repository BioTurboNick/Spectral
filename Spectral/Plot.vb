Public Class Plot
    Inherits Shape

    Private Spline As SpectraGraph.CubicSpline
    Private MaxValue As Double
    Private MinValue As Double

#Region "Dependency Properties"
    Public Shared ReadOnly XAxisMinimumProperty As DependencyProperty = DependencyProperty.Register("XAxisMinimum", GetType(Double), GetType(Plot), New FrameworkPropertyMetadata(0.0, New PropertyChangedCallback(AddressOf OnPlotChanged)))
    Public Shared ReadOnly XAxisMaximumProperty As DependencyProperty = DependencyProperty.Register("XAxisMaximum", GetType(Double), GetType(Plot), New FrameworkPropertyMetadata(0.0, New PropertyChangedCallback(AddressOf OnPlotChanged)))
    Public Shared ReadOnly YAxisMinimumProperty As DependencyProperty = DependencyProperty.Register("YAxisMinimum", GetType(Double), GetType(Plot), New FrameworkPropertyMetadata(0.0, New PropertyChangedCallback(AddressOf OnPlotChanged)))
    Public Shared ReadOnly YAxisMaximumProperty As DependencyProperty = DependencyProperty.Register("YAxisMaximum", GetType(Double), GetType(Plot), New FrameworkPropertyMetadata(0.0, New PropertyChangedCallback(AddressOf OnPlotChanged)))
    Public Shared ReadOnly PlotHeightProperty As DependencyProperty = DependencyProperty.Register("PlotHeight", GetType(Double), GetType(Plot), New FrameworkPropertyMetadata(0.0, New PropertyChangedCallback(AddressOf OnPlotChanged)))
    Public Shared ReadOnly PlotWidthProperty As DependencyProperty = DependencyProperty.Register("PlotWidth", GetType(Double), GetType(Plot), New FrameworkPropertyMetadata(0.0, New PropertyChangedCallback(AddressOf OnPlotChanged)))
    Public Shared ReadOnly PlotStepProperty As DependencyProperty = DependencyProperty.Register("PlotStep", GetType(Double), GetType(Plot), New FrameworkPropertyMetadata(0.0, New PropertyChangedCallback(AddressOf OnPlotChanged)))
    Public Shared ReadOnly DataProperty As DependencyProperty = DependencyProperty.Register("Data", GetType(List(Of PlotPoint)), GetType(Plot), New FrameworkPropertyMetadata(New List(Of PlotPoint), New PropertyChangedCallback(AddressOf OnPlotDataChanged)))
    Public Shared ReadOnly InterpolateProperty As DependencyProperty = DependencyProperty.Register("Interpolate", GetType(Boolean), GetType(Plot), New FrameworkPropertyMetadata(False, New PropertyChangedCallback(AddressOf OnPlotChanged)))
    Public Shared ReadOnly IsFilledProperty As DependencyProperty = DependencyProperty.Register("IsFilled", GetType(Boolean), GetType(Plot), New FrameworkPropertyMetadata(False, New PropertyChangedCallback(AddressOf OnPlotChanged)))

    Public Property XAxisMinimum As Double
        Get
            Return CDbl(MyBase.GetValue(XAxisMinimumProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(XAxisMinimumProperty, value)
        End Set
    End Property

    Public Property XAxisMaximum As Double
        Get
            Return CDbl(MyBase.GetValue(XAxisMaximumProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(XAxisMaximumProperty, value)
        End Set
    End Property

    Public Property YAxisMinimum As Double
        Get
            Return CDbl(MyBase.GetValue(YAxisMinimumProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(YAxisMinimumProperty, value)
        End Set
    End Property

    Public Property YAxisMaximum As Double
        Get
            Return CDbl(MyBase.GetValue(YAxisMaximumProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(YAxisMaximumProperty, value)
        End Set
    End Property

    Public Property PlotHeight As Double
        Get
            Return CDbl(MyBase.GetValue(PlotHeightProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(PlotHeightProperty, value)
        End Set
    End Property

    Public Property PlotWidth As Double
        Get
            Return CDbl(MyBase.GetValue(PlotStepProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(PlotStepProperty, value)
        End Set
    End Property

    Public Property PlotStep As Double
        Get
            Return CDbl(MyBase.GetValue(PlotStepProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(PlotStepProperty, value)
        End Set
    End Property

    Public Property Data As List(Of PlotPoint)
        Get
            Return CType(MyBase.GetValue(DataProperty), List(Of PlotPoint))
        End Get
        Set(ByVal value As List(Of PlotPoint))
            MyBase.SetValue(DataProperty, value)
        End Set
    End Property

    Public Property Interpolate As Boolean
        Get
            Return CBool(MyBase.GetValue(InterpolateProperty))
        End Get
        Set(ByVal value As Boolean)
            MyBase.SetValue(InterpolateProperty, value)
        End Set
    End Property

    Public Property IsFilled As Boolean
        Get
            Return CBool(MyBase.GetValue(IsFilledProperty))
        End Get
        Set(ByVal value As Boolean)
            MyBase.SetValue(IsFilledProperty, value)
        End Set
    End Property

#End Region

    Private Shared Sub OnPlotDataChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        If e.NewValue Is e.OldValue OrElse e.NewValue Is Nothing Then
            Exit Sub
        End If

        DirectCast(d, Plot).OnPlotDataChanged(d)
    End Sub

    Private Sub OnPlotDataChanged(ByVal d As DependencyObject)
        Dim N As Integer = Data.Count - 1
        Dim X(N), Y(N) As Double

        For I As Integer = 0 To N
            X(I) = Data(I).X
            Y(I) = Data(I).Y
        Next

        Spline = SpectraGraph.FindCubicSpline(X, Y)

        'Find the maximum value of the interpolated function
        Dim MaxIndex As Integer = Array.IndexOf(Y, Y.Max)

        'Dim PossibleMaxesX As New List(Of Double)

        'For I As Integer = -1 To 0
        '    Dim Index As Integer = MaxIndex + I
        '    Dim U As Double = -2 * Spline.C(Index)
        '    Dim V As Double = 4 * Spline.C(Index) ^ 2
        '    Dim W As Double = 12 * Spline.B(Index) * Spline.D(Index)
        '    Dim Z As Double = 6 * Spline.D(Index)

        '    If V >= W Then
        '        Dim Roots(1) As Double

        '        Roots(0) = (U + Math.Sqrt(V - W)) / Z + Spline.X(Index)
        '        Roots(1) = (U - Math.Sqrt(V - W)) / Z + Spline.X(Index)

        '        For Each Root As Double In Roots
        '            If Root > 0 AndAlso Root < 5 Then
        '                PossibleMaxesX.Add(Root)
        '            End If
        '        Next
        '    End If
        'Next

        If IsFilled Then
            Dim FillColor As Color = EmissionColor(1)
            FillColor.A = 127
            Fill = New SolidColorBrush(FillColor)
            Dim StrokeColor As Color = EmissionColor(0.5)
            Stroke = New SolidColorBrush(StrokeColor)
        Else
            Dim StrokeColor As Color = EmissionColor(1)
            Stroke = New SolidColorBrush(StrokeColor)
        End If

        

        'Dim PossibleMaxes As New List(Of Double)

        'For Each PossibleMaxX As Double In PossibleMaxesX

        'Next

        ''Find the minimum value of the interpolated function
        'Dim MinIndex As Integer = Array.IndexOf(Y, Y.Min)
        'Dim MinDirection As Integer = CInt(Y(MinIndex + 1) - Y(MinIndex - 1))


        DirectCast(d, Plot).OnPlotChanged(d)
    End Sub

    Private Shared Sub OnPlotChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        If e.NewValue Is e.OldValue OrElse e.NewValue Is Nothing Then
            Exit Sub
        End If

        DirectCast(d, Plot).OnPlotChanged(d)
    End Sub

    Private Sub OnPlotChanged(ByVal d As DependencyObject)
        InvalidateMeasure()
        InvalidateVisual()

    End Sub

    Protected Overrides ReadOnly Property DefiningGeometry As System.Windows.Media.Geometry
        Get
            Dim Geometry As New StreamGeometry

            Using Context As StreamGeometryContext = Geometry.Open()
                InternalDrawShapeGeometry(Context)
            End Using

            ' Freeze the geometry for performance benefits
            Geometry.Freeze()

            Return Geometry
        End Get
    End Property

    Private Sub InternalDrawShapeGeometry(ByVal Context As StreamGeometryContext)
        If PlotStep = 0 Then Exit Sub

        Context.BeginFigure(New Point(((Data(0).X - XAxisMinimum) / (XAxisMaximum - XAxisMinimum) * Width), -0 * Height + Height), True, False)

        Context.LineTo(New Point(((Data(0).X - XAxisMinimum) / (XAxisMaximum - XAxisMinimum) * Width), -Data(0).Y * Height + Height), False, True)

        If Interpolate Then
            For I As Integer = 0 To Data.Count - 2
                For InterpolatedX As Double = Data(I).X + PlotStep To Data(I + 1).X Step PlotStep
                    Dim T As Double = InterpolatedX - Data(I).X
                    Dim InterpolatedY As Double = ((Spline.D(I) * T + Spline.C(I)) * T + Spline.B(I)) * T + Spline.Y(I)
                    Dim Pt As New Point(((InterpolatedX - XAxisMinimum) / (XAxisMaximum - XAxisMinimum) * Width), -InterpolatedY * Height + Height)
                    Context.LineTo(Pt, True, True)
                Next
            Next
        Else
            For I As Integer = 1 To Data.Count - 1
                Dim Pt As New Point(((Data(I).X - XAxisMinimum) / (XAxisMaximum - XAxisMinimum) * Width), -Data(I).Y * Height + Height)
                Context.LineTo(Pt, True, True)
            Next
        End If

        Context.LineTo(New Point(((Data(Data.Count - 1).X - XAxisMinimum) / (XAxisMaximum - XAxisMinimum) * Width), -0 * Height + Height), False, True)

    End Sub

    Private Function EmissionColor(ByVal Brightness As Double) As Color
        'First Approximation

        Dim RGBSum(2) As Double
        Dim MixedColor As New Color

        ColorLogic.LoadCMF()

        For Each DataPoint As PlotPoint In Data
            Dim RGB As Double() = ColorLogic.WavelengthToRGB(DataPoint.X)

            RGBSum(0) += RGB(0) * DataPoint.Y
            RGBSum(1) += RGB(1) * DataPoint.Y
            RGBSum(2) += RGB(2) * DataPoint.Y
        Next

        Dim RGBSumMax As Double = RGBSum.Max

        If RGBSumMax = 0 Then Return New Color

        RGBSum(0) /= RGBSumMax
        RGBSum(1) /= RGBSumMax
        RGBSum(2) /= RGBSumMax

        RGBSum(0) *= Brightness '* 255
        RGBSum(1) *= Brightness '* 255
        RGBSum(2) *= Brightness '* 255

        MixedColor.A = 255
        'MixedColor.R = CByte(RGBSum(0))
        'MixedColor.G = CByte(RGBSum(1))
        'MixedColor.B = CByte(RGBSum(2))
        MixedColor.ScR = CSng(RGBSum(0))
        MixedColor.ScG = CSng(RGBSum(1))
        MixedColor.ScB = CSng(RGBSum(2))

        Return MixedColor

    End Function

End Class

Public Class PlotPoint
    Public Property X As Double
    Public Property Y As Double

    Sub New(ByVal XValue As Double, ByVal YValue As Double)
        X = XValue
        Y = YValue
    End Sub

    Public Function Clone() As PlotPoint
        Return DirectCast(Me.MemberwiseClone, PlotPoint)
    End Function

End Class