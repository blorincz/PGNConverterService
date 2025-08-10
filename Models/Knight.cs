namespace PGNConverterService.Models;

using PGNConverterService.Enums;

public class Knight : PieceClass
{
    private readonly List<Vector> _CaptureVectors;
    private readonly bool _IterativeMovement;
    private readonly ChessPieceNames _Name = ChessPieceNames.Knight;
    private readonly string _PGN = "N";

    public Knight()
    {
        List<Vector> captureVectors =
        [
            new(1, 2),
                new(1, -2),
                new(2, 1),
                new(2, -1),
                new(-1, 2),
                new(-1, -2),
                new(-2, 1),
                new(-2, -1)
        ];
        _CaptureVectors = captureVectors;
        _IterativeMovement = false;
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