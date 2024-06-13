using Livelox2png.entities;
using Livelox2png.entities.livelox;
using Livelox2png.projection;
using Livelox2png.util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
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

//Console.WriteLine("Downloading map file");

//var mapData = new MemoryStream();
//using var progress = new ProgressBar();
//await httpClient.DownloadAsync(activity.Map.Url, mapData, progress, CancellationToken.None);

//Console.WriteLine("");
//Console.WriteLine("Downloaded map successfully");


//mapData.Position = 0;
//using Image image = Image.Load(mapData);

using Image image = Image.Load(@"C:\Users\matsb\Downloads\map.png");


var map = new Livelox2png.entities.Map(activity);
var projector = new Projector(map);

var controls = activity.Courses?.First()?.Controls ?? [];
for (var i = 0; i < controls.Count; i++)
{
    var ctrlCoord = new Livelox2png.entities.Coordinate
    {
        Latitude = controls[i].Control.Position.Latitude,
        Longitude = controls[i].Control.Position.Longitude
    };
    var pos = ctrlCoord.Project(map.ProjectionOrigin);
    var posP = LinearAlgebraUtil.Transform(pos, map.ProjectionMatrix);
    //var pos = projector.Project(new Livelox2png.entities.Coordinate
    //{
    //    Latitude = controls[i].Control.Position.Latitude,
    //    Longitude = controls[i].Control.Position.Longitude
    //});
    Console.WriteLine($"Control {controls[i].Control.Code}: {posP}");

    EllipsePolygon circle = new EllipsePolygon((float)posP.X, (float)posP.Y, 10.0f);

    image.Mutate(x => x.Fill(Color.Red, circle));
}

Console.WriteLine($"map dimensions. W: {map.Width}, H: {map.Height}");

foreach (var vertice in activity.Map.Polygon.Vertices)
{
    //var pos = projector.Project(new Livelox2png.entities.Coordinate
    //{
    //    Latitude = vertice.Latitude,
    //    Longitude = vertice.Longitude,
    //});
    var corner = new Livelox2png.entities.Coordinate() {
        Latitude = vertice.Latitude,
        Longitude = vertice.Longitude,
    };
    var pos = corner.Project(map.ProjectionOrigin);
    var posP = LinearAlgebraUtil.Transform(pos, map.ProjectionMatrix);
    Console.WriteLine($"Vertice: {pos}, transformed: {posP}");

    EllipsePolygon circle = new EllipsePolygon((float)posP.X, (float)posP.Y, 25.0f);

    image.Mutate(x => x.Fill(Color.Blue, circle));

}

// Some metadata from the activity
var firstParticipant = activity.Participants?.FirstOrDefault();
var activityDate = firstParticipant?.TimeInterval?.Start ?? null;
var className = activity.Courses?.FirstOrDefault()?.Classes.FirstOrDefault(c => c.Id == firstParticipant?.ClassId)?.Name;
var fileName = activityDate != null && className != null ?
    $"{activityDate.Value.ToString("yyyy-MM-dd")} {className} {activity.Map.Name}" :
    activity.Map.Name;

var outPutPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"{fileName}.png");
await image.SaveAsPngAsync(outPutPath);
Console.WriteLine($"Saved map to {outPutPath}");