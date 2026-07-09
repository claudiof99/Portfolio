using System.ComponentModel.DataAnnotations;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.ViewModels;

public sealed class ResetPasswordViewModel
{
	[Required(ErrorMessage = "Validation_Required")]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "Validation_Required")]
	public string Token { get; set; } = string.Empty;

	[Required(ErrorMessage = "Validation_Required")]
	[DataType(DataType.Password)]
	[MinLength(6, ErrorMessage = "Validation_MinLength")]
	public string NewPassword { get; set; } = string.Empty;

	[DataType(DataType.Password)]
	[Compare(nameof(NewPassword), ErrorMessage = "Validation_Compare")]
	public string ConfirmPassword { get; set; } = string.Empty;
}
