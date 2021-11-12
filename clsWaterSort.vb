Public Class clsSolutionTreeItem
    Public board As clsBoard
    Dim children As New List(Of clsSolutionTreeItem)
    Public moves As New List(Of clsMove)

    Public Sub New(b As clsBoard, Optional MovesSoFar As List(Of clsMove) = Nothing, Optional CurrentMove As clsMove = Nothing)
        board = b
        If MovesSoFar IsNot Nothing Then moves.AddRange(MovesSoFar)
        If CurrentMove IsNot Nothing Then moves.Add(CurrentMove)
    End Sub

    Public Function GetChildren() As List(Of clsSolutionTreeItem)
        Return children
    End Function

    Public Sub AddChild(b As clsBoard, m As clsMove, ByRef e As HashSet(Of String))
        If Not e.Contains(b.BoardToString) Then
            children.Add(New clsSolutionTreeItem(b, moves, m))
            e.Add(b.BoardToString)
        End If
    End Sub

    Public Function AddAllPossibleChildren(ByRef e As HashSet(Of String)) As Integer
        If Not board.IsSolved And board.HasValidMoves Then
            For Each m As clsMove In board.ValidMovesLeft()
                Dim b As clsBoard = board.DeepClone()
                b.Move(m.FromVial, m.ToVial)
                If Not e.Contains(b.BoardToString) Then
                    AddChild(b, m, e)
                End If
            Next
        End If
        Return board.ValidMovesLeft.Count
    End Function

End Class

Public Class clsMove
    Public FromVial As Integer
    Public ToVial As Integer
    Public Color As Colors

    Public Sub New(f As Integer, t As Integer, c As Colors)
        FromVial = f
        ToVial = t
        Color = c
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0:00} -> {1:00} ({2})", FromVial + 1, ToVial + 1, Color)
    End Function

End Class

Public Class clsItem
    Public Color As Colors
    Public Size As Integer

    Public Sub New(c As Colors, s As Integer)
        Color = c
        Size = s
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0}/{1}", Color, Size)
    End Function

End Class

Public Class clsSolution

    Public moves As New List(Of clsMove)

    Public Enum Strategy
        Any = 0
        Shortest = 1
    End Enum

    Public Sub New(m As List(Of clsMove))
        'deep clone needed?
        moves = m
    End Sub
End Class

