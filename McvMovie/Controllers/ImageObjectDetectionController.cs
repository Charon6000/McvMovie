using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Text.Json;

namespace McvMovie.Controllers
{
    public class ImageObjectDetectionController : Controller
    {
        private readonly string ApiUrl = "https://technikum01.cognitiveservices.azure.com";
        public string ApiKey = "1f25eebbade74665b2f38c264e3ad832";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Detect(IFormFile inputFile)
        {
            if (inputFile == null)
            {
                ViewBag.Message = "No sended file";
                return View("Index");
            }

            try
            {
                var apiClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(ApiKey)) { Endpoint = ApiUrl };
                DetectResult detectResults = await apiClient.DetectObjectsInStreamAsync(inputFile.OpenReadStream());
                ViewBag.Objects = detectResults.Objects;
                var memoryImageStream = new MemoryStream();
                inputFile.OpenReadStream().CopyTo(memoryImageStream);
                string imageBase64 = Convert.ToBase64String(memoryImageStream.ToArray());
                ViewBag.IMG = string.Format("data:image/gif;base64,{0}", imageBase64);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.ToString();
            }
            return View("Index");
        }
    }
}
