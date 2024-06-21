using Livelox2png.entities.livelox;
using Livelox2png.entities.livelox.classinfo;
using Livelox2png.util;
using SixLabors.ImageSharp;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Livelox2png.services;

internal class LiveloxClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    public LiveloxClient()
    {
        httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5)
        };
        jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

    }

    private async Task<ClassInfo> FetchClassInfo(int classId)
    {
        var classInfoUri = new Uri("https://www.livelox.com/Data/ClassInfo");
        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, classInfoUri);
        var requestObj = new ClassInfoRequest() { ClassIds = [classId] };
        req.Headers.Add("X-Requested-With", "XMLHttpRequest");
        req.Content = new StringContent(JsonSerializer.Serialize(requestObj), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(req);
        response.EnsureSuccessStatusCode();

        var classInfo = await JsonSerializer.DeserializeAsync<ClassInfo>(response.Content.ReadAsStream(), jsonSerializerOptions);

        if (classInfo == null)
        {
            throw new Exception("Unable to parse class info. Check url");
        }
        return classInfo;
    }

    private async Task<Activity> FetchClassBlob(Uri classBlobUri)
    {
        var response = await httpClient.GetAsync(classBlobUri);
        response.EnsureSuccessStatusCode();

        var activity = await JsonSerializer.DeserializeAsync<Activity>(response.Content.ReadAsStream(), jsonSerializerOptions);

        return activity ?? throw new Exception("Unable to parse activity");
    }

    private async Task<Activity> FetchClassBlob(int classId)
    {
        var classBlobUri = new Uri("https://www.livelox.com/Data/ClassBlob");
        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, classBlobUri);
        var requestObj = new ClassBlobRequest(classId);
        req.Headers.Add("X-Requested-With", "XMLHttpRequest");
        req.Content = new StringContent(JsonSerializer.Serialize(requestObj), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(req);
        response.EnsureSuccessStatusCode();

        var classInfo = await JsonSerializer.DeserializeAsync<Activity>(response.Content.ReadAsStream(), jsonSerializerOptions);

        return classInfo ?? throw new Exception("Unable to parse activity");
    }


    public async Task<Activity> FetchActivity(string url)
    {
        // Extract classid from url
        var uri = new Uri(url);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var classIdStr = query.Get("classId") ?? throw new Exception($"Unable to properly parse input url. ClassId is missing");
        var classId = Int32.Parse(classIdStr);

        var classInfo = await FetchClassInfo(classId);

        if (classInfo.General.ClassBlobUrl != null)
        {
            return await FetchClassBlob(new Uri(classInfo.General.ClassBlobUrl));
        }

        // There is no ClassBlobUrl, fetch it from the specific endpoint instead.
        return await FetchClassBlob(classId);
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
    }

    public async Task<Stream> FetchMapImage(string url)
    {
        Console.WriteLine("Downloading course image");
        var mapData = new MemoryStream();
        using var progress = new ProgressBar();
        await httpClient.DownloadAsync(url, mapData, progress, CancellationToken.None);
        mapData.Position = 0;
        return mapData;
    }
}
