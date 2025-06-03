using System.ComponentModel.DataAnnotations;

namespace ŠišAppApi.Models.DTOs;

public class CreateReviewDto
{
    [Required]
    public int AppointmentId { get; set; }

    [Required]
    public int BarberId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(500)]
    public string Comment { get; set; }
} 