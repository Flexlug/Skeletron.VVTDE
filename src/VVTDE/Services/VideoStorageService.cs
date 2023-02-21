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
        Video existsVideo = await _context.Videos.FirstOrDefaultAsync(v => v.Url == video.Url);
        if (existsVideo is not null)
        {
            _logger.LogInformation($"Requested video already exists. Guid: {video.Guid}");
            return existsVideo;
        }

        await _context.Videos.AddAsync(video);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Video added. Guid: {video.Guid}");
        return video;
    }
    
    public async Task<Video> GetVideo(Guid guid)
        => await _context.Videos.FirstOrDefaultAsync(v => v.Guid == guid);
}