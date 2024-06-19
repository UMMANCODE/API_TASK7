using System;
using System.ComponentModel.DataAnnotations;

namespace TASK3_UI.Resources {
  public class StudentCreateRequest {
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    [Required]
    public DateTime? BirthDate { get; set; }
    [Required]
    public int GroupId { get; set; }
    public IFormFile Photo { get; set; }
  }
}

