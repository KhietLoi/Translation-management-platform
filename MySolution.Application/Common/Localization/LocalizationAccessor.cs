using MySolution.Application.Common.Interfaces;

namespace MySolution.Application.Common.Localization;

public class LocalizationAccessor
{
    private static ILocalizationService? _localizer;

    public static ILocalizationService Localizer
        => _localizer ?? throw new InvalidOperationException(
            "Localization not configured. Call LocalizationAccessor.Configure() at startup.");

    // Gọi 1 lần ở startup
    public static void Configure(ILocalizationService localizer)
    {
        _localizer = localizer;
    }
}