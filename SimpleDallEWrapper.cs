using Azure;
using Azure.AI.OpenAI;

namespace BulkDallE;
public class SimpleDallEWrapper(string _endpoint, string _key, string _deployment)
{
    private readonly OpenAIClient _client = new(new Uri(_endpoint), new AzureKeyCredential(_key));
    private readonly HttpClient _httpClient = new();
    private readonly int _maxRetries = 5;
    public async Task<byte[]> GenerateImageAsync(string prompt)
    {
        List<Exception> errors = [];
        for (int i = 0; i < _maxRetries; i++)
        {
            try
            {
                var image = await _client.GetImageGenerationsAsync(new ImageGenerationOptions(
                    prompt
                )
                {
                    DeploymentName = _deployment,
                    ImageCount = 1,
                    Quality = ImageGenerationQuality.Standard,
                    Size = ImageSize.Size1024x1024,
                    Style = ImageGenerationStyle.Natural
                });
                return await _httpClient.GetByteArrayAsync(image.Value.Data.Single().Url);
            }
            catch (Exception e)
            {
                errors.Add(e);
                Console.WriteLine($"Unable to generate image: {e.Message}");
                await Task.Delay(i * 2500);
            }
        }
        throw new AggregateException(errors);
    }
}
