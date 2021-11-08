Imports System

Module Program
    Dim tree As clsSolutionTreeItem
    Dim treeitems As New HashSet(Of String)

    Sub Main(args As String())
        'VialTests()
        'BoardTests()
        'TreeTests()

        Init()

        Console.WriteLine("Level to solve:")
        DisplayBoard(tree.board)

        Console.WriteLine(String.Format("Seems like a valid board: {0}", IIf(tree.board.ValidBoard(), "YES", "NO")))
        Console.WriteLine()

        Recursion(1, tree)

        Console.WriteLine("Done")

    End Sub

    Sub Recursion(level As Integer, t As clsSolutionTreeItem)

        t.board.UpdateState()

        If t.board.IsSolved Then
            Console.WriteLine(String.Format("Solved in {0} steps :", t.moves.Count))
            DisplayBoard(t.board)
            For c As Integer = 0 To Math.Min(t.moves.Count - 1, 100)
                Console.WriteLine(t.moves(c))
            Next
            End
        Else
            If Not t.board.HasValidMoves Then
                'Console.WriteLine(level & ": Deadend:")
                'DisplayBoard(t.board)
            Else
                If level < 200 Then
                    Dim a As Integer = t.AddAllPossibleChildren(treeitems)
                    If a = 0 Then
                        'Console.WriteLine(level & ": Deadend:")
                        'Display(t.board)
                    Else
                        'Console.WriteLine(a)
                        For Each c As clsSolutionTreeItem In t.GetChildren
                            Recursion(level + 1, c)
                        Next
                    End If

                Else
                    Console.WriteLine("Tree Branch too deep...")
                End If
            End If
        End If

    End Sub

    Public Sub Init()
        'Dim level As New List(Of clsVial)

        'level.Add(New clsVial({Colors.GRAY, Colors.BROWN, Colors.RED, Colors.RED}, 4))
        'level.Add(New clsVial({Colors.VIOLET, Colors.PINK, Colors.RED, Colors.LGREEN}, 4))
        'level.Add(New clsVial({Colors.GRAY, Colors.LGREEN, Colors.YELLOW, Colors.DBLUE}, 4))
        'level.Add(New clsVial({Colors.DGREEN, Colors.DBLUE, Colors.PINK, Colors.BROWN}, 4))
        'level.Add(New clsVial({Colors.VIOLET, Colors.VIOLET, Colors.YELLOW, Colors.DBLUE}, 4))
        'level.Add(New clsVial({Colors.RED, Colors.LGREEN, Colors.BROWN, Colors.LGREEN}, 4))
        'level.Add(New clsVial({Colors.BROWN, Colors.GRAY, Colors.DGREEN, Colors.PINK}, 4))
        'level.Add(New clsVial({Colors.VIOLET, Colors.DBLUE, Colors.GRAY, Colors.DGREEN}, 4))
        'level.Add(New clsVial({Colors.YELLOW, Colors.YELLOW, Colors.DGREEN, Colors.PINK}, 4))
        'level.Add(New clsVial(4))
        'level.Add(New clsVial(4))

        'tree = New clsSolutionTreeItem(New clsBoard(level))

        Dim boardstring As String = "ORDGORPI REVIGRLG TADGLBOR GRLBPIGR BRDBPIVI REVIDBBR DGBRLGBR YEDBPILG GRRETAYE DBRELBVI LGYEDGTA LBORYETA -------- --------"

        tree = New clsSolutionTreeItem(New clsBoard(boardstring))

    End Sub

    Sub DisplayBoard(b As clsBoard)
        Dim s As String = ""
        For l As Integer = b.Vials(0).Size To 1 Step -1
            For Each v As clsVial In b.Vials
                s = Left(IIf(v.GetPosition(l) = Colors.NONE, "  ", v.GetPosition(l).ToString), 2)
                Console.Write(String.Format("|{0}| ", s))
            Next
            Console.WriteLine()
        Next

        For Each v As clsVial In b.Vials
            Console.Write("\--/ ")
        Next
        Console.WriteLine()
        Console.WriteLine()
    End Sub

    Sub DisplayVial(v As clsVial)
        For l As Integer = v.size To 1 Step -1
            Console.WriteLine(String.Format("|{0}|", Left(IIf(v.GetPosition(l) = Colors.NONE, "  ", v.GetPosition(l).ToString), 2)))
        Next
        Console.WriteLine("\--/ ")
        Console.WriteLine()
    End Sub

    Sub VialTests()
        Dim v As New clsVial(4)
        Console.WriteLine("Create new empty vial")
        For Each c As clsItem In v.Items
            Console.WriteLine(c.ToString)
        Next
        Console.WriteLine()

        DisplayVial(v)

        Console.Write("Add item of size 2 and color gray (should work): ")
        Console.WriteLine(v.PutIn(New clsItem(Colors.GRAY, 2)))
        For Each c As clsItem In v.Items
            Console.WriteLine(c.ToString)
        Next
        Console.WriteLine()

        DisplayVial(v)

        Console.Write("Add item of size 1 and color orange (should fail): ")
        Console.WriteLine(v.PutIn(New clsItem(Colors.ORANGE, 1)))
        For Each c As clsItem In v.Items
            Console.WriteLine(c.ToString)
        Next
        Console.WriteLine()

        DisplayVial(v)

        Console.Write("Add item of size 3 and color gray (should fail): ")
        Console.WriteLine(v.PutIn(New clsItem(Colors.GRAY, 3)))
        For Each c As clsItem In v.Items
            Console.WriteLine(c.ToString)
        Next
        Console.WriteLine()

        DisplayVial(v)

        Console.Write("Add item of size 1 and color gray (should work): ")
        Console.WriteLine(v.PutIn(New clsItem(Colors.GRAY, 1)))
        For Each c As clsItem In v.Items
            Console.WriteLine(c.ToString)
        Next
        Console.WriteLine()

        DisplayVial(v)

        Console.Write("show item that can be retrieved (should give gray/3): ")
        Console.WriteLine(v.TakeOut(False).ToString)
        For Each c As clsItem In v.Items
            Console.WriteLine(c.ToString)
        Next
        Console.WriteLine()

        Console.Write("verify that item can be added (should work): ")
        Console.WriteLine(v.PutIn(New clsItem(Colors.GRAY, 1), False))
        For Each c As clsItem In v.Items
            Console.WriteLine(c.ToString)
        Next
        Console.WriteLine()

    End Sub

    Sub BoardTests()
        Dim board As clsBoard

        'create  board with 3 empty vials of size 2
        board = New clsBoard(New List(Of clsVial)({New clsVial(2), New clsVial(2), New clsVial(2)}))
        board.UpdateState()
        Console.WriteLine("Solved/Valid Moves Left: {0}/{1} (should be true/0)", board.IsSolved, board.ValidMovesLeft.Count)
        DisplayBoard(board)

        'create board with 2 empty and 1 full monochrome vials of size 3
        board = New clsBoard(New List(Of clsVial)({New clsVial(3), New clsVial(3), New clsVial({Colors.PINK, Colors.PINK, Colors.PINK}, 3)}))
        board.UpdateState()
        Console.WriteLine("Solved/Valid Moves Left: {0}/{1} (should be true/2)", board.IsSolved, board.ValidMovesLeft.Count)
        DisplayBoard(board)

        'create board with 2 empty and 1 full polychrome vials of size 4
        board = New clsBoard(New List(Of clsVial)({New clsVial(4), New clsVial(4), New clsVial({Colors.PINK, Colors.PINK, Colors.DBLUE, Colors.DBLUE}, 4)}))
        board.UpdateState()
        Console.WriteLine("Solved/Valid Moves Left: {0}/{1} (should be false/2)", board.IsSolved, board.ValidMovesLeft.Count)
        For Each i As clsMove In board.ValidMovesLeft
            Console.WriteLine("{0}->{1}", i.FromVial + 1, i.ToVial + 1)
        Next

        DisplayBoard(board)

        'create board with 1 empty and 2 partially filled polychrome vials of size 5
        board = New clsBoard(New List(Of clsVial)({New clsVial(5), New clsVial({Colors.DBLUE}, 5), New clsVial({Colors.PINK, Colors.PINK, Colors.DBLUE, Colors.DBLUE}, 5)}))
        board.UpdateState()
        Console.WriteLine("Solved/Valid Moves Left: {0}/{1} (should be false/4)", board.IsSolved, board.ValidMovesLeft.Count)
        For Each i As clsMove In board.ValidMovesLeft
            Console.WriteLine("{0}->{1}", i.FromVial + 1, i.ToVial + 1)
        Next

        DisplayBoard(board)

        'clone a board, make one move on the new board, they should differ now...

        Dim board2 As clsBoard = board.DeepClone
        Console.WriteLine("Make a move, should work: {0}", board2.Move(2, 1))

        Console.WriteLine("Old board (should look like above):")

        DisplayBoard(board)

        Console.WriteLine("New board (should have the move done):")

        DisplayBoard(board2)


        End

    End Sub

    Sub TreeTests()
        Dim board As clsBoard
        Dim tree As clsSolutionTreeItem
        Dim history As New HashSet(Of String)

        'create  board with 3 vials of size 2 two filled with alternate colors
        board = New clsBoard(New List(Of clsVial)({New clsVial({Colors.YELLOW, Colors.BROWN}, 2), New clsVial(2), New clsVial({Colors.YELLOW, Colors.BROWN}, 2)}))
        tree = New clsSolutionTreeItem(board)

        tree.board.UpdateState()

        Console.WriteLine("Solved/Valid Moves Left: {0}/{1} (should be false/4)", board.IsSolved, board.ValidMovesLeft.Count)
        Console.WriteLine("Moves avail: " & String.Join(" ", tree.board.ValidMovesLeft))
        Console.WriteLine("Moves done:  " & String.Join(" ", tree.moves))

        DisplayBoard(board)

        'do first level
        tree.AddAllPossibleChildren(history)

        For Each s As clsSolutionTreeItem In tree.GetChildren
            Console.WriteLine("Child: Solved/Valid Moves Left: {0}/{1}", s.board.IsSolved, s.board.ValidMovesLeft.Count)
            Console.WriteLine("Moves avail: " & String.Join(" ", s.board.ValidMovesLeft))
            Console.WriteLine("Moves done:  " & String.Join(" ", s.moves))
            DisplayBoard(s.board)
        Next

        'do second level for first child only
        tree.GetChildren.First.AddAllPossibleChildren(history)

        For Each s As clsSolutionTreeItem In tree.GetChildren.First.GetChildren
            Console.WriteLine("Grandchild: Solved/Valid Moves Left: {0}/{1}", s.board.IsSolved, s.board.ValidMovesLeft.Count)
            Console.WriteLine("Moves avail: " & String.Join(" ", s.board.ValidMovesLeft))
            Console.WriteLine("Moves done:  " & String.Join(" ", s.moves))
            DisplayBoard(s.board)
        Next

        End
    End Sub

End Module
