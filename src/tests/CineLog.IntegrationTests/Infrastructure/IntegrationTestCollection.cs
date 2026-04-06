namespace CineLog.IntegrationTests.Infrastructure;

/// <summary>
/// Shares a single PostgreSQL container (via CineLogWebApplicationFactory)
/// across all test classes in this collection.
/// </summary>
[CollectionDefinition(nameof(IntegrationTestCollection))]
public class IntegrationTestCollection : ICollectionFixture<CineLogWebApplicationFactory> { }
