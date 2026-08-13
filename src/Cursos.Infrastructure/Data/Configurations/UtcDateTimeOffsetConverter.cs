using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cursos.Infrastructure.Data.Configurations;

public class UtcDateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public UtcDateTimeOffsetConverter() 
        : base(
            dto => dto.ToUniversalTime(),
            dto => dto.ToUniversalTime())
    {
    }
}

public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() 
        : base(
            dt => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
            dt => DateTime.SpecifyKind(dt, DateTimeKind.Utc))
    {
    }
}
