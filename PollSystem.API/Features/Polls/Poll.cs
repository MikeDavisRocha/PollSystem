using System.ComponentModel.DataAnnotations;

namespace PollSystem.API.Features.Polls;

public sealed record Poll
{
    public int Id { get; init; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; init; } = string.Empty;
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public bool IsActive { get; init; } = true;
    
    public ICollection<PollOption> Options { get; init; } = new List<PollOption>();
}
