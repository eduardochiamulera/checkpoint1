using System;
using Cursos.Domain.Exceptions;
using Cursos.Domain.Payments;
using FluentAssertions;
using Xunit;

namespace Cursos.Tests.Domain;

public sealed class PaymentTests
{
    private static readonly Guid EnrollmentId =
        Guid.NewGuid();

    private const int StudentId = 10;
    private const string UserId =
        "7f8d8c3a-2f52-4a3e-9b7a-1d2f9e8a1234";

    private static Payment CreatePayment()
    {
        return Payment.Create(
            enrollmentId: 1,
            studentId: StudentId,
            userId: UserId,
            amount: Money.Create(100m, "BRL"),
            method: PaymentMethod.Create(
                PaymentMethodType.Pix),
            idempotencyKey: "unit-test-key");
    }

    [Fact]
    public void Should_start_as_pending()
    {
        var payment = CreatePayment();

        payment.Status
            .Should()
            .Be(PaymentStatus.Pending);
    }

    [Fact]
    public void Should_transition_from_pending_to_paid()
    {
        var payment = CreatePayment();

        payment.Confirm();

        payment.Status
            .Should()
            .Be(PaymentStatus.Paid);
        payment.Transitions
            .Should()
            .ContainSingle(value =>
                value.From == PaymentStatus.Pending &&
                value.To == PaymentStatus.Paid);
    }

    [Fact]
    public void Should_transition_from_pending_to_failed()
    {
        var payment = CreatePayment();

        payment.Fail("Gateway recusou o pagamento.");

        payment.Status
            .Should()
            .Be(PaymentStatus.Failed);
    }

    [Fact]
    public void Should_refund_only_paid_payment()
    {
        var payment = CreatePayment();
        payment.Confirm();

        payment.Refund("Solicitado pelo cliente.");

        payment.Status
            .Should()
            .Be(PaymentStatus.Refunded);
    }

    [Fact]
    public void Should_reject_refund_of_pending_payment()
    {
        var payment = CreatePayment();

        var action = () => payment.Refund("Teste");

        action.Should()
            .Throw<DomainException>()
            .Which.Code
            .Should()
            .Be("invalid_payment_transition");
    }

    [Fact]
    public void Should_reject_non_positive_amount()
    {
        var action = () => Money.Create(0m, "BRL");

        action.Should()
            .Throw<DomainException>()
            .Which.Code
            .Should()
            .Be("invalid_payment_amount");
    }

    [Fact]
    public void Should_reject_invalid_currency()
    {
        var action = () => Money.Create(10m, "br");

        action.Should()
            .Throw<DomainException>()
            .Which.Code
            .Should()
            .Be("invalid_currency");
    }

    [Fact]
    public void Should_reject_missing_idempotency_key()
    {
        var action = () => Payment.Create(
            1,
            StudentId,
            UserId,
            Money.Create(10m, "BRL"),
            PaymentMethod.Create(PaymentMethodType.Pix),
            "");

        action.Should()
            .Throw<DomainException>()
            .Which.Code
            .Should()
            .Be("missing_idempotency_key");
    }
}