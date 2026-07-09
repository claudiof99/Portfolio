
namespace UmaFestHub.Application.Validation;
public interface IValidationDictionary
{
    void AddError(string key, string message);
    bool IsValid { get; }
}