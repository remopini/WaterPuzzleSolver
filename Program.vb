Imports System

Module Program
    Dim tree As clsSolutionTreeItem
    Dim treeitems As New HashSet(Of String)
    Dim solutions As New List(Of clsSolution)
    Dim strategy As clsSolution.Strategy
    Dim minmoves As Integer = 2000
    Dim movescount As Long = 0
    Dim done As Boolean = False
    Dim start As Long = 0

    Sub Main(args As String())
        'VialTests()
        'BoardTests()
        'TreeTests()

        strategy = clsSolution.Strategy.Shortest

        Init()

        Console.WriteLine("Level to solve:")
        DisplayBoard(tree.board)
        Console.WriteLine()
        Console.WriteLine("Strategy: {0}", strategy)
        Console.WriteLine("Press ESC to stop iterating...")

        If Not tree.board.ValidBoard Then
            Console.WriteLine("Seems like an invalid board:")
            For Each e As String In tree.board.Errors
                Console.WriteLine(e)
            Next
        Else
            start = Now.Ticks
            Recursion(1, tree)
        End If

        'display all solutions
        Dim s As Integer = 0
        Dim found As Boolean = False

        While Not found And s < solutions.Count
            If solutions.ElementAt(s).moves.Count = minmoves Then
                Console.WriteLine("Solved in {0} steps with the following moves:", solutions.ElementAt(s).moves.Count)
                'DisplayBoard(t.board)
                For c As Integer = 0 To Math.Min(solutions.ElementAt(s).moves.Count - 1, 100)
                    Console.WriteLine("{0:00}: {1}", c + 1, solutions.ElementAt(s).moves(c))
                Next
                found = True
            End If
            s += 1
        End While

        Console.WriteLine("Done after {0:n} ms", (Now.Ticks - start) / 10000)

    End Sub

    Sub Recursion(level As Integer, t As clsSolutionTreeItem)

        If done Then Exit Sub

        If Console.KeyAvailable Then
            If Console.ReadKey(True).Key = ConsoleKey.Escape Then
                done = True
                Exit Sub
            End If
        End If

        movescount += 1

        If movescount Mod 100000 = 0 Then Console.WriteLine("Currently on iteration {0:n0} after {1:n} ms", movescount, (Now.Ticks - start) / 10000)

        t.board.UpdateState()

        If t.board.IsSolved Then
            solutions.Add(New clsSolution(t.moves))
            If minmoves > t.moves.Count Then
                minmoves = Math.Min(minmoves, t.moves.Count)
                Console.WriteLine("Found a solution with {0} moves in {1:n0} iterations.", t.moves.Count, movescount)
            End If
            If strategy = clsSolution.Strategy.Any Then done = True
        End If

        If Not done Then
            If t.board.HasValidMoves Then
                If level < 200 Then
                    Dim a As Integer = t.AddAllPossibleChildren(treeitems)
                    If a > 0 Then
                        Dim i As Integer = 0
                        While Not done And i < t.GetChildren.Count
                            Recursion(level + 1, t.GetChildren(i))
                            i += 1
                        End While
                    End If
                End If
            End If
        End If

    End Sub

    Public Sub Init()
        Dim boardstring As String

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

        boardstring = "ORDGORPI REVIGRLG TADGLBOR GRLBPIGR BRDBPIVI REVIDBBR DGBRLGBR YEDBPILG GRRETAYE DBRELBVI LGYEDGTA LBORYETA -------- --------"
        boardstring = "dbtabrre oryelggr orpibrta lglbdbdb pilbtare talbgror yevigrdg vibrdbdg brrepiye vilgpidg lbdgvire yeorgrlg -------- --------" 'lvl 165
        boardstring = "lbdblgpi lgyegrye dbdgviye dbvigrlg vigrrepi gryepidb vidgrere pidglblb lgredglb -------- --------" 'lvl 166
        boardstring = "retadbbr pidbreye lbdgvipi oryedgdg brtalgye pilbvire grorpilg lgyevita grlbbrdb ordblbre lgdgvigr orgrbrta -------- --------" 'lvl 171
        boardstring = "orreredg dglglglb pivilbpi yeorrevi piretaye dgtaorta vipiorvi yelglbdg talblgye -------- --------" 'lvl 172
        boardstring = "vilgyelb pireyebr dglblgor ordggrpi orgrlbre lgvilbdg reordbye vilgbrpi dgtavibr redbgrdb brgrtapi tadbyeta -------- --------" 'lvl 173
        boardstring = "dgdgorgr lgyegrdb lbpibrvi lbdbpigr dbpiorpi tagryeor lgviorlg taviyebr lbredbbr yetavilg redgdgta rerebrlb -------- --------" 'lvl 175
        boardstring = "lgvigrye vipiyedb vipigror dbrelgre tabrorlb taredbbr dbgrrelg lbdgyevi dgpitadg dgorbrgr pilborta brlgyelb -------- --------" 'lvl 184
        boardstring = "lblgvilb pivilbvi lglgpi-- lb------ repilgpi rerevire" 'lvl 186
        boardstring = "lgbryedb dglbreor dblgdgdb piyeyebr dgviorta tavitaor brgrlbdg grdbpire grrelbbr vigryeta virelgpi orpilblg -------- --------" 'lvl 187
        boardstring = "dgbrdgdb vibrlglb pividgta repidbdg talgrelg vibrlbpi dbtatalb lbbrdblg virepire -------- --------" 'lvl 188
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
