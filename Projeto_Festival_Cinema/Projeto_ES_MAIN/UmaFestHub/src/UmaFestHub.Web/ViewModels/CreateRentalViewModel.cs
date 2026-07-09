using System.ComponentModel.DataAnnotations;
using UmaFestHub.Web.Resources;

namespace UmaFestHub.Web.ViewModels;

public sealed class CreateRentalViewModel
{
    public Guid FestivalFilmId { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    [Range(1, 365, ErrorMessage = "Validation_DurationRange")]
    public int DurationValue { get; set; }

    [Required]
    public string DurationUnit { get; set; } = "Hours";
}
