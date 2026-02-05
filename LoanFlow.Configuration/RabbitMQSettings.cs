namespace LoanFlow.Configuration;

public sealed class RabbitMQSettings
{
    public const string SectionName = "RabbitMQ";

    /// <summary>
    /// AMQP connection URL (e.g., amqps://user:pass@host/vhost).
    /// When provided, this takes precedence over individual properties.
    /// </summary>
    public string? ConnectionString { get; set; }

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";

    public bool HasConnectionString => !string.IsNullOrWhiteSpace(ConnectionString);
}
