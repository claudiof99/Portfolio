using System.ComponentModel.DataAnnotations;
using UmaFestHub.Domain.Enums;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.ViewModels;

public sealed class UserViewModel
{
	public Guid Id { get; set; }

	[Required(ErrorMessage = "Validation_Required")]
	[EmailAddress(ErrorMessage = "Validation_Email")]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "Validation_Required")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = string.Empty;

	public string FullName { get; set; } = string.Empty;
	public IReadOnlyList<string> Roles { get; set; } = [];
}
