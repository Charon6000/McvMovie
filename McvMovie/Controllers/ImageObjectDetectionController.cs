using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Text.Json;

namespace McvMovie.Controllers
{
    public class ImageObjectDetectionController : Controller
    {
        private readonly string ApiUrl = "https://technikum01.cognitiveservices.azure.com";
        public string ApiKey = "d0b1d44871564786934befe406045484";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Detect(IFormFile inputFile)
        {
            var apiClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(ApiKey)) { Endpoint = ApiUrl };
            DetectResult detectResults = await apiClient.DetectObjectsInStreamAsync(inputFile.OpenReadStream());
            ViewBag.Message = JsonSerializer.Serialize(detectResults);

            return View("Wynik");
        }
    }
}
