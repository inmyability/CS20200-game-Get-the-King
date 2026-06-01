# Get the King
Console-based Janggi-inspired endgame written in F#.

## Run
```powershell
dotnet run
```

## Input
Enter a move as four numbers:

```text
x1 y1 x2 y2
```

Example:

```text
0 9 0 8
```

## Implemented rules
- Soldier moves forward or sideways by one point.
- King and guard move inside their own palace.
- A player cannot make a move that leaves its own king in check.

## Palace (궁성)
The palace is the 3×3 area at the center of each side of the board.

```text
Enemy Palace

┌───┬───┬───┐
│ ╲ │   │ ╱ │
├───┼───┼───┤
│   │ ╳ │   │
├───┼───┼───┤
│ ╱ │   │ ╲ │
└───┴───┴───┘
```

- The king (`K/k`) and guards (`G/g`) must stay inside the palace.
- Kings and guards may move one step vertically or horizontally.
- Diagonal movement is only allowed along the palace diagonal lines.

---

## Horse (마) and Leg Blocking

The horse moves one step straight and then one step diagonally.

```text
Possible horse movement:

    X
  X   X
    H
  X   X
    X
```

However, if the first straight step is occupied, the horse cannot move in that direction.

Example:

```text
Blocked horse movement

    .
    #   <- blocking piece
    H
```

The horse cannot move upward because the adjacent square is blocked.

---

## Elephant (상) and Blocking

The elephant moves three points horizontally and two vertically,
or three vertically and two horizontally.

```text
Example elephant movement:

        X

   X         X

        E

   X         X

        X
```

Unlike the horse, the elephant checks TWO intermediate positions.
If either position is occupied, the movement is blocked.

Example:

```text
Blocked elephant movement

      target
        X

        #   <- blocking piece

    #

        E
```

The elephant cannot move because one of the intermediate positions is occupied.

---

## Cannon (포)

The cannon must jump over exactly ONE piece (called a screen).

```text
Valid cannon movement:

P --- S --- X

P : cannon
S : screen piece
X : destination
```

Rules:

- The cannon must jump over exactly one piece.
- The cannon cannot jump over another cannon.
- The cannon cannot capture another cannon.
- Palace diagonal cannon movement is supported.

---

## Chariot (차)

The chariot moves any number of points horizontally or vertically.

```text
R ───────── X
```

- The path must be clear.
- Palace diagonal movement is supported.

---

## Edit puzzles
Difficulty-specific puzzle layouts are in `Board.fs`, inside `createInitialBoard`.

## Requirement change log
The original proposal used 9 x 7, but the implementation uses 9 x 10 because real Janggi uses a 9 x 10 board.
