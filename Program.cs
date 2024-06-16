using Livelox2png.configuration;
using Livelox2png.services;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .Build();

var defaultPerson = config.GetSection("DefaultPerson").Get<DefaultPerson>();

Console.WriteLine("-- Livelox map with course extractor --");

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
//Uri classBlobUri = new Uri("https://livelox.blob.core.windows.net/class-storage/0000770267_3926426158285");

var liveloxClient = new LiveloxClient();
var activity = await liveloxClient.FetchActivity(classBlobUri);

using var image = await liveloxClient.FetchMapFile(activity.Map.Url);

var map = new Livelox2png.entities.Map(activity);

var courseDrawer = new CourseDrawer(map, activity, image, config);
courseDrawer.Draw();

await MapSaver.SaveMap(image, activity, config);