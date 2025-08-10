namespace PGNConverterService.Models;

using PGNConverterService.Enums;

public abstract class PieceClass
{
    protected PieceClass()
    {
    }

    public abstract List<Vector> CaptureVectors { get; }

    public abstract bool IterativeMovement { get; }

    public abstract ChessPieceNames Name { get; }

    public abstract string PGN { get; }
}
