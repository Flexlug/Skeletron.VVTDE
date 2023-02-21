using System.Web;
using Microsoft.AspNetCore.Mvc;
using VVTDE.Domain;
using VVTDE.Services;
using VVTDE.Services.Interfaces;

namespace VVTDE.Controllers
{
    [Route("[controller]/[action]")]
    public class VideoController : Controller
    {
        private readonly IVideoStorageService _storage;
        private readonly ILogger<VideoController> _logger;

        public VideoController(IVideoStorageService storage,
            ILogger<VideoController> logger)
        {
            _storage = storage;
            _logger = logger;
            
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
                _logger.LogWarning($"Requested video couldn't be found. Guid: {video.Guid}");
                return NotFound();
            }
            
            _logger.LogInformation($"Requested video found. Guid: {video.Guid}");
            return View(video);
        }
    }
}