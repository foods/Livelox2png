using Livelox2png.entities.livelox;
using Livelox2png.util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System;
using System.Text.Json;
using System.Threading;

Console.WriteLine("-- Mats Livelox map with course extractor");

//if (args.Length != 2)
//{
//    Console.WriteLine("Missing argument, should be URL like 'https://livelox.blob.core.windows.net/class-storage/0000747178_3926681217747'");
//    return;
//}
//Uri classBlobUri;
//try
//{
//    classBlobUri = new Uri(args[1]);
//}
//catch (UriFormatException)
//{
//    Console.WriteLine("Incorrect URL");
//    return;
//}

Uri classBlobUri = new Uri("https://livelox.blob.core.windows.net/class-storage/0000747178_3926681217747");

var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(5)
};
var response = await httpClient.GetAsync(classBlobUri);
response.EnsureSuccessStatusCode();

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};

//var contents = await response.Content.ReadAsStringAsync();

var activity = await JsonSerializer.DeserializeAsync<Activity>(response.Content.ReadAsStream(), options);

//var contensts = File.ReadAllText("C:\\Users\\matsb\\Downloads\\activity.json");
//var activity = JsonSerializer.Deserialize<Activity>(contents, options);


if (activity == null)
{
    Console.WriteLine("Unable to parse activity");
    return;
}

Console.WriteLine("Downloading map file");

var mapData = new MemoryStream();
using var progress = new ProgressBar();
await httpClient.DownloadAsync(activity.Map.Url, mapData, progress, CancellationToken.None);

Console.WriteLine("");
Console.WriteLine("Downloaded map successfully");

mapData.Position = 0;
using Image image = Image.Load(mapData);

// Some metadata from the activity
var firstParticipant = activity.Participants?.FirstOrDefault();
var activityDate = firstParticipant?.TimeInterval?.Start ?? null;
var className = activity.Courses?.FirstOrDefault()?.Classes.FirstOrDefault(c => c.Id == firstParticipant?.ClassId)?.Name;
var fileName = activityDate != null && className != null ?
    $"{activityDate.Value.ToString("yyyy-MM-dd")} {className} {activity.Map.Name}" :
    activity.Map.Name;

var outPutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"{fileName}.png");
await image.SaveAsPngAsync(outPutPath);
Console.WriteLine($"Saved map to {outPutPath}");