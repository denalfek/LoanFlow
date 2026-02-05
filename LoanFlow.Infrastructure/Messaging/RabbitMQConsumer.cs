using System.Text;
using System.Text.Json;
using LoanFlow.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LoanFlow.Infrastructure.Messaging;

public abstract class RabbitMQConsumer<TMessage> : BackgroundService where TMessage : class
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    private const string ExchangeName = "loanflow.events";

    protected abstract string QueueName { get; }
    protected abstract string RoutingKey { get; }

    protected RabbitMQConsumer(IOptions<RabbitMQSettings> settings, ILogger logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    protected abstract Task HandleMessageAsync(TMessage message, CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeAsync(stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel!);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<TMessage>(json);

                if (message is not null)
                {
                    await HandleMessageAsync(message, stoppingToken);
                    await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, stoppingToken);
                    _logger.LogInformation("Processed message {MessageId}", ea.BasicProperties.MessageId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, stoppingToken);
            }
        };

        await _channel!.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("Started consuming from queue {QueueName}", QueueName);

        // Keep the service running
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Bound queue {QueueName} to exchange {Exchange} with routing key {RoutingKey}",
            QueueName, ExchangeName, RoutingKey);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
            await _channel.CloseAsync(cancellationToken);

        if (_connection is not null)
            await _connection.CloseAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}
