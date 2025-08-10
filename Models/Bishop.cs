namespace PGNConverterService.Models;

using PGNConverterService.Enums;

public class Bishop : PieceClass
{
    private readonly List<Vector> _CaptureVectors;
    private readonly bool _IterativeMovement;
    private readonly ChessPieceNames _Name = ChessPieceNames.Bishop;
    private readonly string _PGN = "B";

    public Bishop()
    {
        List<Vector> captureVectors =
            [
                new Vector(-1, 1),
                new Vector(1, 1),
                new Vector(-1, -1),
                new Vector(1, -1)
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