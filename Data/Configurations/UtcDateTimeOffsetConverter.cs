using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cursos.Data.Configurations;

public sealed class UtcDateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTime>
{
    public UtcDateTimeOffsetConverter()
        : base(
            value => value.UtcDateTime,
            value => new DateTimeOffset(
                DateTime.SpecifyKind(
                    value,
                    DateTimeKind.Utc)))
    {
    }
}