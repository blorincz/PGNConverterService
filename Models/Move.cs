namespace PGNConverterService.Models;

public class Move
{
    private Square _From;
    private string _PGN;
    private string _PromoteTo;
    private Square _To;

    public Move()
    {
        _PGN = "";
        _PromoteTo = "";
    }

    public Move(string mv)
    {
        _PGN = "";
        _PromoteTo = "";
        _From = new Square(mv[..2]);
        _To = new Square(mv.Substring(2, 2));
        if (mv.Length > 4)
        {
            string[] a = mv.Split(separator);
            _PromoteTo = a[1];
        }
    }

    public Square From
    {
        get
        {
            return _From;
        }
        set
        {
            _From = value;
        }
    }

    public string PGN
    {
        get
        {
            return _PGN;
        }
        set
        {
            _PGN = value;
        }
    }

    public string PromoteTo
    {
        get
        {
            return _PromoteTo;
        }
        set
        {
            _PromoteTo = value;
        }
    }

    public Square To
    {
        get
        {
            return _To;
        }
        set
        {
            _To = value;
        }
    }
    private static readonly char[] separator = ['='];
}