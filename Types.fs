namespace GetTheKing

type Difficulty =
    | Easy
    | Medium
    | Difficult
    | Expert

type Player =
    | User
    | Enemy

type PieceType =
    | King
    | Guard
    | Chariot
    | Cannon
    | Horse
    | Elephant
    | Soldier

type Piece = {
    Owner: Player
    Kind: PieceType
}

type Cell =
    | Empty
    | Occupied of Piece

type Position = int * int
type Move = Position * Position

type GameBoard = Cell[,]
