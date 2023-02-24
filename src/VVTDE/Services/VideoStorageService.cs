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

    public async Task<Video> AddVideo(Video video)
    {
        await _context.Videos.AddAsync(video);

        // Fix "EF Core SQLITE - SQLite Error 19: 'UNIQUE constraint failed"
        _context.Videos.Attach(video);
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Video added. Guid: {video.Guid}");
        return video;
    }
    
    public async Task<Video> GetVideo(Guid guid)
        => await _context.Videos.FirstOrDefaultAsync(v => v.Guid == guid);

    public async Task<Guid> CheckVideoExists(string url)
    {
        var video = await _context.Videos.FirstOrDefaultAsync(v => v.Url == url);
        if (video is null)
        {
            return Guid.Empty;
        }

        return video.Guid;
    }
}