namespace PGNConverterService.Models;

public class PGN
{
    private string _Header;
    private List<Turn> _Turns = [];

    public string Header
    {
        get
        {
            return _Header;
        }
        set
        {
            _Header = value;
        }
    }

    public List<Turn> Turns
    {
        get
        {
            return _Turns;
        }
        set
        {
            _Turns = value;
        }
    }
}
