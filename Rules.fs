namespace GetTheKing

module Rules =

    let private sign (n: int) =
        if n > 0 then 1
        elif n < 0 then -1
        else 0

    let private absInt (n: int) = System.Math.Abs n

    let private opponent = function
        | User -> Enemy
        | Enemy -> User

    let private palacePositions owner =
        let ys =
            match owner with
            | Enemy -> [0; 1; 2]
            | User -> [7; 8; 9]

        Set.ofList [
            for y in ys do
                for x in [3; 4; 5] do
                    yield (x, y)
        ]

    let private palaceDiagonalEdges owner =
        match owner with
        | Enemy ->
            Set.ofList [
                ((3, 0), (4, 1)); ((4, 1), (3, 0))
                ((5, 0), (4, 1)); ((4, 1), (5, 0))
                ((3, 2), (4, 1)); ((4, 1), (3, 2))
                ((5, 2), (4, 1)); ((4, 1), (5, 2))
            ]
        | User ->
            Set.ofList [
                ((3, 7), (4, 8)); ((4, 8), (3, 7))
                ((5, 7), (4, 8)); ((4, 8), (5, 7))
                ((3, 9), (4, 8)); ((4, 8), (3, 9))
                ((5, 9), (4, 8)); ((4, 8), (5, 9))
            ]

    let private allPalaceDiagonalEdges =
        Set.union (palaceDiagonalEdges Enemy) (palaceDiagonalEdges User)

    let private inPalace owner pos =
        Set.contains pos (palacePositions owner)

    let private isOwnPalaceDiagonalStep owner fromPos toPos =
        Set.contains (fromPos, toPos) (palaceDiagonalEdges owner)

    let private isAnyPalaceDiagonalStep fromPos toPos =
        Set.contains (fromPos, toPos) allPalaceDiagonalEdges

    let private pathBetweenStraight (x1, y1) (x2, y2) =
        if x1 = x2 then
            let step = sign (y2 - y1)
            [ for y in y1 + step .. step .. y2 - step -> (x1, y) ]
        elif y1 = y2 then
            let step = sign (x2 - x1)
            [ for x in x1 + step .. step .. x2 - step -> (x, y1) ]
        else
            []

    let private pathBetweenPalaceDiagonal fromPos toPos =
        // The only non-adjacent palace diagonal movement is corner -> opposite corner,
        // whose middle point is the palace center.
        match fromPos, toPos with
        | (3, 0), (5, 2) | (5, 2), (3, 0)
        | (5, 0), (3, 2) | (3, 2), (5, 0) -> [(4, 1)]
        | (3, 7), (5, 9) | (5, 9), (3, 7)
        | (5, 7), (3, 9) | (3, 9), (5, 7) -> [(4, 8)]
        | _ -> []

    let private isPalaceDiagonalLine fromPos toPos =
        isAnyPalaceDiagonalStep fromPos toPos
        || not (List.isEmpty (pathBetweenPalaceDiagonal fromPos toPos))

    let private isStraight fromPos toPos =
        fst fromPos = fst toPos || snd fromPos = snd toPos

    let private linePath fromPos toPos =
        if isStraight fromPos toPos then pathBetweenStraight fromPos toPos
        elif isPalaceDiagonalLine fromPos toPos then pathBetweenPalaceDiagonal fromPos toPos
        else []

    let private isClearLine board fromPos toPos =
        (isStraight fromPos toPos || isPalaceDiagonalLine fromPos toPos)
        && linePath fromPos toPos |> List.forall (Board.isEmpty board)

    let private countScreens board fromPos toPos =
        linePath fromPos toPos
        |> List.filter (fun pos -> not (Board.isEmpty board pos))
        |> List.length

    let private screenIsValidForCannon board fromPos toPos =
        linePath fromPos toPos
        |> List.tryFind (fun pos -> not (Board.isEmpty board pos))
        |> function
            | Some pos ->
                match Board.tryPiece board pos with
                | Some p -> p.Kind <> Cannon
                | None -> false
            | None -> false

    let private validChariot board fromPos toPos =
        isClearLine board fromPos toPos

    let private validCannon board fromPos toPos owner =
        if not (isStraight fromPos toPos || isPalaceDiagonalLine fromPos toPos) then
            false
        else
            let targetIsCannon =
                match Board.tryPiece board toPos with
                | Some p -> p.Kind = Cannon
                | None -> false

            countScreens board fromPos toPos = 1
            && screenIsValidForCannon board fromPos toPos
            && not targetIsCannon
            && not (Board.hasOwnPiece board toPos owner)

    let private validHorse board (x1, y1) (x2, y2) =
        let dx = x2 - x1
        let dy = y2 - y1

        match dx, dy with
        | 2, 1 | 2, -1 -> Board.isEmpty board (x1 + 1, y1)
        | -2, 1 | -2, -1 -> Board.isEmpty board (x1 - 1, y1)
        | 1, 2 | -1, 2 -> Board.isEmpty board (x1, y1 + 1)
        | 1, -2 | -1, -2 -> Board.isEmpty board (x1, y1 - 1)
        | _ -> false

    let private validElephant board (x1, y1) (x2, y2) =
        let dx = x2 - x1
        let dy = y2 - y1

        match dx, dy with
        | 3, 2 | 3, -2 ->
            Board.isEmpty board (x1 + 1, y1)
            && Board.isEmpty board (x1 + 2, y1 + sign dy)
        | -3, 2 | -3, -2 ->
            Board.isEmpty board (x1 - 1, y1)
            && Board.isEmpty board (x1 - 2, y1 + sign dy)
        | 2, 3 | -2, 3 ->
            Board.isEmpty board (x1, y1 + 1)
            && Board.isEmpty board (x1 + sign dx, y1 + 2)
        | 2, -3 | -2, -3 ->
            Board.isEmpty board (x1, y1 - 1)
            && Board.isEmpty board (x1 + sign dx, y1 - 2)
        | _ -> false

    let private validSoldier owner (x1, y1) (x2, y2) =
        let dx = x2 - x1
        let dy = y2 - y1

        let forward =
            match owner with
            | User -> -1
            | Enemy -> 1

        (dx = 0 && dy = forward)
        || (absInt dx = 1 && dy = 0)

    let private validKingLike owner (x1, y1) (x2, y2) =
        let dx = absInt (x2 - x1)
        let dy = absInt (y2 - y1)
        let fromPos = (x1, y1)
        let toPos = (x2, y2)

        inPalace owner toPos
        && (
            (dx + dy = 1)
            || (dx = 1 && dy = 1 && isOwnPalaceDiagonalStep owner fromPos toPos)
        )

    let private isValidRawMove board ((fromPos, toPos): Move) player =
        if fromPos = toPos then
            false
        elif not (Board.inRange fromPos && Board.inRange toPos) then
            false
        elif Board.hasOwnPiece board toPos player then
            false
        else
            match Board.tryPiece board fromPos with
            | None -> false
            | Some piece when piece.Owner <> player -> false
            | Some piece ->
                match piece.Kind with
                | Chariot -> validChariot board fromPos toPos
                | Cannon -> validCannon board fromPos toPos player
                | Horse -> validHorse board fromPos toPos
                | Elephant -> validElephant board fromPos toPos
                | Soldier -> validSoldier player fromPos toPos
                | King -> validKingLike player fromPos toPos
                | Guard -> validKingLike player fromPos toPos

    let private copyBoard (board: GameBoard) =
        Array2D.init Board.height Board.width (fun y x -> board[y, x])

    let private applyMoveOnCopy board move =
        let copied = copyBoard board
        Board.applyMove copied move
        copied

    let private findKing board owner =
        seq {
            for y in 0 .. Board.height - 1 do
                for x in 0 .. Board.width - 1 do
                    match Board.tryPiece board (x, y) with
                    | Some p when p.Owner = owner && p.Kind = King -> yield Some (x, y)
                    | _ -> yield None
        }
        |> Seq.choose id
        |> Seq.tryHead

    let private isSquareAttackedBy board attacker target =
        seq {
            for y in 0 .. Board.height - 1 do
                for x in 0 .. Board.width - 1 do
                    match Board.tryPiece board (x, y) with
                    | Some p when p.Owner = attacker ->
                        yield isValidRawMove board ((x, y), target) attacker
                    | _ -> yield false
        }
        |> Seq.exists id

    let private isKingInCheck board player =
        match findKing board player with
        | None -> true
        | Some kingPos -> isSquareAttackedBy board (opponent player) kingPos

    let isValidMove board move player =
        isValidRawMove board move player
        && not (isKingInCheck (applyMoveOnCopy board move) player)

    let allLegalMoves board player =
        [
            for y1 in 0 .. Board.height - 1 do
                for x1 in 0 .. Board.width - 1 do
                    match Board.tryPiece board (x1, y1) with
                    | Some p when p.Owner = player ->
                        for y2 in 0 .. Board.height - 1 do
                            for x2 in 0 .. Board.width - 1 do
                                let move = ((x1, y1), (x2, y2))
                                if isValidMove board move player then
                                    yield move
                    | _ -> ()
        ]
