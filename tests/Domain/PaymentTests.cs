using System;
using Cursos.Domain.Exceptions;
using Cursos.Domain.Payments;
using FluentAssertions;
using Xunit;

namespace Cursos.Tests.Domain;

public sealed class PaymentTests
{
    private static readonly Guid EnrollmentId = Guid.NewGuid();

    private static Payment CreatePayment(
        decimal amount = 100m,
        string currency = "BRL",
        PaymentMethodType methodType = PaymentMethodType.Pix)
    {
        return new Payment(
            enrollmentId: EnrollmentId,
            amount: new Money(amount, currency),
            paymentMethodType: methodType);
    }

    [Fact]
    public void Should_start_as_pending()
    {
        // Arrange
        var payment = CreatePayment();

        // Act
        var status = payment.Status;

        // Assert
        status.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public void Should_transition_from_pending_to_confirmed()
    {
        // Arrange
        var payment = CreatePayment();
        const string transactionId = "txn_unit_test_123";

        // Act
        payment.Confirm(transactionId);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Confirmed);
        payment.GatewayTransactionId.Should().Be(transactionId);
        payment.StatusTransitions
            .Should()
            .Contain(t =>
                t.FromStatus == PaymentStatus.Pending.ToString() &&
                t.ToStatus == PaymentStatus.Confirmed.ToString());
    }

    [Fact]
    public void Should_transition_from_pending_to_failed()
    {
        // Arrange
        var payment = CreatePayment();

        // Act
        payment.MarkAsFailed("Gateway recusou o pagamento.");

        // Assert
        payment.Status.Should().Be(PaymentStatus.Failed);
        payment.StatusTransitions
            .Should()
            .Contain(t =>
                t.FromStatus == PaymentStatus.Pending.ToString() &&
                t.ToStatus == PaymentStatus.Failed.ToString() &&
                t.Reason == "Gateway recusou o pagamento.");
    }

    [Fact]
    public void Should_transition_from_pending_to_cancelled()
    {
        // Arrange
        var payment = CreatePayment();

        // Act
        payment.Cancel("Cancelado pelo usuario.");

        // Assert
        payment.Status.Should().Be(PaymentStatus.Cancelled);
    }

    [Fact]
    public void Should_refund_only_confirmed_payment()
    {
        // Arrange
        var payment = CreatePayment();
        payment.Confirm("txn_unit_test_456");

        // Act
        payment.Refund("Solicitado pelo cliente.");

        // Assert
        payment.Status.Should().Be(PaymentStatus.Refunded);
        payment.StatusTransitions
            .Should()
            .Contain(t =>
                t.FromStatus == PaymentStatus.Confirmed.ToString() &&
                t.ToStatus == PaymentStatus.Refunded.ToString() &&
                t.Reason == "Solicitado pelo cliente.");
    }

    [Fact]
    public void Should_reject_refund_of_pending_payment()
    {
        // Arrange
        var payment = CreatePayment();

        // Act
        var action = () => payment.Refund("Teste");

        // Assert
        action.Should().Throw<DomainException>();
        payment.Status.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public void Should_reject_confirm_of_already_confirmed_payment()
    {
        // Arrange
        var payment = CreatePayment();
        payment.Confirm("txn_first");

        // Act
        var action = () => payment.Confirm("txn_second");

        // Assert
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_reject_cancel_of_confirmed_payment()
    {
        // Arrange
        var payment = CreatePayment();
        payment.Confirm("txn_confirmed");

        // Act
        var action = () => payment.Cancel("Tentativa invalida");

        // Assert
        action.Should().Throw<DomainException>();
        payment.Status.Should().Be(PaymentStatus.Confirmed);
    }

    [Fact]
    public void Should_reject_negative_amount()
    {
        // Arrange
        // Act
        var action = () => new Money(-10m, "BRL");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Amount cannot be negative");
    }

    [Fact]
    public void Should_allow_zero_amount()
    {
        // Arrange
        // Act
        var action = () => new Money(0m, "BRL");

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void Should_default_currency_to_BRL_when_not_specified()
    {
        // Arrange
        // Act
        var money = new Money(50m);

        // Assert
        money.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Should_add_money_with_same_currency()
    {
        // Arrange
        var first = new Money(50m, "BRL");
        var second = new Money(30m, "BRL");

        // Act
        var result = first.Add(second);

        // Assert
        result.Amount.Should().Be(80m);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Should_reject_adding_money_with_different_currencies()
    {
        // Arrange
        var brl = new Money(50m, "BRL");
        var usd = new Money(30m, "USD");

        // Act
        var action = () => brl.Add(usd);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot add money with different currencies");
    }

    [Fact]
    public void Should_consider_money_with_same_amount_and_currency_equal()
    {
        // Arrange
        var first = new Money(20m, "BRL");
        var second = new Money(20m, "BRL");

        // Act
        var areEqual = first.Equals(second);

        // Assert
        areEqual.Should().BeTrue();
    }
}
