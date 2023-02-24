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
            
            var video = _storage.GetVideo(guid);

            if (video is null)
            {
                _logger.LogWarning($"Requested video couldn't be found. Guid: {guid}");
                return NotFound();
            }
            
            _logger.LogInformation($"Requested video found. Guid: {video.Guid}");
            return View(video);
        }

        [HttpGet]
        public async Task<IActionResult> AllVideos()
        {
            if (!validateRequest(Request))
            {
                // Can not return Forbid() because this method requires authentication configured
                return StatusCode(403);
            }

            var videos = _storage.GetAllVideos();

            return Json(videos);
        }
        
        [HttpPost]
        public async Task<IActionResult> RequestDownload([FromBody] VideoRequest request)
        {
            if (!validateRequest(Request))
            {
                // Can not return Forbid() because this method requires authentication configured
                return StatusCode(403);
            }

            var videoGuid = _storage.CheckVideoExists(request.Url);

            if (videoGuid != Guid.Empty)
            {
                _logger.LogInformation($"Requested video download, but it already exists. Guid: {videoGuid}");
                return Ok(new VideoResponse()
                {
                    Guid = videoGuid,
                    AlreadyDownloaded = false
                });
            }

            var newVideo = new Video()
            {
                Guid = Guid.NewGuid(),
                Url = request.Url,
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl
            };

            var video = _storage.AddVideo(newVideo);
            
            _logger.LogInformation($"Requested video download. Guid: {video.Guid}");
            _downloader.Download(video);

            return Created(@$"https://vvtde.flexlug.ru/Video/Watch?guid={video.Guid}", new VideoResponse()
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
                // Can not return Forbid() because this method requires authentication configured
                return StatusCode(403);
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