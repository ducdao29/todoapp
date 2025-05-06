using System.Diagnostics;

namespace todoapp.Server;
public class Dice
{
    public ActivitySource activitySource;
    private int min;
    private int max;

    public Dice(int min, int max, ActivitySource activitySource)
    {
        this.min = min;
        this.max = max;
        this.activitySource = activitySource;
    }

    public List<int> rollTheDice(int rolls)
    {
        List<int> results = new List<int>();

        using (var myActivity = activitySource.StartActivity("rollTheDice"))
        {
            for (int i = 0; i < rolls; i++)
            {
                results.Add(rollOnce());
            }

            return results;
        }
    }

    private int rollOnce()
    {
        return Random.Shared.Next(min, max + 1);
    }
}