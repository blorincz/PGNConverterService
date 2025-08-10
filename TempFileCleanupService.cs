namespace PGNConverterService;

public class TempFileCleanupService(ILogger<TempFileCleanupService> logger) : BackgroundService
{
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1);
    private readonly TimeSpan _fileRetentionPeriod = TimeSpan.FromDays(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanUpOldTempFiles(stoppingToken);
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during temp file cleanup");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task CleanUpOldTempFiles(CancellationToken cancellationToken)
    {
        var tempDir = Path.GetTempPath();
        var cutoff = DateTime.Now - _fileRetentionPeriod;

        foreach (var file in Directory.EnumerateFiles(tempDir, "*.chesstitanssave-ms"))
        {
            if (cancellationToken.IsCancellationRequested) return;

            try
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.LastWriteTime < cutoff)
                {
                    await DeleteFileWithRetryAsync(fileInfo, cancellationToken);
                    logger.LogInformation("Deleted old temp file: {FilePath}", file);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not delete temp file: {FilePath}", file);
            }
        }
    }

    private static async Task DeleteFileWithRetryAsync(FileInfo fileInfo, CancellationToken cancellationToken, int maxRetries = 3)
    {
        for (int retry = 0; retry < maxRetries; retry++)
        {
            try
            {
                // Use DeleteAsync in .NET 6+ or fallback to Task.Run for synchronous API
                await Task.Run(() => fileInfo.Delete(), cancellationToken);
                return;
            }
            catch (IOException) when (retry < maxRetries - 1)
            {
                // Exponential backoff
                var delay = TimeSpan.FromMilliseconds(100 * Math.Pow(2, retry));
                await Task.Delay(delay, cancellationToken);
            }
        }
        throw new IOException($"Failed to delete file after {maxRetries} attempts: {fileInfo.FullName}");
    }
}
