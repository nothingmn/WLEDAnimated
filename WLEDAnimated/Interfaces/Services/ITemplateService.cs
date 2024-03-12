namespace WLEDAnimated.Interfaces.Services;

public interface ITemplateService
{
    Task<string> Replace(string text, object state);
}