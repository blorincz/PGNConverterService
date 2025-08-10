namespace PGNConverterService.Models;

using PGNConverterService.Enums;

public class Rook : PieceClass
{
    private readonly List<Vector> _CaptureVectors;
    private readonly bool _IterativeMovement;
    private readonly ChessPieceNames _Name = ChessPieceNames.Rook;
    private readonly string _PGN = "R";

    public Rook()
    {
        List<Vector> captureVectors =
            [
                new Vector(0, 1),
                new Vector(0, -1),
                new Vector(1, 0),
                new Vector(-1, 0)
            ];
        _CaptureVectors = captureVectors;
        _IterativeMovement = true;
    }

    public override List<Vector> CaptureVectors
    {
        get
        {
            return _CaptureVectors;
        }
    }

    public override bool IterativeMovement
    {
        get
        {
            return _IterativeMovement;
        }
    }

    public override ChessPieceNames Name
    {
        get
        {
            return _Name;
        }
    }

    public override string PGN
    {
        get
        {
            return _PGN;
        }
    }
}