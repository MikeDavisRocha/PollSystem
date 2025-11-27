using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PollSystem.API.Data;
using PollSystem.API.Hubs;

namespace PollSystem.API.Features.Polls;

public static class PollsEndpoints
{
    public static void MapPollEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/polls");

        group.MapPost("/", CreatePoll);
        group.MapGet("/{id}", GetPoll);
        group.MapPost("/{id}/vote", Vote);
    }

    public record CreatePollRequest(string Title, List<string> Options);

    private static async Task<IResult> CreatePoll(
        CreatePollRequest request,
        AppDbContext db)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || request.Options.Count < 2)
        {
            return Results.BadRequest("Title and at least 2 options are required.");
        }

        var poll = new Poll
        {
            Title = request.Title,
            Options = request.Options.Select(o => new PollOption { Text = o }).ToList()
        };

        db.Polls.Add(poll);
        await db.SaveChangesAsync();

        return Results.Created($"/api/polls/{poll.Id}", poll);
    }

    private static async Task<IResult> GetPoll(int id, AppDbContext db)
    {
        var poll = await db.Polls
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == id);

        return poll is null ? Results.NotFound() : Results.Ok(poll);
    }

    public record VoteRequest(int OptionId);

    private static async Task<IResult> Vote(
        int id,
        VoteRequest request,
        AppDbContext db,
        IHubContext<PollHub> hub)
    {
        var option = await db.PollOptions.FindAsync(request.OptionId);

        if (option is null || option.PollId != id)
        {
            return Results.BadRequest("Invalid option.");
        }

        option = option with { VoteCount = option.VoteCount + 1 };
        
        // EF Core tracking might need explicit update if we replaced the record instance, 
        // but since we loaded it, let's just modify the property if it was a class.
        // Wait, PollOption is a record. Records are immutable by default if using 'with', 
        // but EF Core tracks entities. 
        // Actually, for EF Core with Records, it's better to treat them as mutable or let EF handle it.
        // My PollOption record definition:
        // public sealed record PollOption { ... public int VoteCount { get; init; } }
        // 'init' properties cannot be changed after initialization.
        // This is a problem for EF Core updates if I want to just increment.
        // I should probably change the entities to be mutable classes or use a different approach.
        // However, the prompt asked for "Records or Classes seladas".
        // If I use records with 'init', I have to replace the entity in the context or use 'ExecuteUpdate'.
        
        // Let's use ExecuteUpdate for atomicity and performance, which is also very .NET 8/9 style.
        
        await db.PollOptions
            .Where(o => o.Id == request.OptionId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(o => o.VoteCount, o => o.VoteCount + 1));

        // Notify clients
        await hub.Clients.All.SendAsync("ReceiveVote", id, request.OptionId);

        return Results.Ok();
    }
}
