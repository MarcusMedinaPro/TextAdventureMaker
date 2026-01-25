namespace MarcusMedina.TextAdventure.Localization;

public interface ILanguageProvider
{
    string Get(string key);
    string Format(string key, params object[] args);
}
