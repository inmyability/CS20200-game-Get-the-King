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

The horse moves in an L-shape, but the first straight point must be empty.

```text
┌───┬───┬───┬───┬───┐
│   │ X │   │ X │   │
├───┼───┼───┼───┼───┤
│ X │   │ B │   │ X │
├───┼───┼───┼───┼───┤
│   │ B │ H │ B │   │
├───┼───┼───┼───┼───┤
│ X │   │ B │   │ X │
├───┼───┼───┼───┼───┤
│   │ X │   │ X │   │
└───┴───┴───┴───┴───┘
```

- `H` : horse position
- `X` : possible destination
- `B` : blocking point

If a blocking point is occupied, the two destinations behind it become unavailable.

---

## Elephant (상) and Blocking

The elephant moves farther than the horse and checks two intermediate positions.

```text
┌───┬───┬───┬───┬───┬───┬───┐
│   │ X │   │   │   │ X │   │
├───┼───┼───┼───┼───┼───┼───┤
│ X │   │ B │   │ B │   │ X │
├───┼───┼───┼───┼───┼───┼───┤
│   │ B │   │ B │   │ B │   │
├───┼───┼───┼───┼───┼───┼───┤
│   │   │ B │ E │ B │   │   │
├───┼───┼───┼───┼───┼───┼───┤
│   │ B │   │ B │   │ B │   │
├───┼───┼───┼───┼───┼───┼───┤
│ X │   │ B │   │ B │   │ X │
├───┼───┼───┼───┼───┼───┼───┤
│   │ X │   │   │   │ X │   │
└───┴───┴───┴───┴───┴───┴───┘
```

- `E` : elephant position
- `X` : possible destination
- `B` : intermediate blocking points

The elephant move is blocked if either intermediate position is occupied.

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


## Requirement change log
The original proposal used 9 x 7, but the implementation uses 9 x 10 because real Janggi uses a 9 x 10 board.

## LLM usage
I used LLM for building the Janggi board, which I didn't have any idea for especially the palace.
It also gave help when building when the game ends.
However, I had to deal with the details of the moves of the pieces, which LLM didn't seem to understand.
