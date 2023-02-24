using Microsoft.EntityFrameworkCore;
using VVTDE.Domain;
using VVTDE.Persistence;
using VVTDE.Services.Interfaces;

namespace VVTDE.Services;

public class VideoStorageService : IVideoStorageService
{
    private readonly IVideoDbContext _context;
    private readonly ILogger<VideoStorageService> _logger;

    public VideoStorageService(IVideoDbContext context,
        ILogger<VideoStorageService> logger)
    {
        _context = context;
        _logger = logger;
        
        _logger.LogInformation("{nameofClass} is loaded", nameof(VideoStorageService));
    }

    public Video AddVideo(Video video)
    {
        _context.Videos.Add(video);

        // Fix "EF Core SQLITE - SQLite Error 19: 'UNIQUE constraint failed"
        _context.Videos.Attach(video);

        _context.SaveChangesAsync().ConfigureAwait(false);
        
        _logger.LogInformation($"Video added. Guid: {video.Guid}");
        return video;
    }
    
    public Video GetVideo(Guid guid)
        => _context.Videos.FirstOrDefault(v => v.Guid == guid);

    public List<Video> GetAllVideos()
        => _context.Videos.Select(x => x)?.ToList() ?? new List<Video>(); 
    
    public Guid CheckVideoExists(string url)
    {
        var video = _context.Videos.FirstOrDefault(v => v.Url == url);
        if (video is null)
        {
            return Guid.Empty;
        }

        return video.Guid;
    }
}