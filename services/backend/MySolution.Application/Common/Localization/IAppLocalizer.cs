namespace MySolution.Application.Common.Localization;

public interface IAppLocalizer
{
    string this[string key] { get; }
}