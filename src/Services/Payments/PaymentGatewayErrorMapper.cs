using Cursos.Domain.Exceptions;
using Cursos.Domains.Payments;

namespace Cursos.Services.Payments;

public static class PaymentGatewayErrorMapper
{
    public static DomainException ToDomainException(
        GatewayOperationStatus status,
        string? errorCode,
        string? errorMessage)
    {
        return status switch
        {
            GatewayOperationStatus.Declined => new DomainException(
                "payment_declined",
                "O pagamento foi recusado."),

            GatewayOperationStatus.Timeout => new DomainException(
                "payment_gateway_timeout",
                "O gateway de pagamento excedeu o tempo limite."),

            GatewayOperationStatus.Failed => new DomainException(
                "payment_gateway_failure",
                "Não foi possível processar o pagamento."),

            _ => new DomainException(
                errorCode ?? "payment_gateway_error",
                errorMessage ?? "Erro no gateway de pagamento.")
        };
    }
}