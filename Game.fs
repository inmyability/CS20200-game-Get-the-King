namespace GetTheKing

open System

module Game =

    let rng = Random ()

    let chooseDifficulty () =
        printfn "Choose difficulty:"
        printfn "1. Easy"
        printfn "2. Medium"
        printfn "3. Difficult"
        printfn "4. Expert"

        match Console.ReadLine() with
        | "1" | "easy" | "Easy" -> Easy
        | "2" | "medium" | "Medium" -> Medium
        | "3" | "difficult" | "Difficult" -> Difficult
        | "4" | "expert" | "Expert" -> Expert
        | _ ->
            printfn "Invalid input. Defaulting to Easy."
            Easy

    let private chooseEnemyMove difficulty board =
        let moves = Rules.allLegalMoves board Enemy

        match moves with
        | [] -> None
        | _ ->
            match difficulty with
            | Easy ->
                Some moves[rng.Next moves.Length]

            | Medium | Difficult | Expert ->
                let kingCaptures =
                    moves
                    |> List.filter (fun (_, toPos) ->
                        match Board.tryPiece board toPos with
                        | Some p -> p.Owner = User && p.Kind = King
                        | None -> false)

                if not kingCaptures.IsEmpty then
                    Some kingCaptures[0]
                else
                    Some moves[rng.Next moves.Length]

    let private printMove ((x1, y1), (x2, y2)) =
        printfn "Enemy moves: %d %d -> %d %d" x1 y1 x2 y2

    let rec loop difficulty board userTurns =
        Board.printBoard board

        if not (Board.kingExists board Enemy) then
            printfn "You win!"
        elif not (Board.kingExists board User) then
            printfn "Defeat!"
        elif userTurns >= 100 then
            printfn "Draw! The game reached 100 user moves."
        elif Rules.allLegalMoves board Enemy |> List.isEmpty then
            printfn "You win! Enemy has no legal move."
        else
            printf "> "

            let input = Console.ReadLine()

            match Board.parseMove input with
            | None ->
                printfn "Invalid input. Defeat!"
            | Some move ->
                if Rules.isValidMove board move User then
                    Board.applyMove board move

                    if not (Board.kingExists board Enemy) then
                        Board.printBoard board
                        printfn "You win!"
                    else
                        match chooseEnemyMove difficulty board with
                        | None ->
                            Board.printBoard board
                            printfn "You win! Enemy has no legal move."
                        | Some enemyMove ->
                            printMove enemyMove
                            Board.applyMove board enemyMove

                            if not (Board.kingExists board User) then
                                Board.printBoard board
                                printfn "Defeat!"
                            else
                                loop difficulty board (userTurns + 1)
                else
                    printfn "Invalid move. Defeat!"

    let start () =
        Console.OutputEncoding <- Text.Encoding.UTF8
        printfn "=== Get the King ==="
        printfn "Board: 9 x 10 Janggi-style board."
        printfn "Goal: capture the enemy king or make the enemy have no legal move."
        let difficulty = chooseDifficulty ()
        let board = Board.createInitialBoard difficulty
        loop difficulty board 0
