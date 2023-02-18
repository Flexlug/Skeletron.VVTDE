using Cysharp.Diagnostics;
using VVTDE.Domain;
using VVTDE.Persistence;

namespace VVTDE.Services;

public class VideoDownloadService
{
    private readonly string _downloadPath;
    private readonly string _ytDlpPath;
    private readonly VideoStorageService _storage;
    private readonly ILogger<VideoDownloadService> _logger;
    
    private readonly List<Guid> _downloadQueue = new();

    public VideoDownloadService(VideoStorageService storage, 
        IConfiguration configuration, 
        ILogger<VideoDownloadService> logger)
    {
        _storage = storage;
        _logger = logger;
        _downloadPath = configuration["VideoDownloadPath"] ?? throw new NullReferenceException("Video download path is not specified in appsettings.json");
        _ytDlpPath = configuration["YtDlpPath"] ?? throw new NullReferenceException("Yt-dlp path is not specified in appsettings.json");

        if (!File.Exists(_ytDlpPath))
        {
            throw new FileNotFoundException($"Couldn't find yt-dlp executable in specified path: {_ytDlpPath}");
        }
        
        _logger.LogInformation("{nameofClass} is loaded", nameof(VideoDownloadService));
    }

    public async Task<bool> IsDownloading(Guid guid) =>
        _downloadQueue.Contains(guid);

    public async Task DownloadVideo(Video video)
    {
        _downloadQueue.Add(video.Guid);
        var filename = await ProcessX.StartAsync($"yt-dlp --quiet --no-simulate --no-warnings --print filename {video.Url} -P {_downloadPath}")
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(filename))
        {
            _logger.LogWarning("Couldn't download video. Output video filename is null or empty");
            _downloadQueue.Remove(video.Guid);
            return;
        }
        
        _logger.LogInformation("Download complete. Filename: {Filename}", filename);

        _downloadQueue.Remove(video.Guid);
        await _storage.AddVideo(video);
    }
}