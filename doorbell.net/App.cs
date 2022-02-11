using doorbell.net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Device.Gpio;
using System.Threading.Tasks;

public class App
{
    private readonly ILogger<App> logger;
    private readonly AppSettings appSettings;
    private GpioController controller;
    private SlackMessenger slackMessenger;

    public App(IOptions<AppSettings> appSettings, ILogger<App> logger, SlackMessenger slackMessenger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
        this.controller = new GpioController();
        this.slackMessenger = slackMessenger ?? throw new ArgumentNullException(nameof(slackMessenger));
    }


    public async Task<Task> Run(string[] args, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting...");
        if (!controller.IsPinModeSupported(appSettings.DetectPin, PinMode.InputPullDown))
        {
            logger.LogError($"Pin {appSettings.DetectPin} does not support input pulldown, please configure a pin that does support this");
            return Task.CompletedTask;
        }
        controller.OpenPin(appSettings.DetectPin, PinMode.InputPullDown);

        while (!cancellationToken.IsCancellationRequested)
        {
            await controller.WaitForEventAsync(appSettings.DetectPin, PinEventTypes.Rising, cancellationToken);
            logger.LogInformation("Got button press");
            await this.slackMessenger.SendSlackMessage();
            await Task.Delay(appSettings.DebounceTime);
        }

        logger.LogInformation("Shutting down");
        return Task.CompletedTask;
    }
}