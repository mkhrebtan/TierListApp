using System.Text.RegularExpressions;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Entities;

namespace TierList.Application.Commands.TierRow;

public partial class UpdateTierRowColorCommand : IUpdateRowCommand
{
    private static Regex _colorHexRegex = CreateColorHexRegex();

    required public int Id { get; set; }

    required public int ListId { get; set; }

    required public string ColorHex { get; set; }

    public void UpdateRow(TierRowEntity rowEntity)
    {
        if (string.IsNullOrEmpty(ColorHex))
        {
            throw new ArgumentException("ColorHex cannot be null or empty.");
        }
        else if (!_colorHexRegex.IsMatch(ColorHex))
        {
            throw new ArgumentException("ColorHex must be a valid hex color code (e.g., #FFFFFF or #FFF).");
        }

        rowEntity.ColorHex = ColorHex;
    }

    [GeneratedRegex(@"^#([0-9a-fA-F]{6}|[0-9a-fA-F]{3})$")]
    private static partial Regex CreateColorHexRegex();
}
