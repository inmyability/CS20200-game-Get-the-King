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
- Chariot moves along straight lines and palace diagonal lines.
- Cannon moves along straight lines and palace diagonal lines by jumping exactly one screen.
- Cannon cannot jump over another cannon.
- Cannon cannot capture another cannon.
- Horse movement includes leg blocking.
- Elephant movement includes blocking points.
- Soldier moves forward or sideways by one point.
- King and guard move inside their own palace.
- A player cannot make a move that leaves its own king in check.

## Edit puzzles
Difficulty-specific puzzle layouts are in `Board.fs`, inside `createInitialBoard`.

## Requirement change log
The original proposal used 9 x 7, but the implementation uses 9 x 10 because real Janggi uses a 9 x 10 board.