Public Class clsBoard
    Public Vials As New List(Of clsVial)
    Public IsSolved As Boolean = False
    Public HasValidMoves As Boolean = True
    Public Errors As New List(Of String)

    Public Sub New(l As List(Of clsVial))
        Vials.AddRange(l)
    End Sub

    Public Sub New(s As String)
        For Each v As String In s.Split(" ")
            Dim size As Integer = v.Length / 2
            Vials.Add(New clsVial(v, size))
        Next
    End Sub

    ''' <summary>
    ''' Update the state of the board (is it solved, has it moves left)
    ''' </summary>
    Public Sub UpdateState()
        Dim state As Boolean = True
        For Each v As clsVial In Vials
            If Not v.IsLocked Then state = False
        Next
        IsSolved = state
        HasValidMoves = ValidMovesLeft.Count > 0
    End Sub

    ''' <summary>
    ''' Determine whether this is a valid board (i.e. there are vials and no color is present more often than the size of a vial).
    ''' </summary>
    ''' <returns>True if the board is a valid board, false otherwise</returns>
    Public Function ValidBoard() As Boolean
        Dim a As New Dictionary(Of Colors, Integer)
        Dim s As Colors
        Dim t As Integer

        'if board contains less than one vial then the board is invalid
        If Vials.Count < 1 Then
            Errors.Add("No Vials found.")
            Return False
        End If

        t = Vials(0).Size

        For Each v As clsVial In Vials
            'if vials have different sizes then the board is invalid
            If t <> v.Size Then
                Errors.Add("Vials have different sizes.")
                Return False
            End If

            For p = 1 To t
                s = v.GetPosition(p)
                If a.ContainsKey(s) Then
                    a(s) += 1
                Else
                    a.Add(s, 1)
                End If
            Next
        Next

        For Each i As Colors In a.Keys
            'if there is less or more of one color to fill exactly one vial, the board is invalid
            If a(i) <> t And i <> Colors.NONE Then
                Errors.Add(String.Format("Color {0} has wrong count of {1}.", i, a(i)))
            End If
        Next

        'if there are errors, the board is wrong...
        If Errors.Count > 0 Then Return False

        'if we made it this far, the board should be ok...
        Return True
    End Function

    ''' <summary>
    ''' returns the amount of valid moves possible on the current board
    ''' </summary>
    ''' <returns>returns a list of valid moves</returns>
    Public Function ValidMovesLeft() As List(Of clsMove)
        Dim ValidMoves As New List(Of clsMove)

        For v1 As Integer = 0 To Vials.Count - 1
            For v2 As Integer = 0 To Vials.Count - 1
                If IsValidMove(v1, v2) Then ValidMoves.Add(New clsMove(v1, v2, Vials(v1).TakeOut(False).Color))
            Next
        Next

        Return ValidMoves
    End Function

    ''' <summary>
    ''' move from v1 to v2 if possible, return false if not
    ''' </summary>
    ''' <param name="v1">source vial</param>
    ''' <param name="v2">destination vial</param>
    ''' <returns></returns>
    Public Function Move(v1 As Integer, v2 As Integer)
        If IsValidMove(v1, v2) Then
            Vials(v2).PutIn(Vials(v1).TakeOut())
            UpdateState()
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Is this a valid move?
    ''' </summary>
    ''' <param name="v1">source vial</param>
    ''' <param name="v2">destination vial</param>
    ''' <returns></returns>
    Public Function IsValidMove(v1 As Integer, v2 As Integer)
        'check bounds
        If v1 < 0 Or v1 > Vials.Count Or v2 < 0 Or v2 > Vials.Count Then Return False
        'move to itself is invalid
        If v1 = v2 Then Return False

        'move of a monochrome vial to an empty vial is pointless (just switching stuff around for no reason)
        If Vials(v1).IsMonochrome And Vials(v2).IsEmpty Then Return False

        'try move
        If Vials(v2).PutIn(Vials(v1).TakeOut(False), False) Then Return True
        'default to invalid
        Return False
    End Function

    ''' <summary>
    ''' Create a deep clone of the board (where every object is a NEW copy of the old one)
    ''' </summary>
    ''' <returns>a deep cloned copy of the board</returns>
    Function DeepClone() As clsBoard
        Dim l As New List(Of clsVial)
        Dim i As List(Of clsItem)
        For Each c As clsVial In Vials
            i = New List(Of clsItem)
            For Each x As clsItem In c.Items
                i.Add(New clsItem(x.Color, x.Size))
            Next
            l.Add(New clsVial(i, c.Size))
        Next
        Return New clsBoard(l)
    End Function

    ''' <summary>
    ''' Return a string representing the board
    ''' </summary>
    ''' <returns>The string representing the board</returns>
    Public Function BoardToString() As String
        Dim s As New List(Of String)

        For Each v As clsVial In Vials
            s.Add(v.VialToString)
        Next

        Return Join(s.ToArray, "|")
    End Function
End Class

Public Class clsVial
    Private _items As New List(Of clsItem)
    Private _size As Integer

    Public ReadOnly Property Items As List(Of clsItem)
        Get
            Return _items
        End Get
    End Property

    Public ReadOnly Property Size As Integer
        Get
            Return _size
        End Get
    End Property

    Public Sub New(size As Integer)
        _size = size
    End Sub

    Public Sub New(s As String, size As Integer)
        Me.New(clsVial.ColorStringToColorArray(s), size)
    End Sub

    Public Sub New(items As List(Of clsItem), size As Integer)
        Dim s As Integer = 0

        For Each i As clsItem In items
            s += i.Size
        Next

        If s <= size Then
            _size = size
            _items = items
        End If
    End Sub

    Public Sub New(cols() As Colors, size As Integer)

        If cols.Count > size Then Exit Sub

        _size = size

        For Each c As Colors In cols
            If c <> Colors.NONE Then
                'if there are no items yet, create one with this color
                If _items.Count = 0 Then
                    _items.Add(New clsItem(c, 1))
                Else
                    'if there are items but with different color, create a new item
                    If _items.Last().Color <> c Then
                        _items.Add(New clsItem(c, 1))
                    Else
                        'if there is an item with the same color already, add to it's size
                        _items.Last().Size += 1
                    End If
                End If

            End If
        Next

    End Sub

    Public Shared Function ColorStringToColorArray(s As String) As Colors()
        Dim cols As New List(Of Colors)
        Dim c As Colors

        For Each i As String In Enumerable.Range(0, s.Length / 2).[Select](Function(_i) s.Substring(_i * 2, 2))
            Select Case i.ToUpper
                Case "VI"
                    c = Colors.VIOLET
                Case "DG"
                    c = Colors.DGREEN
                Case "OR"
                    c = Colors.ORANGE
                Case "LG"
                    c = Colors.LGREEN
                Case "PI"
                    c = Colors.PINK
                Case "BR"
                    c = Colors.BROWN
                Case "LB"
                    c = Colors.LBLUE
                Case "RE"
                    c = Colors.RED
                Case "DB"
                    c = Colors.DBLUE
                Case "TA"
                    c = Colors.TAN
                Case "GR"
                    c = Colors.GRAY
                Case "YE"
                    c = Colors.YELLOW
                Case Else
                    c = Colors.NONE
            End Select

            cols.Add(c)
        Next

        Return cols.ToArray
    End Function

    ''' <summary>
    ''' returns the amount of available space in the vial
    ''' </summary>
    ''' <returns>the amount of space in the vial (0-4)</returns>
    Public Function VialSpace() As Integer
        Dim s As Integer = 0
        For Each i As clsItem In _items
            s += i.Size
        Next
        Return _size - s
    End Function

    ''' <summary>
    ''' Try to put a color in the vial. If it worked, return true.
    ''' </summary>
    ''' <param name="ItemToPutIn">color to try to put in</param>
    ''' <param name="Execute">(optional) execute the move (true, default) or only check if the move is possible (false)</param>
    ''' <returns>true if it works, false if it doesn't.</returns>
    Public Function PutIn(ItemToPutIn As clsItem, Optional Execute As Boolean = True) As Boolean
        'item invalid
        If ItemToPutIn Is Nothing Then Return False

        'move of a complete vial is pointless
        If ItemToPutIn.Size = _size Then Return False

        'cannot put in air
        If ItemToPutIn.Color = Colors.NONE Then Return False

        'vial too little space
        If VialSpace() < ItemToPutIn.Size Then Return False

        'if vial is empty, put it in without any checks
        If _items.Count < 1 Then
            If Execute Then _items.Add(ItemToPutIn)
            Return True
        End If

        'if there is stuff, only put in if color matches existing stuff

        If _items.Last().Color = ItemToPutIn.Color Then
            If Execute Then _items.Last().Size += ItemToPutIn.Size
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Take out an item from the vial. Return the item taken out (return nothing is there are no items)
    ''' </summary>
    ''' <param name="Execute">(optional) execute the move (true, default) or only check if the move is possible (false)</param>
    ''' <returns>item taken out (or </returns>
    Public Function TakeOut(Optional Execute As Boolean = True) As clsItem
        Dim out As clsItem

        'can't take anything from an empty vial
        If _items.Count < 1 Then Return Nothing

        out = _items(_items.Count - 1)
        If Execute Then
            _items.RemoveAt(_items.Count - 1)
        End If

        Return out

    End Function


    ''' <summary>
    ''' Return color at a specific position
    ''' </summary>
    ''' <param name="p">Position to return the color for</param>
    ''' <returns>The color at the position (or NOTHING if invalid)</returns>
    Public Function GetPosition(p As Integer) As Colors
        Dim offset As Integer = 0

        For Each i As clsItem In _items
            For s As Integer = 1 To i.Size
                If s + offset = p Then
                    Return i.Color
                End If
            Next
            offset += i.Size
        Next

        Return Colors.NONE
    End Function

    ''' <summary>
    ''' Return the purity of the vial (if all positions are filled with same color, the vial is pure and thus locked, otherwise it isn't)
    ''' </summary>
    ''' <returns>true if pure, false otherwise</returns>
    Public Function IsLocked() As Boolean
        'an empty vial is pure by definition
        If _items.Count = 0 Then Return True
        'otherwise it has to be full with ONE color
        If _items.Count = 1 AndAlso _items(0).Size = _size Then Return True
        Return False
    End Function

    Public Function IsMonochrome() As Boolean
        'a vial with zero or one item is monochrome
        Return _items.Count < 2
    End Function

    Public Function IsEmpty() As Boolean
        'a vial is empty if it has no items in it
        Return _items.Count = 0
    End Function

    Public Function VialToString() As String
        Dim s As String = ""
        For i As Integer = 1 To 4
            s &= Hex(GetPosition(i))
        Next
        Return s
    End Function
End Class

Public Enum Colors
    NONE = 0
    DBLUE = 1
    DGREEN = 2
    LBLUE = 3
    VIOLET = 4
    PINK = 5
    LGREEN = 6
    GRAY = 7
    ORANGE = 8
    RED = 9
    BROWN = 10
    YELLOW = 11
    BGREEN = 12
    TAN = 13
End Enum
