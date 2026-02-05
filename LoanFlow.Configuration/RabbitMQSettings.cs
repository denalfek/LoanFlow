namespace LoanFlow.Configuration;

public sealed class RabbitMQSettings
{
    public const string SectionName = "RabbitMQ";

    /// <summary>
    /// RabbitMQ connection URI (e.g., amqp://user:pass@host:port/vhost).
    /// If provided, this takes precedence over individual properties.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// RabbitMQ host. Used only if ConnectionString is not provided.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// RabbitMQ port. Used only if ConnectionString is not provided.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// RabbitMQ username. Used only if ConnectionString is not provided.
    /// </summary>
    public string Username { get; set; } = "guest";

    /// <summary>
    /// RabbitMQ password. Used only if ConnectionString is not provided.
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// RabbitMQ virtual host. Used only if ConnectionString is not provided.
    /// </summary>
    public string VirtualHost { get; set; } = "/";
}
