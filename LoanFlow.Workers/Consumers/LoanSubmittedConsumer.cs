using LoanFlow.Configuration;
using LoanFlow.Contracts.Events;
using LoanFlow.Domain.Entities;
using LoanFlow.Infrastructure.Messaging;
using LoanFlow.Workers.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LoanFlow.Workers.Consumers;

public class LoanSubmittedConsumer : RabbitMQConsumer<LoanSubmittedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<LoanSubmittedConsumer> _logger;

    protected override string QueueName => "loanflow.loan-submitted";
    protected override string RoutingKey => "loan.submitted";

    public LoanSubmittedConsumer(
        IOptions<RabbitMQSettings> settings,
        IServiceScopeFactory scopeFactory,
        IMessagePublisher publisher,
        ILogger<LoanSubmittedConsumer> logger)
        : base(settings, logger)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
        _logger = logger;
    }

    protected override async Task HandleMessageAsync(LoanSubmittedEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing loan application {LoanApplicationId} for applicant {ApplicantId}",
            message.LoanApplicationId, message.ApplicantId);

        using var scope = _scopeFactory.CreateScope();
        var validationService = scope.ServiceProvider.GetRequiredService<LoanValidationService>();

        // Validate the loan application
        var result = await validationService.ValidateAsync(message.LoanApplicationId, cancellationToken);

        if (result.IsValid)
        {
            // Update status to UnderReview
            await validationService.UpdateStatusAsync(
                message.LoanApplicationId,
                LoanApplicationStatus.UnderReview,
                cancellationToken);

            // Publish event for next stage (credit scoring)
            var validatedEvent = new LoanValidatedEvent(
                message.LoanApplicationId,
                message.ApplicantId,
                message.Amount,
                result.IsValid,
                result.Errors,
                DateTime.UtcNow);

            await _publisher.PublishAsync(validatedEvent, "loan.validated", cancellationToken);

            _logger.LogInformation(
                "Loan application {LoanApplicationId} validated successfully, published LoanValidatedEvent",
                message.LoanApplicationId);
        }
        else
        {
            // Update status to Rejected
            await validationService.UpdateStatusAsync(
                message.LoanApplicationId,
                LoanApplicationStatus.Rejected,
                cancellationToken);

            _logger.LogWarning(
                "Loan application {LoanApplicationId} validation failed: {Errors}",
                message.LoanApplicationId, string.Join(", ", result.Errors));
        }
    }
}
