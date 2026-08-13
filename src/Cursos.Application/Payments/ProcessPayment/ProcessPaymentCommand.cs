using System;
using Cursos.Application.Common;

namespace Cursos.Application.Payments.ProcessPayment;

public record ProcessPaymentCommand(
    Guid EnrollmentId,
    decimal Amount,
    string PaymentMethodType
) : ICommand<PaymentResultDto>;
