namespace Feane.Models;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
