namespace PGNConverterService.Models;

public class Square
{
    private string _File = "";
    private int _iFile = 0;
    private int _iRank = 0;
    private string _Rank = "";
    private string _SAN = "";

    // Constructor for SAN notation string
    public Square(string SAN)
    {
        _SAN = SAN;
        InitializeRankFile();
    }

    // New constructor for integer file and rank
    public Square(int iFile, int iRank)
    {
        _iFile = iFile;
        _iRank = iRank;
        UpdateSAN();
        InitializeRankFile();
    }

    private static int ConvertAlphaToNumeric(string Alpha)
    {
        return Alpha.ToLower().ToCharArray()[0] - '`';
    }

    private static string ConvertNumericToAlpha(int Numeric)
    {
        return Convert.ToChar(Numeric + 0x60).ToString();
    }

    private void InitializeRankFile()
    {
        _File = _SAN[..1];
        _Rank = _SAN.Substring(1, 1);
        _iFile = ConvertAlphaToNumeric(_File);
        _iRank = Convert.ToInt32(_Rank);
    }

    private void UpdateSAN()
    {
        _SAN = ConvertNumericToAlpha(_iFile) + _iRank.ToString();
    }

    public string File
    {
        get
        {
            return _File;
        }
    }

    public int IFile
    {
        get
        {
            return _iFile;
        }
        set
        {
            _iFile = value;
            if ((_iRank > 0) && (_iRank < 9) && (_iFile > 0) && (_iFile < 9))
            {
                UpdateSAN();
            }
        }
    }

    public int IRank
    {
        get
        {
            return _iRank;
        }
        set
        {
            _iRank = value;
            if ((_iRank > 0) && (_iRank < 9) && (_iFile > 0) && (_iFile < 9))
            {
                UpdateSAN();
            }
        }
    }

    public string Rank
    {
        get
        {
            return _Rank;
        }
    }

    public string SAN
    {
        get
        {
            return _SAN;
        }
        set
        {
            _SAN = value;
            InitializeRankFile();
        }
    }
}
