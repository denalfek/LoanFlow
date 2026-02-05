using LoanFlow.Configuration;
using RabbitMQ.Client;

namespace LoanFlow.Infrastructure.Messaging;

public static class RabbitMQConnectionFactory
{
    public static ConnectionFactory Create(RabbitMQSettings settings)
    {
        if (settings.HasConnectionString)
        {
            return new ConnectionFactory
            {
                Uri = new Uri(settings.ConnectionString!)
            };
        }

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
