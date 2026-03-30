using CineLog.Application.Common;
using CineLog.Application.Features.Reviews.CreateReview;
using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using CineLog.Domain.ValueObjects;
using CineLog.UnitTests.TestHelpers;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace CineLog.UnitTests.Application;

public class CreateReviewHandlerTests
{
    private static (
        TestAppDbContext ctx,
        IMovieRepository movieRepo,
        IReviewRepository reviewRepo,
        IUserRepository userRepo,
        ICurrentUserService currentUser,
        IPublisher publisher,
        CreateReviewHandler handler) BuildSut()
    {
        var ctx = TestAppDbContext.Create();
        var movieRepo = Substitute.For<IMovieRepository>();
        var reviewRepo = Substitute.For<IReviewRepository>();
        var userRepo = Substitute.For<IUserRepository>();
        var currentUser = Substitute.For<ICurrentUserService>();
        var publisher = Substitute.For<IPublisher>();

        var handler = new CreateReviewHandler(
            ctx, movieRepo, reviewRepo, userRepo, currentUser, publisher);

        return (ctx, movieRepo, reviewRepo, userRepo, currentUser, publisher, handler);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesReview()
    {
        var (ctx, movieRepo, reviewRepo, userRepo, currentUser, _, handler) = BuildSut();

        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var user = User.Create("testuser", "test@example.com", "hash");
        var movie = Movie.Create(12345, "Test Movie", MovieType.Movie);

        currentUser.UserId.Returns(userId);
        userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        movieRepo.GetByIdAsync(movieId, Arg.Any<CancellationToken>()).Returns(movie);

        // Side-effect: also save to ctx so the rating-recalculation query finds it
        reviewRepo.When(r => r.AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>()))
            .Do(ci =>
            {
                ctx.Reviews.Add(ci.ArgAt<Review>(0));
                ctx.SaveChanges();
            });

        var command = new CreateReviewCommand(movieId, 4.0m, "Great film", false);
        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Rating.Should().Be(4.0m);
        result.MovieTitle.Should().Be("Test Movie");
        result.Username.Should().Be(user.Username.Value);
    }

    [Fact]
    public async Task Handle_MovieNotFound_ThrowsNotFoundException()
    {
        var (_, movieRepo, _, userRepo, currentUser, _, handler) = BuildSut();

        var userId = Guid.NewGuid();
        currentUser.UserId.Returns(userId);
        userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(User.Create("testuser", "u@x.com", "hash"));
        movieRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Movie?)null);

        var act = async () => await handler.Handle(
            new CreateReviewCommand(Guid.NewGuid(), 3.0m, null, false),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReviewAlreadyExists_ThrowsConflictException()
    {
        var (ctx, movieRepo, reviewRepo, userRepo, currentUser, _, handler) = BuildSut();

        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();

        currentUser.UserId.Returns(userId);
        userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(User.Create("testuser", "u@x.com", "hash"));
        movieRepo.GetByIdAsync(movieId, Arg.Any<CancellationToken>())
            .Returns(Movie.Create(1, "Film", MovieType.Movie));

        // Pre-seed an existing review for same user + movie
        var existing = Review.Create(userId, movieId, Rating.Create(3.0m), null, false);
        ctx.Reviews.Add(existing);
        await ctx.SaveChangesAsync();

        var act = async () => await handler.Handle(
            new CreateReviewCommand(movieId, 4.0m, null, false),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        var (_, _, _, userRepo, currentUser, _, handler) = BuildSut();

        var userId = Guid.NewGuid();
        currentUser.UserId.Returns(userId);
        userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var act = async () => await handler.Handle(
            new CreateReviewCommand(Guid.NewGuid(), 3.0m, null, false),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
