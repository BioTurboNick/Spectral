

Public Module LinearAlgebra
    'This module treats dimensions as M, N, O, ...
    'While arrays iterate through and are set in reverse order.
    'e.g. {{a, b},{c, d}, {e, f}} creates the 2x3 matrix: | a b |
    '                                                     | c d |
    '                                                     | e f |
    'where the bounds are (2, 1)

    Function Equals(ByVal Matrix1 As Array, ByVal Matrix2 As Array) As Boolean
        Dim Matrix1Dimensions As Integer = Matrix1.Rank - 1
        Dim Matrix2Dimensions As Integer = Matrix2.Rank - 1
        Dim Matrix1Bounds() As Integer = GetBounds(Matrix1)
        Dim Matrix2Bounds() As Integer = GetBounds(Matrix2)

        'If dimensions aren't equal, the matrices aren't either
        If Matrix1Dimensions <> Matrix2Dimensions Then Return False

        'If bounds of each dimension aren't equal, the matrices aren't either
        For BoundID As Integer = 0 To Matrix1Dimensions
            If Matrix1Bounds(BoundID) <> Matrix2Bounds(BoundID) Then Return False
        Next

        'Now its okay to check each value
        Dim Matrix1Enumerator As IEnumerator = Matrix1.GetEnumerator
        Dim Matrix2Enumerator As IEnumerator = Matrix2.GetEnumerator

        While Matrix1Enumerator.MoveNext And Matrix2Enumerator.MoveNext
            If CDbl(Matrix1Enumerator.Current) <> CDbl(Matrix2Enumerator.Current) Then Return False
        End While

        Return True

    End Function

    Function GetBounds(ByVal Matrix As Array) As Integer()
        Dim MatrixDimensions As Integer = Matrix.Rank - 1
        Dim MatrixBounds(MatrixDimensions) As Integer

        For Dimension As Integer = 0 To MatrixDimensions
            MatrixBounds(Dimension) = Matrix.GetUpperBound(Dimension)
        Next

        Return MatrixBounds
    End Function

    Function GetLengths(ByVal Matrix As Array) As Integer()
        Dim MatrixDimensions As Integer = Matrix.Rank - 1
        Dim MatrixBounds(MatrixDimensions) As Integer

        For Dimension As Integer = 0 To MatrixDimensions
            MatrixBounds(Dimension) = Matrix.GetUpperBound(Dimension) + 1
        Next

        Return MatrixBounds
    End Function

    Function Add(ByVal Matrix1 As Array, ByVal Matrix2 As Array) As Array
        Dim Matrix1Dimensions As Integer = Matrix1.Rank - 1
        Dim Matrix2Dimensions As Integer = Matrix2.Rank - 1
        Dim Matrix1Bounds() As Integer = GetBounds(Matrix1)
        Dim Matrix2Bounds() As Integer = GetBounds(Matrix2)

        If Not Equals(Matrix1Bounds, Matrix2Bounds) Then
            Throw New InvalidOperationException("Matrix addition is undefined for matrices of different sizes.")
        End If

        Dim ElementIndex(Matrix1Dimensions) As Integer
        Dim ResultMatrix As Array = Array.CreateInstance(GetType(Double), GetLengths(Matrix1))

        'There may be a way to more directly iterate and increment the ElementIndex
        For ElementID As Integer = 0 To ResultMatrix.Length - 1
            Dim Sum As Double = CDbl(Matrix1.GetValue(ElementIndex)) + CDbl(Matrix2.GetValue(ElementIndex))

            ResultMatrix.SetValue(Sum, ElementIndex)

            'Increment ElementIndex
            For Dimension As Integer = Matrix1Dimensions To 0 Step -1
                Dim IncrementNextDimension As Boolean

                If ElementIndex(Dimension) = Matrix1Bounds(Dimension) Then
                    ElementIndex(Dimension) = 0
                    IncrementNextDimension = True
                ElseIf (Dimension = Matrix1Dimensions) OrElse IncrementNextDimension Then
                    ElementIndex(Dimension) += 1
                    Exit For
                End If
            Next
        Next

        Return ResultMatrix

    End Function

    Function Multiply(ByVal Matrix1 As Array, ByVal Matrix2 As Array) As Array
        Dim Matrix1Bounds() As Integer = GetBounds(Matrix1)
        Dim Matrix2Bounds() As Integer = GetBounds(Matrix2)

        If Matrix1.Rank > 2 OrElse Matrix2.Rank > 2 Then
            Throw New InvalidOperationException("Matrix multiplication is only defined for two-dimensional matrices")
        End If

        If Not Matrix1Bounds(1) = Matrix2Bounds(0) Then
            Throw New InvalidOperationException("Matrix multiplication requires Matrix1 to have bounds (M,N) and Matrix2 to have bounds (N,P).")
        End If

        Dim ElementIndex(1) As Integer
        Dim ResultMatrix As Array = Array.CreateInstance(GetType(Double), {Matrix1Bounds(0) + 1, Matrix2Bounds(1) + 1})

        For Matrix2ColumnIndex As Integer = 0 To Matrix2Bounds(1)
            For Matrix1RowIndex As Integer = 0 To Matrix1Bounds(0)
                Dim ResultElement As Double = 0

                For CurrentIndex As Integer = 0 To Matrix1Bounds(1)
                    ResultElement += CDbl(Matrix1.GetValue({Matrix1RowIndex, CurrentIndex})) * CDbl(Matrix2.GetValue({CurrentIndex, Matrix2ColumnIndex}))
                Next

                ResultMatrix.SetValue(ResultElement, {Matrix1RowIndex, Matrix2ColumnIndex})
            Next
        Next

        Return ResultMatrix
    End Function

    Function Multiply(ByVal Matrix As Array, ByVal Scalar As Double) As Array
        Dim MatrixDimensions As Integer = Matrix.Rank - 1
        Dim MatrixBounds() As Integer = GetBounds(Matrix)
        Dim ElementIndex(MatrixDimensions) As Integer

        For Index As Integer = 0 To Matrix.Length - 1
            Matrix.SetValue(CDbl(Matrix.GetValue(ElementIndex)) * Scalar, ElementIndex)

            'Increment ElementIndex
            For Dimension As Integer = MatrixDimensions To 0 Step -1
                Dim IncrementNextDimension As Boolean

                If ElementIndex(Dimension) = MatrixBounds(Dimension) Then
                    ElementIndex(Dimension) = 0
                    IncrementNextDimension = True
                ElseIf (Dimension = MatrixDimensions) OrElse IncrementNextDimension Then
                    ElementIndex(Dimension) += 1
                    Exit For
                End If
            Next
        Next

        Return Matrix

    End Function

    Function Power(ByVal Matrix As Array, ByVal Exponent As Double) As Array
        'Not strictly a linear algebra function. Simply allows every element of a matrix to be raised to the specified power

        Dim MatrixDimensions As Integer = Matrix.Rank - 1
        Dim MatrixBounds() As Integer = GetBounds(Matrix)
        Dim ElementIndex(MatrixDimensions) As Integer

        For Index As Integer = 0 To Matrix.Length - 1
            Matrix.SetValue(CDbl(Matrix.GetValue(ElementIndex)) ^ Exponent, ElementIndex)

            'Increment ElementIndex
            For Dimension As Integer = MatrixDimensions To 0 Step -1
                Dim IncrementNextDimension As Boolean

                If ElementIndex(Dimension) = MatrixBounds(Dimension) Then
                    ElementIndex(Dimension) = 0
                    IncrementNextDimension = True
                ElseIf (Dimension = MatrixDimensions) OrElse IncrementNextDimension Then
                    ElementIndex(Dimension) += 1
                    Exit For
                End If
            Next
        Next

        Return Matrix

    End Function

    Function Zeros(ByVal Lengths() As Integer, ByVal DataType As Type) As Array
        Return Array.CreateInstance(DataType, Lengths)
    End Function

    Function Build(ByVal Pieces As Array()) As Array
        'Assemble vectors into a 2D matrix, 2d matrices into a 3d matrix, etc.

        Dim PieceMatrixDimensions As Integer = Pieces(0).Rank - 1
        Dim PieceMatrixBounds() As Integer = GetBounds(Pieces(0))
        Dim PieceElementIndex(PieceMatrixDimensions) As Integer

        'Check that all the pieces have the same dimensions and bounds
        For Each PieceMatrix As Array In Pieces
            If PieceMatrix.Rank - 1 <> PieceMatrixDimensions OrElse Not Equals(GetBounds(PieceMatrix), PieceMatrixBounds) Then
                Throw New InvalidOperationException("Pieces can only be assembled if they have the same dimensions and bounds.")
            End If
        Next

        Dim BuiltMatrixLengths(PieceMatrixDimensions + 1) As Integer

        For Index As Integer = 0 To PieceMatrixDimensions
            BuiltMatrixLengths(Index) = PieceMatrixBounds(Index) + 1
        Next

        BuiltMatrixLengths(PieceMatrixDimensions + 1) = Pieces.Count

        Dim BuiltMatrix As Array = Array.CreateInstance(GetType(Double), BuiltMatrixLengths)
        Dim ElementIndex(PieceMatrixDimensions + 1) As Integer

        For PieceMatrixID As Integer = 0 To Pieces.Count - 1

            For Index As Integer = 0 To Pieces(PieceMatrixID).Length - 1
                Dim PieceMatrixValue As Double = CDbl(Pieces(PieceMatrixID).GetValue(PieceElementIndex))
                BuiltMatrix.SetValue(PieceMatrixValue, ElementIndex)

                'Increment PieceElementIndex
                For Dimension As Integer = PieceMatrixDimensions To 0 Step -1
                    Dim IncrementNextDimension As Boolean

                    If PieceElementIndex(Dimension) = PieceMatrixBounds(Dimension) Then
                        ElementIndex(Dimension) = 0
                        PieceElementIndex(Dimension) = 0
                        IncrementNextDimension = True
                    ElseIf (Dimension = PieceMatrixDimensions) OrElse IncrementNextDimension Then
                        ElementIndex(Dimension) += 1
                        PieceElementIndex(Dimension) += 1
                        Exit For
                    End If
                Next
            Next

            ElementIndex(PieceMatrixDimensions + 1) += 1

        Next

        Return BuiltMatrix

    End Function

End Module
