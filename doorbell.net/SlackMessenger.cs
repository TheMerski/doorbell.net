using Microsoft.Extensions.Logging;
using Slack.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doorbelldotnet;

public class SlackMessenger
{
    private readonly ILogger<SlackMessenger> _logger;
    private readonly AppSettings _appSettings;
    private SlackClient? slackClient;
    private readonly bool enabled;

    public SlackMessenger(ILogger<SlackMessenger> logger, AppSettings appSettings)
    {
        _logger = logger;
        _appSettings = appSettings;
        enabled = !string.IsNullOrWhiteSpace(_appSettings.SlackWebhook) || (!string.IsNullOrWhiteSpace(_appSettings.SlackChannel) && _appSettings.SlackChannel.Contains("#"));
        if (enabled)
        {
            slackClient = new SlackClient(_appSettings.SlackWebhook);
        } else
        {
            _logger.LogInformation("Slack not configured");
        }
    }

    public async Task<Task> SendSlackMessage()
    {
        if (!enabled || slackClient == null)
            return Task.CompletedTask;

        var date = DateTime.Now;

        var slackMessage = new SlackMessage
        {
            Channel = _appSettings.SlackChannel,
            Text = $"<!here> The doorbell rang on `{date.ToString("HH:mm:ss")}`",
            Username = "doorbell"
        };
        await slackClient.PostAsync(slackMessage);
        return Task.CompletedTask;
    }
}
