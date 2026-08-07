using System.ComponentModel.DataAnnotations;
using Cursos.Domain.Payments;

namespace Cursos.Models.Payments;

public sealed class CreatePaymentRequest : IValidatableObject
{
    [Required]
    [Range(0.01, 999999999.99)]
    public decimal Amount { get; init; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    [RegularExpression(
        "^[A-Z]{3}$",
        ErrorMessage = "Currency deve possuir três letras maiúsculas.")]
    public string Currency { get; init; } = null!;

    [Range(1, int.MaxValue)]
    public int EnrollmentId { get; init; }

    [Required]
    [EnumDataType(typeof(PaymentMethodType))]
    public PaymentMethodType? Method { get; init; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (Amount <= 0)
        {
            yield return new ValidationResult(
                "Amount deve ser maior que zero.",
                [nameof(Amount)]);
        }

        if (EnrollmentId <= 0)
        {
            yield return new ValidationResult(
                "EnrollmentId deve ser maior que zero.",
                [nameof(EnrollmentId)]);
        }

        if (Method is null ||
            !Enum.IsDefined(Method.Value))
        {
            yield return new ValidationResult(
                "Method é obrigatório e deve ser válido.",
                [nameof(Method)]);
        }
    }
}