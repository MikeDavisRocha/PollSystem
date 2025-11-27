using System.ComponentModel.DataAnnotations;

namespace PollSystem.API.Features.Polls;

public sealed record PollOption
{
    public int Id { get; init; }
    
    public int PollId { get; init; }
    
    [Required]
    [MaxLength(100)]
    public string Text { get; init; } = string.Empty;
    
    public int VoteCount { get; init; }
}
