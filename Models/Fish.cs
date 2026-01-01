using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Fish
{
    public int Id { get; set; }

    [Required]
    public string FishName { get; set; }


    [Required]
    [MaxLength(500)]
    public string Description { get; set; }

    [Required]
    [Range(0.01, 99999, ErrorMessage = "Price must be between 1 and 99,999 AED")]
    public decimal Price { get; set; }

    public string? ImagePath { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }
}
