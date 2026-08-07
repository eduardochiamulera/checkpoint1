namespace Cursos.Domains.Payments;

public interface IPaymentGateway
{
    Task<GatewayCreateResult> CreateAsync(
        GatewayCreateRequest request,
        CancellationToken cancellationToken = default);

    Task<GatewayOperationResult> ConfirmAsync(
        GatewayConfirmRequest request,
        CancellationToken cancellationToken = default);

    Task<GatewayOperationResult> RefundAsync(
        GatewayRefundRequest request,
        CancellationToken cancellationToken = default);
}