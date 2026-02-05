using LoanFlow.Configuration;
using RabbitMQ.Client;

namespace LoanFlow.Infrastructure.Messaging;

internal static class RabbitMQConnectionFactory
{
    /// <summary>
    /// Creates a ConnectionFactory from RabbitMQSettings.
    /// If ConnectionString (URI) is provided, it takes precedence.
    /// Otherwise, individual properties are used for backward compatibility.
    /// </summary>
    public static ConnectionFactory Create(RabbitMQSettings settings)
    {
        // If ConnectionString (URI) is provided, use it
        if (!string.IsNullOrWhiteSpace(settings.ConnectionString))
        {
            try
            {
                return new ConnectionFactory
                {
                    Uri = new Uri(settings.ConnectionString)
                };
            }
            catch (UriFormatException ex)
            {
                throw new InvalidOperationException(
                    $"Invalid RabbitMQ connection string format. Please ensure it follows the format: amqp://username:password@hostname:port/virtualhost. Error: {ex.Message}",
                    ex);
            }
        }

        // Otherwise, use individual properties for backward compatibility
        return new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password,
            VirtualHost = settings.VirtualHost
        };
    }
}
