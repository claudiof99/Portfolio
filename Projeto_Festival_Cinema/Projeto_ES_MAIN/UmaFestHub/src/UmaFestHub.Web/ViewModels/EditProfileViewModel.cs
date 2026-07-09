
using System.ComponentModel.DataAnnotations;

namespace UmaFestHub.Web.ViewModels;
public sealed class EditProfileViewModel
{
    public Guid Id {get; set;}

    [EmailAddress]
    public string? NewEmail { get; set; }

    [MinLength(2)]
    public string? NewName { get; set; }
}