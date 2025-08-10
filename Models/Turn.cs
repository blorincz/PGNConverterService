namespace PGNConverterService.Models;

public class Turn(int TurnNumber)
{
    private readonly int _TurnNumber = TurnNumber;
    public Move BlackMove = new();
    public Move WhiteMove = new();

    public int TurnNumber
    {
        get
        {
            return _TurnNumber;
        }
    }
}