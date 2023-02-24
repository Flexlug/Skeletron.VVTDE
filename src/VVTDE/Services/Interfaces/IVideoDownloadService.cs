using VVTDE.Domain;

namespace VVTDE.Services.Interfaces;

public interface IVideoDownloadService
{
    Task<bool> IsDownloading(Guid guid);
    void Download(Video video);
    bool TryGetVideoPath(Guid guid, out string path);
}