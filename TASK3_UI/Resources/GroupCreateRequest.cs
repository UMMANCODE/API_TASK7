using System.ComponentModel.DataAnnotations;

namespace TASK3_UI.Resources {
  public class GroupCreateRequest {
    [MaxLength(5)]
    [MinLength(4)]
    public string Name { get; set; }
    [Range(5, 18)]
    public byte Limit { get; set; }
  }
}
