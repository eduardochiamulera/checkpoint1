using Cursos.Application.Common;

namespace Cursos.Application.Payments.GetPaymentByEnrollment;

public record GetPaymentByEnrollmentQuery(Guid EnrollmentId) : IQuery<PaymentDto?>;
