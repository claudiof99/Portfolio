
using UmaFestHub.Application.Validation;
using UmaFestHub.Web.ViewModels;
using UmaFestHub.Web.Resources;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;

namespace UmaFestHub.Web.Validation.Validators;
public class EditProfileViewModelValidator: IViewModelValidator<EditProfileViewModel>
{
	private readonly IStringLocalizer<SharedResources> _localizer;

	public EditProfileViewModelValidator(IStringLocalizer<SharedResources> localizer)
	{
		_localizer = localizer;
	}

    public void Validate(EditProfileViewModel model, IValidationDictionary vd)
    {
        if (!string.IsNullOrWhiteSpace(model.NewName))
        {
           if(model.NewName.Length > 25 || model.NewName.Length < 5)
            {
                vd.AddError(nameof(model.NewName), _localizer["EditProfile_NameLength"].Value);
            }

            if(Regex.IsMatch(model.NewName, @"[^a-zA-Z0-9\s]"))
            {
                vd.AddError(nameof(model.NewName), _localizer["EditProfile_NameSpecialChars"].Value);
            }
          
        }
        


        
    }
}
