using Livelox2png.entities.livelox;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;

namespace Livelox2png.services;

internal static class MapSaver
{
    public static async Task SaveMap(Image image, Activity activity, IConfigurationRoot config)
    {
        var extractor = new ActivityExtractor(activity, config);

        // Some metadata from the activity
        var participant = extractor.GetParticipant();
        var activityDate = participant?.TimeInterval?.Start ?? null;
        var className = activity.Courses?.FirstOrDefault()?.Classes.FirstOrDefault(c => c.Id == participant?.ClassId)?.Name;
        var fileName = activityDate != null && className != null ?
            $"{activityDate.Value.ToString("yyyy-MM-dd")} {className} {activity.Map.Name}" :
            activity.Map.Name;

        var outPutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"{fileName}.png");
        await image.SaveAsPngAsync(outPutPath);
        Console.WriteLine($"Saved map to {outPutPath}");
    }
}
