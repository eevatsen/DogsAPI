using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dogs.Models;

public class Dog
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("color")]
    public string Color { get; set; } = string.Empty;

    [Required]
    [Column("tail_length")]
    [JsonPropertyName("tail_length")]
    public int TailLength { get; set; }

    [Required]
    [Column("weight")]
    public int Weight { get; set; }
}


public class CreateDogRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Color is required")]
    [MaxLength(100)]
    public string Color { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tail length is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Tail length must be zero or positive")]
    [JsonPropertyName("tail_length")]
    public int TailLength { get; set; }

    [Required(ErrorMessage = "Weight is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Weight must be positive")]
    public int Weight { get; set; }
}

public class DogsQueryParams
{
    public string? Attribute { get; set; }
    public string? Order { get; set; } = "asc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}