namespace PGNConverterService.Models;

using PGNConverterService.Enums;

public class Piece
{
    private PieceClass _Class;
    private readonly ChessColors _Color;
    private readonly ChessSide _Side;
    private readonly Square _Square;

    public Piece(string SAN)
    {
        _Class = new Pawn();
        _Color = ChessColors.White;
        _Side = ChessSide.Queenside;
        _Square = new Square(SAN);
    }

    public Piece(string SAN, ChessColors Color)
    {
        _Class = new Pawn();
        _Color = ChessColors.White;
        _Side = ChessSide.Queenside;
        _Square = new Square(SAN);
        _Color = Color;
    }

    public Piece(string SAN, ChessSide Side)
    {
        _Class = new Pawn();
        _Color = ChessColors.White;
        _Side = ChessSide.Queenside;
        _Square = new Square(SAN);
        _Side = Side;
    }

    public Piece(string SAN, PieceClass Class)
    {
        _Class = new Pawn();
        _Color = ChessColors.White;
        _Side = ChessSide.Queenside;
        _Square = new Square(SAN);
        _Class = Class;
    }

    public Piece(string SAN, ChessColors Color, ChessSide Side)
    {
        _Class = new Pawn();
        _Color = ChessColors.White;
        _Side = ChessSide.Queenside;
        _Square = new Square(SAN);
        _Color = Color;
        _Side = Side;
    }

    public Piece(string SAN, PieceClass Class, ChessColors Color)
    {
        _Class = new Pawn();
        _Color = ChessColors.White;
        _Side = ChessSide.Queenside;
        _Square = new Square(SAN);
        _Class = Class;
        _Color = Color;
    }

    public Piece(string SAN, PieceClass Class, ChessSide Side)
    {
        _Class = new Pawn();
        _Color = ChessColors.White;
        _Side = ChessSide.Queenside;
        _Square = new Square(SAN);
        _Class = Class;
        _Side = Side;
    }

    public Piece(string SAN, PieceClass Class, ChessColors Color, ChessSide Side)
    {
        _Class = new Pawn();
        _Color = ChessColors.White;
        _Side = ChessSide.Queenside;
        _Square = new Square(SAN);
        _Class = Class;
        _Color = Color;
        _Side = Side;
    }

    public void PromoteClass(PieceClass Class)
    {
        _Class = Class;
    }

    public PieceClass Class
    {
        get
        {
            return _Class;
        }
    }

    public ChessColors Color
    {
        get
        {
            return _Color;
        }
    }

    public ChessSide Side
    {
        get
        {
            return _Side;
        }
    }

    public Square Square
    {
        get
        {
            return _Square;
        }
    }
}
