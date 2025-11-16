namespace SmartGreenhouse.Api.Contracts
{
    public record AdapterSettingsRequest(
         string ActuatorMode,
         string NotificationMode,
         string? WebhookUrl);
}
