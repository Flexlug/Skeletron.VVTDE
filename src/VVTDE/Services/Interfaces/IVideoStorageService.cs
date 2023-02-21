using VVTDE.Domain;

namespace VVTDE.Services.Interfaces;

public interface IVideoStorageService
{
    Task<Video> AddVideo(Video video);
    Task<Video> GetVideo(Guid guid);
}