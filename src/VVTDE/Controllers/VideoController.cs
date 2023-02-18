using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace VVTDE.Controllers
{
    [Route("[controller]")]
    public class VideoController : Controller
    {
        [HttpGet("{guid}")]
        public IActionResult Video(string guid)
        {
            ViewData["guid"] = HttpUtility.UrlDecode(guid);
            return View();
        }
    }
}