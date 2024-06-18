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
//    Console.WriteLine("Missing argument, should be URL of event, like 'https://www.livelox.com/Viewer/Stockholm-City-Cup-1/H45?classId=761272&tab=player'");
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

//string classBlobUri = "https://www.livelox.com/Viewer/Stockholm-City-Cup-1/H45?classId=761272&tab=player";
//string classBlobUri = "https://www.livelox.com/Viewer/Stockholm-City-Cup-2/H45?classId=770267&tab=player";
string classBlobUri = "https://www.livelox.com/Viewer/Stockholm-City-Cup-1/H45?classId=761272&tab=player";

var liveloxClient = new LiveloxClient();
var activity = await liveloxClient.FetchActivity(classBlobUri);

using var image = await liveloxClient.FetchMapFile(activity.Map.Url);

var map = new Livelox2png.entities.Map(activity);

var courseDrawer = new CourseDrawer(map, activity, image, config);
courseDrawer.Draw();

await MapSaver.SaveMap(image, activity, config);