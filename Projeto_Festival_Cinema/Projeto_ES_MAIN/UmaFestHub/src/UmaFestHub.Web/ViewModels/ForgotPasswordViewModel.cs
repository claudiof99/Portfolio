using System.ComponentModel.DataAnnotations;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.ViewModels;

public sealed class ForgotPasswordViewModel
{
	[Required(ErrorMessage = "Validation_Required")]
	[EmailAddress(ErrorMessage = "Validation_Email")]
	public string Email { get; set; } = string.Empty;
}
