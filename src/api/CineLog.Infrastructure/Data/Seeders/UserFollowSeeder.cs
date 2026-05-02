using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class UserFollowSeeder
{
    private static readonly Dictionary<string, string[]> FollowGraph = new()
    {
        ["dave@cinelog.dev"] =
        [
            "carol@cinelog.dev"
        ],

        ["alice@cinelog.dev"] =
        [
            "bob@cinelog.dev",
            "carol@cinelog.dev",
            "dave@cinelog.dev"
        ],

        ["bob@cinelog.dev"] =
        [
            "alice@cinelog.dev",
            "carol@cinelog.dev"
        ],

        ["carol@cinelog.dev"] =
        [
            "alice@cinelog.dev",
            "bob@cinelog.dev",
            "dave@cinelog.dev"
        ]
    };

    internal static async Task SeedAsync(IAppDbContext context)
    {
        var users = await context.Users.ToListAsync();

        if (users.Count == 0)
            return;

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
}
