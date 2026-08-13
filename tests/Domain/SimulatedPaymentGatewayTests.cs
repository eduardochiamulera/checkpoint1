using System;
using System.Threading.Tasks;
using Cursos.Domain.Payments;
using Cursos.Infrastructure.Gateways;
using FluentAssertions;
using Xunit;

namespace Cursos.Tests.Domain;

public sealed class SimulatedPaymentGatewayTests
{
    [Fact]
    public async Task Should_return_deterministic_success()
    {
        var gateway = new SimulatedPaymentGateway();
        var request = new GatewayCreateRequest(
            Guid.NewGuid(),
            100m,
            "BRL",
            PaymentMethodType.Pix,
            "same-key");

        var first = await gateway.CreateAsync(request);
        var second = await gateway.CreateAsync(request);

        first.Status.Should().Be(GatewayOperationStatus.Succeeded);
        first.ExternalPaymentId
            .Should()
            .Be(second.ExternalPaymentId);
        first.Receipt.Should().Be(second.Receipt);
    }

    [Fact]
    public async Task Should_return_declined_for_decline_key()
    {
        var gateway = new SimulatedPaymentGateway();
        var request = new GatewayCreateRequest(
            Guid.NewGuid(),
            100m,
            "BRL",
            PaymentMethodType.Pix,
            "decline-test");

        var result = await gateway.CreateAsync(request);

        result.Status
            .Should()
            .Be(GatewayOperationStatus.Declined);
        result.ErrorCode
            .Should()
            .Be("SIMULATED_DECLINED");
    }

    [Fact]
    public async Task Should_return_timeout_for_timeout_key()
    {
        var gateway = new SimulatedPaymentGateway();
        var request = new GatewayCreateRequest(
            Guid.NewGuid(),
            100m,
            "BRL",
            PaymentMethodType.Pix,
            "timeout-test");

        var result = await gateway.CreateAsync(request);

        result.Status
            .Should()
            .Be(GatewayOperationStatus.Timeout);
    }
}