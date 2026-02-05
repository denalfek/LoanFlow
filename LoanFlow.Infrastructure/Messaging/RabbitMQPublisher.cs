using System.Text;
using System.Text.Json;
using LoanFlow.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace LoanFlow.Infrastructure.Messaging;

public class RabbitMQPublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly RabbitMQSettings _settings;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private const string ExchangeName = "loanflow.events";

    public RabbitMQPublisher(IOptions<RabbitMQSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : class
    {
        await EnsureConnectionAsync(cancellationToken);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = Guid.NewGuid().ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await _channel!.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    private async Task EnsureConnectionAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
            return;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_channel is { IsOpen: true })
                return;

            var factory = RabbitMQConnectionFactory.Create(_settings);

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.CloseAsync();

        if (_connection is not null)
            await _connection.CloseAsync();

        _semaphore.Dispose();
    }
}
