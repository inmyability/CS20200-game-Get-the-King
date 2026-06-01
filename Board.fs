namespace GetTheKing

module Board =

    let width = 9
    let height = 10

    let emptyBoard () : GameBoard =
        Array2D.create height width Empty

    let private putPiece (board: GameBoard) x y owner kind =
        board[y, x] <- Occupied { Owner = owner; Kind = kind }

    let createInitialBoard (difficulty: Difficulty) : GameBoard =
        let board = emptyBoard ()

        match difficulty with
        | Easy ->
            putPiece board 4 1 Enemy King
            putPiece board 4 8 User King
            putPiece board 3 9 User Guard
            putPiece board 5 9 User Guard
            putPiece board 4 9 User Chariot
            putPiece board 0 6 User Soldier
            putPiece board 8 3 Enemy Soldier

        | Medium ->
            // Your sample puzzle.
            putPiece board 4 0 Enemy King
            putPiece board 4 8 User King
            putPiece board 4 7 User Cannon
            putPiece board 4 2 Enemy Cannon
            putPiece board 3 0 Enemy Cannon
            putPiece board 2 3 User Elephant
            putPiece board 3 1 Enemy Guard
            putPiece board 4 1 Enemy Guard
            putPiece board 1 0 User Chariot
            putPiece board 8 5 User Chariot

        | Difficult ->
            putPiece board 4 1 Enemy King
            putPiece board 3 0 Enemy Guard
            putPiece board 5 1 Enemy Guard
            putPiece board 3 1 Enemy Cannon
            putPiece board 4 2 Enemy Cannon
            putPiece board 3 2 Enemy Horse
            putPiece board 2 0 Enemy Chariot
            putPiece board 8 2 Enemy Chariot
            putPiece board 4 9 User King
            putPiece board 4 8 User Guard
            putPiece board 5 9 User Guard
            putPiece board 0 2 User Chariot
            putPiece board 4 3 User Chariot
            putPiece board 3 7 User Horse
            putPiece board 1 9 User Elephant
            putPiece board 4 7 User Cannon
            putPiece board 1 3 Enemy Soldier
            putPiece board 2 4 Enemy Soldier
            putPiece board 5 4 Enemy Soldier
            putPiece board 1 6 User Soldier
            putPiece board 3 6 User Soldier
            putPiece board 6 6 User Soldier
            putPiece board 7 6 User Soldier

        | Expert ->
            putPiece board 3 1 Enemy King
            putPiece board 4 7 User King
            putPiece board 4 8 User Cannon
            putPiece board 5 9 User Cannon
            putPiece board 3 9 User Guard

        board

    let inRange (x, y) =
        x >= 0 && x < width && y >= 0 && y < height

    let getCell (board: GameBoard) (x, y) =
        board[y, x]

    let setCell (board: GameBoard) (x, y) cell =
        board[y, x] <- cell

    let isEmpty board pos =
        match getCell board pos with
        | Empty -> true
        | _ -> false

    let tryPiece board pos =
        match getCell board pos with
        | Occupied p -> Some p
        | Empty -> None

    let hasOwnPiece board pos owner =
        match tryPiece board pos with
        | Some p -> p.Owner = owner
        | None -> false

    let pieceChar = function
        | { Owner = User; Kind = King } -> "K"
        | { Owner = User; Kind = Guard } -> "G"
        | { Owner = User; Kind = Chariot } -> "R"
        | { Owner = User; Kind = Cannon } -> "P"
        | { Owner = User; Kind = Horse } -> "H"
        | { Owner = User; Kind = Elephant } -> "E"
        | { Owner = User; Kind = Soldier } -> "S"
        | { Owner = Enemy; Kind = King } -> "k"
        | { Owner = Enemy; Kind = Guard } -> "g"
        | { Owner = Enemy; Kind = Chariot } -> "r"
        | { Owner = Enemy; Kind = Cannon } -> "p"
        | { Owner = Enemy; Kind = Horse } -> "h"
        | { Owner = Enemy; Kind = Elephant } -> "e"
        | { Owner = Enemy; Kind = Soldier } -> "s"

    let private cellContent (board: GameBoard) x y =
        match board[y, x] with
        | Occupied p -> pieceChar p
        | Empty ->
            match x, y with
            | 4, 1 | 4, 8 -> "╳"
            | 3, 0 | 5, 0 | 3, 2 | 5, 2
            | 3, 7 | 5, 7 | 3, 9 | 5, 9 -> "╲"
            | _ -> " "

    let printBoard (board: GameBoard) =
        let top    = "   ┌───┬───┬───┬───┬───┬───┬───┬───┬───┐"
        let middle = "   ├───┼───┼───┼───┼───┼───┼───┼───┼───┤"
        let bottom = "   └───┴───┴───┴───┴───┴───┴───┴───┴───┘"

        printfn ""
        printfn "     0   1   2   3   4   5   6   7   8"
        printfn "%s" top

        for y in 0 .. height - 1 do
            printf "%2d │" y

            for x in 0 .. width - 1 do
                printf " %s │" (cellContent board x y)

            printfn ""

            if y = height - 1 then
                printfn "%s" bottom
            else
                printfn "%s" middle

        printfn ""
        printfn "User : K=궁 G=사 R=차 P=포 H=마 E=상 S=병"
        printfn "Enemy: k=궁 g=사 r=차 p=포 h=마 e=상 s=병"
        printfn "Input: x1 y1 x2 y2   Example: 0 9 0 8"
        printfn ""

    let parseMove (input: string) =
        let parts =
            input.Replace(",", " ")
                 .Replace("/", " ")
                 .Split(' ', System.StringSplitOptions.RemoveEmptyEntries)

        if parts.Length <> 4 then
            None
        else
            try
                let x1 = int parts[0]
                let y1 = int parts[1]
                let x2 = int parts[2]
                let y2 = int parts[3]
                let fromPos = (x1, y1)
                let toPos = (x2, y2)

                if inRange fromPos && inRange toPos then
                    Some (fromPos, toPos)
                else
                    None
            with
            | _ -> None

    let applyMove (board: GameBoard) (fromPos, toPos) =
        let piece = getCell board fromPos
        setCell board fromPos Empty
        setCell board toPos piece

    let kingExists (board: GameBoard) owner =
        seq {
            for y in 0 .. height - 1 do
                for x in 0 .. width - 1 do
                    yield getCell board (x, y)
        }
        |> Seq.exists (function
            | Occupied p when p.Owner = owner && p.Kind = King -> true
            | _ -> false)
