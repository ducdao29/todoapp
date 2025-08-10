/*DiceController.cs*/

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using todoapp.Server;

public class DiceController : ControllerBase
{
    private ILogger<DiceController> logger;

    private ActivitySource activitySource;

    public DiceController(ILogger<DiceController> logger, Instrumentation instrumentation)
    {
        this.logger = logger;
        this.activitySource = instrumentation.ActivitySource;
    }

    [HttpGet("/rolldice")]
    public List<int> RollDice(string player, int? rolls)
    {
        List<int> result = new List<int>();

        if (!rolls.HasValue)
        {
            logger.LogError("Missing rolls parameter");
            throw new HttpRequestException("Missing rolls parameter", null, HttpStatusCode.BadRequest);
        }

        result = new Dice(1, 6, activitySource).rollTheDice(rolls.Value);

        if (string.IsNullOrEmpty(player))
        {
            logger.LogInformation($"Anonymous player is rolling the dice: {string.Join(",", result)}");
        }
        else
        {
            logger.LogInformation($"{player} is rolling the dice: {string.Join(",", result)}");
        }

        return result;
    }
}