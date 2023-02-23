using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using VVTDE.Domain;
using VVTDE.Models;
using VVTDE.Services;
using VVTDE.Services.Interfaces;

namespace VVTDE.Controllers
{
    [Route("[controller]/[action]")]
    public class VideoController : Controller
    {
        private readonly IVideoStorageService _storage;
        private readonly ILogger<VideoController> _logger;
        private readonly IVideoDownloadService _downloader;
        private readonly string _token;

        public VideoController(IVideoStorageService storage,
            ILogger<VideoController> logger,
            IVideoDownloadService downloader,
            IConfiguration configuration)
        {
            _storage = storage;
            _logger = logger;
            _downloader = downloader;

            _token = configuration["Token"] ?? throw new ArgumentNullException("No token specified");
            
            _logger.LogInformation("{nameofClass} is loaded", nameof(VideoController));
        }

        [HttpGet]
        public async Task<IActionResult> Watch(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                return BadRequest("Please, specify guid");
            }
            
            var video = await _storage.GetVideo(guid);

            if (video is null)
            {
                _logger.LogWarning($"Requested video couldn't be found. Guid: {guid}");
                return NotFound();
            }
            
            _logger.LogInformation($"Requested video found. Guid: {video.Guid}");
            return View(video);
        }

        [HttpPost]
        public async Task<IActionResult> RequestDownload([FromBody] VideoRequest request)
        {
            if (!validateRequest(Request))
            {
                return Forbid();
            }
            
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
            if (video.Guid == newVideo.Guid)
            {
                _logger.LogInformation($"Requested video download. Guid: {video.Guid}");
                _downloader.Download(video);
                
                return Created(@$"https://vvtde.flexlug.ru/Video/Watch?guid={video.Guid}", new VideoResponse()
                {
                    Guid = video.Guid,
                    AlreadyDownloaded = false
                });            
            }
        
            _logger.LogInformation($"Requested video download, but it already exists. Guid: {video.Guid}");
            return Ok(new VideoResponse()
            {
                Guid = video.Guid,
                AlreadyDownloaded = false
            });
        }

        [HttpPost]
        public async Task<IActionResult> FetchVideo([FromBody] FetchVideoRequest request)
        {
            if (!validateRequest(Request))
            {
                return Forbid();
            }
            
            if (await _downloader.IsDownloading(request.Guid))
            {
                _logger.LogInformation($"Fetched video is still in download a queue. {request.Guid}");
                return Ok(new FetchVideoResponse()
                {
                    DownloadComplete = false
                });
            }
        
            _logger.LogInformation($"Fetched video is not in download a queue. {request.Guid}");
            return Ok(new FetchVideoResponse()
            {
                DownloadComplete = true
            });
        }

        [NonAction]
        private bool validateRequest(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("Token"))
            {
                return false;
            }

            var value = request.Headers["Token"];
            if (value.Count == 0)
            {
                return false;
            }

            if (value[0] != _token)
            {
                return false;
            }

            return true;
        }
    }
}