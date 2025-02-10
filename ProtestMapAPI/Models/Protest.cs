using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ProtestMapAPI.Models;

public class Protest
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public required string Title { get; set; }

    [Required]
    [StringLength(255)]
    public required string Street { get; set; }

    [Required]
    [StringLength(100)]
    public required string City { get; set; }

    [Required]
    [StringLength(50)]
    public required string State { get; set; }

    [Required]
    [StringLength(20)]
    public required string ZipCode { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [StringLength(500)]
    public required string Cause { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public string? CreatedByUserId { get; set; }

    [ForeignKey("CreatedByUserId")]
    [JsonIgnore] // Prevents serialization in API responses
    public ApplicationUser? CreatedByUser { get; set; }
}
