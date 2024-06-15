using Livelox2png.entities.livelox;
using Livelox2png.util;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Livelox2png.services;

internal class LiveloxClient
{
    private readonly HttpClient httpClient;
    public LiveloxClient()
    {
        httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5)
        };

    }



    public async Task<Activity> FetchActivity(Uri classBlobUri)
    {
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
            throw new Exception("Unable to parse activity");
        }

        return activity;
    }

    public async Task<Image> FetchMapFile(string mapUrl)
    {
        Console.WriteLine("Downloading map file");

        var mapData = new MemoryStream();
        using var progress = new ProgressBar();
        await httpClient.DownloadAsync(mapUrl, mapData, progress, CancellationToken.None);

        Console.WriteLine("");
        Console.WriteLine("Downloaded map successfully");


        mapData.Position = 0;
        return Image.Load(mapData);

        //return Image.Load(@"C:\Users\matsb\Downloads\map.png");

    }
}
