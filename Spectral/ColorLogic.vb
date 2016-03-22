Public Class ColorLogic
    Public Shared RSpline As SpectraGraph.CubicSpline
    Public Shared GSpline As SpectraGraph.CubicSpline
    Public Shared BSpline As SpectraGraph.CubicSpline

    Public Shared Function GenerateRGBColorList(ByVal StartWavelength As Double, ByVal EndWavelength As Double, ByVal WavelengthStep As Double, ByVal Brightness As Double) As List(Of Color)
        LoadCMF()

        Dim RGBList As New List(Of Double())
        Dim ColorList As New List(Of Color)

        For CurrentWavelength As Double = StartWavelength To EndWavelength Step WavelengthStep
            Dim RGB As Double() = WavelengthToRGB(CurrentWavelength)

            Dim Gamma As Double = 0.8

            RGB(0) ^= Gamma
            RGB(1) ^= Gamma
            RGB(2) ^= Gamma

            RGBList.Add(RGB)

            'Correct colors to achieve uniform intensity?

        Next


        For Each RGB As Double() In RGBList
            Dim NewColor As New Color

            NewColor.ScA = 1
            NewColor.ScR = CSng(RGB(0) * Brightness)
            NewColor.ScG = CSng(RGB(1) * Brightness)
            NewColor.ScB = CSng(RGB(2) * Brightness)
            'NewColor.A = 255
            'NewColor.R = CByte(RGB(0) * 255 * Brightness)
            'NewColor.G = CByte(RGB(1) * 255 * Brightness)
            'NewColor.B = CByte(RGB(2) * 255 * Brightness)

            ColorList.Add(NewColor)
        Next

        Return ColorList

    End Function

    Public Shared Function GenerateRGBColorList2(ByVal StartWavelength As Double, ByVal EndWavelength As Double, ByVal WavelengthStep As Double, ByVal Brightness As Double) As List(Of Color)
        LoadCMF()

        Dim RGBList As New List(Of Double())
        Dim ColorList As New List(Of Color)

        For CurrentWavelength As Double = StartWavelength To EndWavelength Step WavelengthStep
            Dim RGB As Double() = {0, 0, 0}

            Dim IntensityScalingFactor As Double = 161 / 255

            Select Case CurrentWavelength
                Case Is < 380
                    RGB(0) = 0.5
                    RGB(1) = 0
                    RGB(2) = 0.5
                Case Is <= 420
                    RGB(0) = -0.5 * (CurrentWavelength - 440) / (440 - 380)
                    RGB(1) = 0
                    RGB(2) = 0.5
                Case Is < 440
                    RGB(0) = -0.5 * (CurrentWavelength - 440) / (440 - 380)
                    RGB(1) = 0
                    'RGB(2) = 0.5
                    RGB(2) = (15849.2 - 112.464 * CurrentWavelength + 0.266291 * CurrentWavelength ^ 2 - 0.00020868 * CurrentWavelength ^ 3) / 255
                Case Is < 490
                    RGB(0) = 0
                    RGB(1) = 0.5 * ((CurrentWavelength - 440) / (490 - 440))
                    'RGB(2) = 0.5
                    RGB(2) = (15849.2 - 112.464 * CurrentWavelength + 0.266291 * CurrentWavelength ^ 2 - 0.00020868 * CurrentWavelength ^ 3) / 255
                Case Is < 510
                    RGB(0) = 0
                    'RGB(1) = 0.5
                    RGB(1) = (0.000027118 * CurrentWavelength ^ 3 - 0.0591625 * CurrentWavelength ^ 2 + 39.9634 * CurrentWavelength - 8440.06) / 255
                    RGB(2) = -0.5 * (CurrentWavelength - 510) / (510 - 490)
                Case Is < 580
                    RGB(0) = 0.5 * (CurrentWavelength - 510) / (580 - 510)
                    RGB(1) = (0.000027118 * CurrentWavelength ^ 3 - 0.0591625 * CurrentWavelength ^ 2 + 39.9634 * CurrentWavelength - 8440.06) / 255
                    'RGB(1) = 0.5
                    RGB(2) = 0
                Case Is < 645
                    RGB(0) = (-8845.18 + 37.0252 * CurrentWavelength - 0.049725 * CurrentWavelength ^ 2 + 0.0000216572 * CurrentWavelength ^ 3) / 255
                    'RGB(0) = 0.5
                    RGB(1) = -0.5 * (CurrentWavelength - 645) / (645 - 580)
                    RGB(2) = 0
                Case Is < 700
                    RGB(0) = (-8845.18 + 37.0252 * CurrentWavelength - 0.049725 * CurrentWavelength ^ 2 + 0.0000216572 * CurrentWavelength ^ 3) / 255
                    'RGB(0) = 0.5
                    RGB(1) = 0
                    RGB(2) = 0
                Case Else
                    RGB(0) = 0.5
                    RGB(1) = 0
                    RGB(2) = 0
            End Select

            Dim BrightnessCorrection As Double

            Select Case CurrentWavelength
                Case Is >= 700
                    BrightnessCorrection = 0.3 + 0.7 * (780 - CurrentWavelength) / (780 - 700)
                Case Is <= 420
                    BrightnessCorrection = 0.3 + 0.7 * (CurrentWavelength - 380) / (420 - 380)
                Case Else
                    BrightnessCorrection = 1
            End Select

            RGB(0) *= BrightnessCorrection / IntensityScalingFactor
            RGB(1) *= BrightnessCorrection / IntensityScalingFactor
            RGB(2) *= BrightnessCorrection / IntensityScalingFactor

            Dim Gamma As Double = 0.8

            RGB(0) ^= Gamma
            RGB(1) ^= Gamma
            RGB(2) ^= Gamma

            RGBList.Add(RGB)
        Next


        For Each RGB As Double() In RGBList
            Dim NewColor As New Color

            'NewColor.ScA = 1
            'NewColor.ScR = CSng(RGB(0) * Brightness) 'scRGB values supposedly range from -0.5 to just less than 7.5, but the red values from this function dip to -1
            'NewColor.ScG = CSng(RGB(1) * Brightness)
            'NewColor.ScB = CSng(RGB(2) * Brightness)
            NewColor.A = 255
            NewColor.R = CByte(RGB(0) * 255 * Brightness)
            NewColor.G = CByte(RGB(1) * 255 * Brightness)
            NewColor.B = CByte(RGB(2) * 255 * Brightness)

            ColorList.Add(NewColor)
        Next

        Return ColorList

    End Function

    Public Shared Sub LoadCMF()
        'File structure: First row contains headers.
        '                Column 1: Wavelength (nm); Columns 2-4: r, g, and b values
        Dim FileLineList As List(Of String) = MainWindow.LoadFile(My.Settings.CMFLocation)
        Dim N As Integer = FileLineList.Count - 2
        Dim X(N), R(N), G(N), B(N) As Double

        For I As Integer = 0 To N
            Dim LineElements(0 To 3) As String

            LineElements = FileLineList(I + 1).Split({CChar(",")})

            X(I) = CDbl(LineElements(0))
            R(I) = CDbl(LineElements(1))
            G(I) = CDbl(LineElements(2))
            B(I) = CDbl(LineElements(3))
        Next

        RSpline = SpectraGraph.FindCubicSpline(X, R)
        GSpline = SpectraGraph.FindCubicSpline(X, G)
        BSpline = SpectraGraph.FindCubicSpline(X, B)

    End Sub

    Public Shared Function WavelengthToRGB(ByVal Wavelength As Double) As Double()
        Dim R As Double = 0
        Dim G As Double = 0
        Dim B As Double = 0
        Dim N As Integer = RSpline.X.Length - 1

        'Make this faster with a Queue of somesort?

        For I As Integer = 0 To N
            Dim WavelengthBase As Double = RSpline.X(I)

            If (I = N AndAlso Wavelength = WavelengthBase) OrElse
                (I <> N AndAlso Wavelength >= WavelengthBase AndAlso Wavelength < RSpline.X(I + 1)) Then

                Dim DeltaWavelength As Double = Wavelength - WavelengthBase

                R = ((RSpline.D(I) * DeltaWavelength + RSpline.C(I)) * DeltaWavelength + RSpline.B(I)) * DeltaWavelength + RSpline.Y(I)
                G = ((GSpline.D(I) * DeltaWavelength + GSpline.C(I)) * DeltaWavelength + GSpline.B(I)) * DeltaWavelength + GSpline.Y(I)
                B = ((BSpline.D(I) * DeltaWavelength + BSpline.C(I)) * DeltaWavelength + BSpline.B(I)) * DeltaWavelength + BSpline.Y(I)

                Exit For
            End If
        Next

        'At edges of CMF, use linear slope until = 0
        If Wavelength < RSpline.X(0) Then
            Dim DeltaR As Double = (RSpline.Y(1) - RSpline.Y(0)) / (RSpline.X(1) - RSpline.X(0))
            Dim DeltaG As Double = (GSpline.Y(1) - GSpline.Y(0)) / (GSpline.X(1) - GSpline.X(0))
            Dim DeltaB As Double = (BSpline.Y(1) - BSpline.Y(0)) / (BSpline.X(1) - BSpline.X(0))

            Dim DeltaX As Double = Wavelength - RSpline.X(0)

            R = DeltaR * DeltaX + RSpline.Y(0)
            G = DeltaG * DeltaX + GSpline.Y(0)
            B = DeltaB * DeltaX + BSpline.Y(0)

            If Math.Sign(R) <> Math.Sign(RSpline.Y(0)) Then R = 0
            If Math.Sign(G) <> Math.Sign(GSpline.Y(0)) Then G = 0
            If Math.Sign(B) <> Math.Sign(BSpline.Y(0)) Then B = 0
        ElseIf Wavelength > RSpline.X(N) Then
            Dim DeltaR As Double = (RSpline.Y(N) - RSpline.Y(N - 1)) / (RSpline.X(N) - RSpline.X(N - 1))
            Dim DeltaG As Double = (GSpline.Y(N) - GSpline.Y(N - 1)) / (GSpline.X(N) - GSpline.X(N - 1))
            Dim DeltaB As Double = (BSpline.Y(N) - BSpline.Y(N - 1)) / (BSpline.X(N) - BSpline.X(N - 1))

            Dim DeltaX As Double = Wavelength - RSpline.X(N)

            R = DeltaR * DeltaX + RSpline.Y(N)
            G = DeltaG * DeltaX + GSpline.Y(N)
            B = DeltaB * DeltaX + BSpline.Y(N)

            If Math.Sign(R) <> Math.Sign(RSpline.Y(0)) Then R = 0
            If Math.Sign(G) <> Math.Sign(GSpline.Y(0)) Then G = 0
            If Math.Sign(B) <> Math.Sign(BSpline.Y(0)) Then B = 0
        End If

        Return {R, G, B}

    End Function

End Class
