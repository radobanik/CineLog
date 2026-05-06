// src/api/CineLog.Infrastructure/Data/Seeders/UserFollowSeeder.cs
using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class UserFollowSeeder
{
    private static readonly Dictionary<string, string[]> FollowGraph = new()
    {
        ["dave@cinelog.dev"] = ["carol@cinelog.dev"],

        ["alice@cinelog.dev"] = ["bob@cinelog.dev", "carol@cinelog.dev", "lucy@cinelog.dev", "karsten@cinelog.dev", "sofia@cinelog.dev"],
        ["bob@cinelog.dev"] = ["alice@cinelog.dev", "carol@cinelog.dev", "noah@cinelog.dev", "leo@cinelog.dev"],
        ["carol@cinelog.dev"] = ["alice@cinelog.dev", "bob@cinelog.dev", "dave@cinelog.dev", "lucy@cinelog.dev", "nina@cinelog.dev"],

        ["lucy@cinelog.dev"] = ["alice@cinelog.dev", "carol@cinelog.dev", "karsten@cinelog.dev", "mila@cinelog.dev", "emma@cinelog.dev", "ivy@cinelog.dev"],
        ["karsten@cinelog.dev"] = ["bob@cinelog.dev", "lucy@cinelog.dev", "leo@cinelog.dev", "nina@cinelog.dev", "oscar@cinelog.dev"],
        ["mila@cinelog.dev"] = ["lucy@cinelog.dev", "sofia@cinelog.dev", "emma@cinelog.dev", "tom@cinelog.dev"],
        ["noah@cinelog.dev"] = ["bob@cinelog.dev", "karsten@cinelog.dev", "mateo@cinelog.dev", "oscar@cinelog.dev"],
        ["sofia@cinelog.dev"] = ["alice@cinelog.dev", "mila@cinelog.dev", "lucy@cinelog.dev", "ivy@cinelog.dev"],
        ["leo@cinelog.dev"] = ["bob@cinelog.dev", "karsten@cinelog.dev", "nina@cinelog.dev", "tom@cinelog.dev"],
        ["emma@cinelog.dev"] = ["lucy@cinelog.dev", "mila@cinelog.dev", "sofia@cinelog.dev", "ivy@cinelog.dev", "tom@cinelog.dev"],
        ["mateo@cinelog.dev"] = ["dave@cinelog.dev", "noah@cinelog.dev", "oscar@cinelog.dev", "tom@cinelog.dev"],
        ["nina@cinelog.dev"] = ["carol@cinelog.dev", "karsten@cinelog.dev", "leo@cinelog.dev", "ivy@cinelog.dev"],
        ["oscar@cinelog.dev"] = ["noah@cinelog.dev", "mateo@cinelog.dev", "karsten@cinelog.dev", "emma@cinelog.dev"],
        ["ivy@cinelog.dev"] = ["lucy@cinelog.dev", "sofia@cinelog.dev", "nina@cinelog.dev", "emma@cinelog.dev"],
        ["tom@cinelog.dev"] = ["mateo@cinelog.dev", "mila@cinelog.dev", "leo@cinelog.dev", "oscar@cinelog.dev"]
    };

    internal static async Task SeedAsync(IAppDbContext context)
    {
        var users = await context.Users.ToListAsync();

        if (users.Count == 0)
            return;

        await EnforceDaveFollowsOnlyCarolAsync(context, users);

        foreach (var (followerEmail, followedEmails) in FollowGraph)
        {
            var follower = users.FirstOrDefault(u => u.Email == followerEmail);
            if (follower is null)
                continue;

            foreach (var followedEmail in followedEmails)
            {
                var followed = users.FirstOrDefault(u => u.Email == followedEmail);
                if (followed is null || followed.Id == follower.Id)
                    continue;

                var exists = await context.UserFollows.AnyAsync(f =>
                    f.FollowerId == follower.Id &&
                    f.FollowedId == followed.Id);

                if (!exists)
                    context.UserFollows.Add(UserFollow.Create(follower.Id, followed.Id));
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnforceDaveFollowsOnlyCarolAsync(
        IAppDbContext context,
        IReadOnlyList<User> users)
    {
        var dave = users.FirstOrDefault(u => u.Email == "dave@cinelog.dev");
        var carol = users.FirstOrDefault(u => u.Email == "carol@cinelog.dev");

        if (dave is null || carol is null)
            return;

        var extraDaveFollows = await context.UserFollows
            .Where(f => f.FollowerId == dave.Id && f.FollowedId != carol.Id)
            .ToListAsync();

        foreach (var follow in extraDaveFollows)
            context.UserFollows.Remove(follow);
    }
}
