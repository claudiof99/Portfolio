using System.ComponentModel.DataAnnotations;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.ViewModels;

public sealed class CreatePassViewModel
{
    public Guid FestivalId { get; set; }
    
    [Required]
    public string PassType { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }
}
