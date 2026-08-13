using Cursos.Domain.Payments;
using Cursos.Infrastructure.Gateways;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Cursos.Tests.Domain;

public sealed class SimulatedPaymentGatewayTests
{
    private static SimulatedPaymentGateway CreateGateway()
    {
        return new SimulatedPaymentGateway(NullLogger<SimulatedPaymentGateway>.Instance);
    }

    private static Payment CreatePayment(decimal amount = 100m)
    {
        return new Payment(
            enrollmentId: Guid.NewGuid(),
            amount: new Money(amount, "BRL"),
            paymentMethodType: PaymentMethodType.Pix);
    }

    [Fact]
    public async Task Should_return_a_result_for_every_processed_payment()
    {
        // Arrange
        var gateway = CreateGateway();
        var payment = CreatePayment();

        // Act
        var result = await gateway.ProcessAsync(payment);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_return_transaction_id_when_successful()
    {
        // Arrange
        var gateway = CreateGateway();
        var payment = CreatePayment();

        // Act
        // NOTE: SimulatedPaymentGateway has a ~10% random failure rate (Random.Shared).
        // We assert the *contract* for a successful outcome rather than forcing determinism,
        // since the current implementation has no seam to control randomness.
        PaymentResult? successResult = null;
        for (var attempt = 0; attempt < 20 && successResult is null; attempt++)
        {
            var result = await gateway.ProcessAsync(payment);
            if (result.Success)
            {
                successResult = result;
            }
        }

        // Assert
        successResult.Should().NotBeNull("gateway succeeds ~90% of the time; 20 attempts should yield at least one success");
        successResult!.TransactionId.Should().NotBeNullOrWhiteSpace();
        successResult.TransactionId.Should().StartWith("sim_");
        successResult.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task Should_return_error_message_when_failed()
    {
        // Arrange
        var gateway = CreateGateway();
        var payment = CreatePayment();

        // Act
        PaymentResult? failureResult = null;
        for (var attempt = 0; attempt < 60 && failureResult is null; attempt++)
        {
            var result = await gateway.ProcessAsync(payment);
            if (!result.Success)
            {
                failureResult = result;
            }
        }

        // Assert
        failureResult.Should().NotBeNull("gateway fails ~10% of the time; 60 attempts should yield at least one failure");
        failureResult!.TransactionId.Should().BeNull();
        failureResult.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Should_refund_successfully_for_any_transaction_id()
    {
        // Arrange
        var gateway = CreateGateway();

        // Act
        var result = await gateway.RefundAsync("sim_any_transaction_id");

        // Assert
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task Should_respect_cancellation_token_during_processing()
    {
        // Arrange
        var gateway = CreateGateway();
        var payment = CreatePayment();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var action = async () => await gateway.ProcessAsync(payment, cts.Token);

        // Assert
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
}
