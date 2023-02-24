using VVTDE.Domain;

namespace VVTDE.Services.Interfaces;

public interface IVideoStorageService
{
    Video AddVideo(Video video);
    Video GetVideo(Guid guid);
    Guid CheckVideoExists(string url);
}