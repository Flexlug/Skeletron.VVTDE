using System.Diagnostics.CodeAnalysis;
using Grpc.Core;
using Skeletron;
using VVTDE.Domain;

namespace VVTDE.Services;

public class GrpcServerService : VVTDEBridge.VVTDEBridgeBase
{
    private readonly ILogger<GrpcServerService> _logger;
    private readonly VideoStorageService _storage;
    private readonly VideoDownloadService _downloader;

    public GrpcServerService(ILogger<GrpcServerService> logger,
        VideoStorageService storage,
        VideoDownloadService downloader)
    {
        _logger = logger;
        _storage = storage;
        _downloader = downloader;
        
        _logger.LogInformation("{nameofClass} is loaded", nameof(GrpcServerService));
    }

    public override async Task<VideoReply> RequestDownloadVideo(VideoRequest request, ServerCallContext context)
    {
        var newVideo = new Video()
        {
            Guid = Guid.NewGuid(),
            Url = request.Url,
            Title = request.Title,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        };

        var video = await _storage.AddVideo(newVideo);
        
        // Если видео было найдено в базе, то будет возвращен экземпляр видео из БД с его собственным GUID
        // Если видео не было найдено в базе, то будет возвращен тот же экземпляр с тем же GUID
        if (video.Guid != newVideo.Guid)
        {
            _logger.LogInformation($"Requested video download. Guid: {video.Guid}");
            await _downloader.DownloadVideo(video);
            return new()
            {
                Guid = video.Guid.ToString(),
                AlreadyDownloaded = false
            };            
        }
        
        _logger.LogInformation($"Requested video download, but it already exists. Guid: {video.Guid}");
        return new()
        {
            Guid = video.Guid.ToString(),
            AlreadyDownloaded = true
        };
    }

    public override async Task<FetchVideoReply> FetchDownloadVideo(FetchVideoRequest request, ServerCallContext context)
    {
        if (await _downloader.IsDownloading(Guid.Parse(request.Guid)))
        {
            _logger.LogInformation($"Fetched video is still in download a queue. {request.Guid}");
            return new()
            {
                DownloadComplete = true
            };            
        }
        
        _logger.LogInformation($"Fetched video is not in download a queue. {request.Guid}");
        return new()
        {
            DownloadComplete = false
        };
    }
}