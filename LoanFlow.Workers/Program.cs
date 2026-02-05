using LoanFlow.Configuration;
using LoanFlow.Infrastructure;
using LoanFlow.Infrastructure.Messaging;
using LoanFlow.Workers.Consumers;
using LoanFlow.Workers.Services;

var builder = Host.CreateApplicationBuilder(args);

// Configuration
var databaseSettings = builder.Services.AddDatabaseSettings(builder.Configuration);
builder.Services.AddRabbitMQSettings(builder.Configuration);

// Infrastructure
builder.Services.AddInfrastructure(databaseSettings);

// Messaging
builder.Services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

// Services
builder.Services.AddScoped<LoanValidationService>();

// Consumers
builder.Services.AddHostedService<LoanSubmittedConsumer>();

var host = builder.Build();
host.Run();
