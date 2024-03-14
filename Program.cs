using BulkDallE;
using System.Text.Json;

string azureOpenAiEndpoint = "";
string azureOpenAiKey = "";
string dalleDeploymentName = "dall-e-3"; // Tested with Dall-e-3 in Sweden Central

string jsonFile = "example.json";

// Set up our Dall-E wrapper
var dalle = new SimpleDallEWrapper(azureOpenAiEndpoint, azureOpenAiKey, dalleDeploymentName);

var items = LoadArrayFromJson<(string Id, string Description)>(jsonFile);
(int Width, int Height)[] sizes = [
    (200, 200),
    (512, 512),
];

// Set up directories
string rawPath = "_generated/raw";
string resizePathPrefix = "_generated/resize";
Directory.CreateDirectory(rawPath);
foreach (var (Width, Height) in sizes) Directory.CreateDirectory($"{resizePathPrefix}{Width}x{Height}");

foreach (var (Id, Description) in items)
{
    string prompt = $"A \"{Description}\" on a blank white background, retail photography, no shadows, no text, lightbox, middle of image, plain white background";

    var image = await dalle.GenerateImageAsync(prompt);
    WriteWithLogging(Path.Combine(rawPath, Id + ".png"), image);


    foreach (var (Width, Height) in sizes)
    {
        var resizedImage = SimpleResizer.ResizeToJpg(image, Width, Height);
        WriteWithLogging(Path.Combine($"{resizePathPrefix}{Width}x{Height}", Id + ".jpg"), resizedImage);
    }
}
Console.WriteLine("Done!");

static void WriteWithLogging(string path, byte[] data)
{
    Console.WriteLine("Saving " + path);
    File.WriteAllBytes(path, data);
}
static T[] LoadArrayFromJson<T>(string fileName)
    => JsonSerializer.Deserialize<T[]>(File.ReadAllText(fileName)) ?? throw new("Null json");