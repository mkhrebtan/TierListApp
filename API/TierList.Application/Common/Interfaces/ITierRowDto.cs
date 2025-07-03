namespace TierList.Application.Common.Interfaces;

public interface ITierRowDto
{
    int Id { get; init; }

    string Rank { get; init; }

    string ColorHex { get; init; }

    int Order { get; init; }
}
