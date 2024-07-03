using Livelox2png.configuration;
using Livelox2png.services;
using Livelox2png.services.CourseDrawer;
using Livelox2png.util;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .Build();

var defaultPerson = config.GetSection("DefaultPerson").Get<DefaultPerson>();

Console.WriteLine("-- Livelox map with course extractor --");
Console.WriteLine("");

//string classBlobUri = "https://www.livelox.com/Viewer/Stockholm-City-Cup-1/H45?classId=761272&tab=player";
//string classBlobUri = "https://www.livelox.com/Viewer/Stockholm-City-Cup-2/H45?classId=770267&tab=player";
//string classBlobUri = "https://www.livelox.com/Viewer/Stockholm-City-Cup-1/H45?classId=761272&tab=player";
//string classBlobUri = "https://www.livelox.com/Viewer/Natt-KM/H40?classId=685619&tab=player";
//string classBlobUri = "https://www.livelox.com/Viewer/Fasta-kontroller-Adran/Sommarbana?classId=497742";
// https://www.livelox.com/Viewer/SM-traning-1-2/Svart?classId=477888&tab=player <- clipping of control rings
// https://www.livelox.com/Viewer/Motionsorientering-i-Palsjo-skog/Lang?classId=797486&tab=player <- reused controls && cut off control lines
// https://www.livelox.com/Viewer/Trekvallars-3-lang/H45?classId=797945 ← Too big control rings, connection lines inside rings

string? classBlobUrl = "";
if (args.Length > 0)
{
    classBlobUrl = args[0];
}
while (!Uri.TryCreate(classBlobUrl, UriKind.Absolute, out _))
{
    Console.WriteLine("Enter Livelox URL of course:");
    classBlobUrl = Console.ReadLine();
}

var liveloxClient = new LiveloxClient();
try
{
    var activity = await liveloxClient.FetchActivity(classBlobUrl);

    using var image = await liveloxClient.FetchMapFile(activity.Map.Url);

    var map = new Livelox2png.entities.Map(activity);

    var courseDrawer = new CourseDrawer(map, activity, image, config);
    await courseDrawer.Draw();

    await MapSaver.SaveMap(image, activity, config);
}
catch (FatalException e)
{
    Console.WriteLine(e.Message);
}
catch (Exception e)
{
    Console.WriteLine("A fatal error occurred");
    Console.Write(e);
}