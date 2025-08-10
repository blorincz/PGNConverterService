namespace PGNConverterService;

public sealed class TempFile(string extension = ".tmp", bool deleteOnDispose = true) : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            $"{Guid.NewGuid()}{extension}");
    private readonly bool _deleteOnDispose = deleteOnDispose;

    public void Dispose()
    {
        if (_deleteOnDispose)
        {
            try
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                }
            }
            catch (IOException)
            {
                // Log if needed, but don't throw in Dispose
            }
        }
    }

    public static TempFile CreateFromStream(Stream stream, string extension = ".tmp")
    {
        var tempFile = new TempFile(extension);
        try
        {
            using (var fileStream = new FileStream(tempFile.Path, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
            return tempFile;
        }
        catch
        {
            tempFile.Dispose();
            throw;
        }
    }
}
