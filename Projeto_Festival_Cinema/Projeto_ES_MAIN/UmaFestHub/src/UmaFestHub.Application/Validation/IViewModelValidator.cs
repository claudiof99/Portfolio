
namespace UmaFestHub.Application.Validation;
public interface IViewModelValidator<T>
{
    void Validate(T model, IValidationDictionary validationDictionary);
}