
using UmaFestHub.Application.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
public class ModelStateWrapper : IValidationDictionary
{
    private readonly ModelStateDictionary _modelState;

    public ModelStateWrapper(ModelStateDictionary modelState)
    {
        _modelState = modelState;
    }

    public void AddError(string key, string message)
    {
        _modelState.AddModelError(key, message);
    }

    public bool IsValid => _modelState.IsValid;
}