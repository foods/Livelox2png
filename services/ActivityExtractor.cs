using Livelox2png.configuration;
using Livelox2png.entities.livelox;
using Microsoft.Extensions.Configuration;

namespace Livelox2png.services;

internal class ActivityExtractor
{
    private readonly Activity activity;
    private readonly IConfigurationRoot config;
    public ActivityExtractor(Activity activity, IConfigurationRoot config)
    {
        this.activity = activity;
        this.config = config;
    }

    /// <summary>
    /// Fetches the participant that matches the name and org from appsettings.
    /// 
    /// Note! If no match, it will return an arbitrary participant
    /// </summary>
    /// <returns></returns>
    public Participant? GetParticipant()
    {
        if (activity.Participants == null)
        {
            return null;
        }
        var defaultPerson = config.GetSection("DefaultPerson").Get<DefaultPerson>();
        var match = defaultPerson != null ? activity.Participants.FirstOrDefault(p =>
            p.Result != null &&
            p.Result.PersonFirstName == defaultPerson.PersonFirstName &&
            p.Result.PersonLastName == defaultPerson.PersonLastName &&
            p.Result.OrganisationName == defaultPerson.OrganisationName) : null;

        return match ?? activity.Participants.FirstOrDefault();
    }

    public Course GetCourse()
    {
        if (activity.Courses == null || activity.Courses.Count == 0)
        {
            throw new Exception("No courses in this activity. Unable to extract");
        }
        var participant = GetParticipant();
        if (participant == null)
        {
            return activity.Courses.First();
        }
        return activity.Courses.First(c => c.Classes.Any(cl => cl.Id == participant.ClassId));
    }
}
