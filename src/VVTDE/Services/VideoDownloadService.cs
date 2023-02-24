using System.Collections.Concurrent;
using Cysharp.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VVTDE.Domain;
using VVTDE.Persistence;
using VVTDE.Services.Interfaces;

namespace VVTDE.Services;

public class VideoDownloadService : IVideoDownloadService
{
    private readonly string _downloadPath;
    private readonly string _ytDlpPath;
    private readonly IVideoStorageService _storage;
    private readonly ILogger<VideoDownloadService> _logger;
    
    // В .NET нет ConcurrentList
    private readonly ConcurrentDictionary<Guid, Guid> _downloadQueue = new();

    public event EventHandler<Video> OnVideoDownload;

    public VideoDownloadService(IVideoStorageService storage, 
        IConfiguration configuration, 
        ILogger<VideoDownloadService> logger)
    {
        _storage = storage;
        _logger = logger;
        _downloadPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            configuration["VideoDownloadPath"] ??
                throw new NullReferenceException("Video download path is not specified in appsettings.json"));
        _ytDlpPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            configuration["YtDlpPath"] ??
            throw new NullReferenceException("Yt-dlp path is not specified in appsettings.json"));

        if (!File.Exists(_ytDlpPath))
        {
            throw new FileNotFoundException($"Couldn't find yt-dlp executable in specified path: {_ytDlpPath}");
        }
        
        _logger.LogInformation("{nameofClass} is loaded", nameof(VideoDownloadService));
    }

    public async Task<bool> IsDownloading(Guid guid) =>
        _downloadQueue.TryGetValue(guid, out _);

    public void Download(Video video)
    {
        Task.Factory.StartNew(async () => DownloadWorker(video));
    }

    private async Task DownloadWorker(Video video)
    {
        _logger.LogWarning($"Starting video download. Guid: {video.Guid}");
        _downloadQueue.TryAdd(video.Guid, video.Guid);
        var output = await ProcessX.StartAsync($"{_ytDlpPath} --quiet --no-simulate --no-warnings --print filename {video.Url} -P {_downloadPath} -o \"{video.Guid}.mp4\"")
            .ToTask();

        var filename = output.FirstOrDefault();

        if (string.IsNullOrEmpty(filename))
        {
            _logger.LogWarning("Couldn't download video. Output video filename is null or empty");
            _downloadQueue.Remove(video.Guid, out _);
            return;
        }
        
        _logger.LogInformation("Download complete. Filename: {Filename}", filename);

        _downloadQueue.Remove(video.Guid, out _);
        await _storage.AddVideo(video);
    }
}