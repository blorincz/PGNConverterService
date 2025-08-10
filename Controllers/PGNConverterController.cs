namespace PGNConverterService.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;
using PGNConverterService.Models;
using PGNConverterService.Enums;

[ApiController]
[Route("api/[controller]")]
[RequestSizeLimit(10 * 1024 * 1024)] // 10MB
public class PgnConverterController : ControllerBase
{
    private Dictionary<string, Piece> Board;
    private PGN Game;
    private string _enPassantSquare;

    // The HTTP POST endpoint to handle the file upload and conversion.
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertFileToPgn([Required] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        if (!Path.GetExtension(file.FileName).Equals(".chesstitanssave-ms", StringComparison.InvariantCultureIgnoreCase))
        {
            return BadRequest($"The file '{file.FileName}' is not a Chess Titans saved game file.");
        }

        try
        {
            // Create a temporary file to process the uploaded data
            using var tempFile = await CreateTempFileFromUploadAsync(file);

            // Convert the file to PGN
            if (ConvertTitansToPGN(tempFile.Path))
            {
                // Return the PGN text directly as the response body
                return Content(PGNText, "text/plain");
            }
            else
            {
                return BadRequest("PGN conversion failed.");
            }
        }
        catch (Exception ex)
        {
            // Return a 500 Internal Server Error with the exception message
            return StatusCode(500, $"An error occurred while processing the file: {ex.Message}");
        }
    }

    // The following methods are ported directly from your Razor Page model.
    // They should be kept as private helper methods.

