using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Text.Json;

namespace McvMovie.Controllers
{
    public class ImageObjectDetectionController : Controller
    {
        readonly string ApiUrl = "https://technikum01.cognitiveservices.azure.com";
        readonly string ApiKey;
        readonly string OpenAiApiUrl= "https://technikum-ai01.openai.azure.com";
        readonly string OpenAiApiKey;
        readonly string OpenAiApiDeploymentName = "tes03";
        private string GetDescription(IEnumerable<string> concepts)
        {
            var writeAboutIt = String.Join(',', concepts);
            OpenAIClient client = new OpenAIClient(new Uri(OpenAiApiUrl), new AzureKeyCredential(OpenAiApiKey));

            string opowiadanie= "You are a Polish AI assistance. Write mi a story about";
            string wydarzenie = "Write in Polish a real story that happend with";

            var msgs = new List<ChatMessage> {
                new(ChatRole.System, wydarzenie),
                new(ChatRole.User, writeAboutIt)
            };

            ChatCompletionsOptions chatOptions = new();
            chatOptions.Messages.Add(msgs[0]);
            chatOptions.Messages.Add(msgs[1]);
            Response<ChatCompletions> chatResponse = client.GetChatCompletions(OpenAiApiDeploymentName, chatOptions);

            string completion = chatResponse.Value.Choices[0].Message.Content.ReplaceLineEndings("<br />");
            return completion;
        }


        public ImageObjectDetectionController(IConfiguration config)
        {
            ApiKey = config["Images:ApiKey"] ?? string.Empty;
            OpenAiApiKey = config["Images:OpenAiApiKey"] ?? string.Empty;
        }


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
                ViewBag.Description = GetDescription(detectResults.Objects.Select(a => a.ObjectProperty).Distinct());
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.ToString();
            }
            return View("Index");
        }
    }
}