    private static async Task<TempFile> CreateTempFileFromUploadAsync(IFormFile uploadedFile)
    {
        const int maxFileSize = 5 * 1024 * 1024;
        if (uploadedFile.Length > maxFileSize)
        {
            throw new InvalidOperationException("File size exceeds maximum allowed");
        }

        var tempFile = new TempFile(Path.GetExtension(uploadedFile.FileName));
        try
        {
            using (var stream = new FileStream(tempFile.Path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(stream);
            }
            if (!IsValidChessFile(tempFile.Path))
            {
                throw new InvalidOperationException("Invalid file content");
            }
            return tempFile;
        }
        catch
        {
            tempFile.Dispose();
            throw;
        }
    }

    private static bool IsValidChessFile(string filePath)
    {
        try
        {
            var content = System.IO.File.ReadAllText(filePath);
            return content.Contains("[Event") && content.Contains("[Site");
        }
        catch
        {
            return false;
        }
    }

    private bool ConvertTitansToPGN(string savePath)
    {
        InitializeBoard();
        string[] a = ParsedFile(savePath).Split(["\n"], StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < a.Length - 1; i++)
        {
            Game.Header = Game.Header + a[i] + "\n";
        }
        string[] m = a[^1].Split([' '], StringSplitOptions.RemoveEmptyEntries);
        List<Move> moves = [];
        foreach (string mv in m)
        {
            Move move = new(mv);
            moves.Add(move);
        }
        bool IsWhiteMove = true;
        int currentTurn = 0;
        _enPassantSquare = null;

        foreach (Move move in moves)
        {
            move.PGN = ConvertToPGN(move.From.SAN, move.To.SAN, move.PromoteTo);
            if (IsWhiteMove)
            {
                Game.Turns.Add(new Turn(currentTurn + 1));
                Game.Turns[currentTurn].WhiteMove = move;
                IsWhiteMove = false;
            }
            else
            {
                Game.Turns[currentTurn].BlackMove = move;
                currentTurn++;
                IsWhiteMove = true;
            }
        }
        PGNText = Game.Header;
        foreach (Turn Turn in Game.Turns)
        {
            PGNText = PGNText + Turn.TurnNumber.ToString() + ". " + Turn.WhiteMove.PGN + " " + Turn.BlackMove.PGN + "\r\n";
        }
        return true;
    }

    private string ConvertToPGN(string FromSquare, string ToSquare, string PromoteTo)
    {
        string pgn;
        bool pieceTaken = Board.ContainsKey(ToSquare);
        Piece FromPiece = Board[FromSquare];
        string disambiguatingNotation = Disambiguate(FromPiece, ToSquare);

        if (FromPiece.Class.Name == ChessPieceNames.Pawn && !pieceTaken && ToSquare == _enPassantSquare)
        {
            pgn = FromPiece.Square.File + "x" + ToSquare;
            string capturedPawnSquare = ToSquare[0].ToString() + FromSquare[1].ToString();
            Board.Remove(capturedPawnSquare);
        }
        else if (FromPiece.Class.Name == ChessPieceNames.Pawn)
        {
            if (pieceTaken)
            {
                pgn = FromPiece.Square.File + "x" + ToSquare;
            }
            else if (!string.IsNullOrEmpty(PromoteTo))
            {
                pgn = ToSquare + "=" + PromotionClass(PromoteTo).PGN;
            }
            else
            {
                pgn = ToSquare;
            }
        }
        else
        {
            pgn = FromPiece.Class.PGN + disambiguatingNotation + (pieceTaken ? "x" : "") + ToSquare;
        }

        Piece pieceToMove = FromPiece;
        if (!string.IsNullOrEmpty(PromoteTo))
        {
            PieceClass newClass = PromotionClass(PromoteTo);
            pieceToMove = new Piece(FromPiece.Square.SAN, newClass, FromPiece.Color, FromPiece.Side);
        }

        Board.Remove(FromSquare);
        Board[ToSquare] = pieceToMove;
        pieceToMove.Square.SAN = ToSquare;

        string move = FromSquare + ToSquare;
        if (move == "e1g1")
        {
            pgn = "O-O";
            Piece rook = Board["h1"];
            Board.Remove("h1");
            Board.Add("f1", rook);
            rook.Square.SAN = "f1";
        }
        else if (move == "e1c1")
        {
            pgn = "O-O-O";
            Piece rook = Board["a1"];
            Board.Remove("a1");
            Board.Add("d1", rook);
            rook.Square.SAN = "d1";
        }
        else if (move == "e8g8")
        {
            pgn = "O-O";
            Piece rook = Board["h8"];
            Board.Remove("h8");
            Board.Add("f8", rook);
            rook.Square.SAN = "f8";
        }
        else if (move == "e8c8")
        {
            pgn = "O-O-O";
            Piece rook = Board["a8"];
            Board.Remove("a8");
            Board.Add("d8", rook);
            rook.Square.SAN = "d8";
        }

        if (FromPiece.Class.Name == ChessPieceNames.Pawn && Math.Abs(new Square(ToSquare).IRank - new Square(FromSquare).IRank) == 2)
        {
            int passedRank = FromPiece.Color == ChessColors.White ? new Square(FromSquare).IRank + 1 : new Square(FromSquare).IRank - 1;
            _enPassantSquare = new Square(new Square(ToSquare).IFile, passedRank).SAN;
        }
        else
        {
            _enPassantSquare = null;
        }

        if (OpponentInCheck(FromPiece.Color))
        {
            pgn += "+";
        }
        return pgn;
    }

    private string Disambiguate(Piece FromPiece, string ToSquare)
    {
        if (FromPiece.Class.Name == ChessPieceNames.Pawn || FromPiece.Class.Name == ChessPieceNames.King)
        {
            return "";
        }

        var otherPieces = Board.Values
            .Where(p => p.Color == FromPiece.Color &&
                        p.Class.Name == FromPiece.Class.Name &&
                        p.Square.SAN != FromPiece.Square.SAN &&
                        FindAllMoves(p).Exists(s => s.SAN == ToSquare))
            .ToList();

        if (otherPieces.Count == 0)
        {
            return "";
        }

        if (!otherPieces.Any(p => p.Square.File == FromPiece.Square.File))
        {
            return FromPiece.Square.File;
        }

        if (!otherPieces.Any(p => p.Square.Rank == FromPiece.Square.Rank))
        {
            return FromPiece.Square.Rank;
        }
        return FromPiece.Square.SAN;
    }

    private List<Square> FindAllMoves(Piece Piece)
    {
        List<Square> moves = [];
        var (iFile, iRank) = (Piece.Square.IFile, Piece.Square.IRank);
        var vectors = Piece.Class.CaptureVectors;
        bool isWhite = Piece.Color == ChessColors.White;

        if (Piece.Class.IterativeMovement)
        {
            foreach (var vector in vectors)
            {
                for (int i = 1; i <= 8; i++)
                {
                    int newFile = iFile + vector.dx * i;
                    int newRank = iRank + vector.dy * i;
                    if (IsOutsideBoard(newFile, newRank))
                    {
                        break;
                    }
                    Square newSquare = new(newFile, newRank);
                    if (Board.ContainsKey(newSquare.SAN))
                    {
                        if (Board[newSquare.SAN].Color != Piece.Color)
                        {
                            moves.Add(newSquare);
                        }
                        break;
                    }
                    else
                    {
                        moves.Add(newSquare);
                    }
                }
            }
        }
        else
        {
            foreach (var vector in vectors)
            {
                int newFile = iFile + vector.dx;
                int newRank = iRank + vector.dy;
                if (!IsOutsideBoard(newFile, newRank))
                {
                    Square newSquare = new(newFile, newRank);
                    if (Piece.Class.Name == ChessPieceNames.Pawn)
                    {
                        if (Board.TryGetValue(newSquare.SAN, out Piece value) && value.Color != Piece.Color)
                        {
                            moves.Add(newSquare);
                        }
                        else if (newSquare.SAN == _enPassantSquare)
                        {
                            moves.Add(newSquare);
                        }
                    }
                    else if (Board.ContainsKey(newSquare.SAN))
                    {
                        if (Board[newSquare.SAN].Color != Piece.Color)
                        {
                            moves.Add(newSquare);
                        }
                    }
                    else
                    {
                        moves.Add(newSquare);
                    }
                }
            }
            if (Piece.Class.Name == ChessPieceNames.Pawn)
            {
                int direction = isWhite ? 1 : -1;
                Square forwardOne = new(iFile, iRank + direction);
                if (!Board.ContainsKey(forwardOne.SAN))
                {
                    moves.Add(forwardOne);
                    if (isWhite && iRank == 2 || !isWhite && iRank == 7)
                    {
                        Square forwardTwo = new(iFile, iRank + 2 * direction);
                        if (!Board.ContainsKey(forwardTwo.SAN))
                        {
                            moves.Add(forwardTwo);
                        }
                    }
                }
            }
        }
        return moves;
    }

    private static bool IsOutsideBoard(int newFile, int newRank)
    {
        return newFile < 1 || newFile > 8 || newRank < 1 || newRank > 8;
    }

    private static string ParsedFile(string savePath)
    {
        FileStream fileStream = new(savePath, FileMode.Open);
        byte[] fileBytes = new byte[fileStream.Length];
        fileStream.Read(fileBytes, 0, (int)fileStream.Length);
        string fileString = new ASCIIEncoding().GetString(fileBytes);
        int indexNotation = fileString.IndexOf("[Event");
        if (indexNotation > -1)
        {
            return fileString[indexNotation..];
        }
        return string.Empty;
    }

    private static PieceClass PromotionClass(string PGN)
    {
        return PGN switch
        {
            "R" => new Rook(),
            "N" => new Knight(),
            "B" => new Bishop(),
            _ => new Queen(),
        };
    }

    private void InitializeBoard()
    {
        Game = new PGN();
        Board = new Dictionary<string, Piece>
        {
            { "a1", new Piece("a1", new Rook()) },
            { "b1", new Piece("b1", new Knight()) },
            { "c1", new Piece("c1", new Bishop()) },
            { "d1", new Piece("d1", new Queen()) },
            { "e1", new Piece("e1", new King(), ChessSide.Kingside) },
            { "f1", new Piece("f1", new Bishop(), ChessSide.Kingside) },
            { "g1", new Piece("g1", new Knight(), ChessSide.Kingside) },
            { "h1", new Piece("h1", new Rook(), ChessSide.Kingside) },
            { "a2", new Piece("a2") },
            { "b2", new Piece("b2") },
            { "c2", new Piece("c2") },
            { "d2", new Piece("d2") },
            { "e2", new Piece("e2", ChessSide.Kingside) },
            { "f2", new Piece("f2", ChessSide.Kingside) },
            { "g2", new Piece("g2", ChessSide.Kingside) },
            { "h2", new Piece("h2", ChessSide.Kingside) },
            { "a8", new Piece("a8", new Rook(), ChessColors.Black) },
            { "b8", new Piece("b8", new Knight(), ChessColors.Black) },
            { "c8", new Piece("c8", new Bishop(), ChessColors.Black) },
            { "d8", new Piece("d8", new Queen(), ChessColors.Black) },
            { "e8", new Piece("e8", new King(), ChessColors.Black, ChessSide.Kingside) },
            { "f8", new Piece("f8", new Bishop(), ChessColors.Black, ChessSide.Kingside) },
            { "g8", new Piece("g8", new Knight(), ChessColors.Black, ChessSide.Kingside) },
            { "h8", new Piece("h8", new Rook(), ChessColors.Black, ChessSide.Kingside) },
            { "a7", new Piece("a7", ChessColors.Black) },
            { "b7", new Piece("b7", ChessColors.Black) },
            { "c7", new Piece("c7", ChessColors.Black) },
            { "d7", new Piece("d7", ChessColors.Black) },
            { "e7", new Piece("e7", ChessColors.Black, ChessSide.Kingside) },
            { "f7", new Piece("f7", ChessColors.Black, ChessSide.Kingside) },
            { "g7", new Piece("g7", ChessColors.Black, ChessSide.Kingside) },
            { "h7", new Piece("h7", ChessColors.Black, ChessSide.Kingside) }
        };
    }

    private bool OpponentInCheck(ChessColors Color)
    {
        ChessColors OpponentColor = Color == ChessColors.White ? ChessColors.Black : ChessColors.White;
        Square KingSquare = Board.Values.FirstOrDefault(p => p.Class.Name == ChessPieceNames.King && p.Color == OpponentColor)?.Square;
        return Board.Values.Any(p =>
        p.Color == Color &&
        FindAllMoves(p).Exists(move => move.SAN == KingSquare.SAN));
    }

    public string PGNText { get; set; }
}
