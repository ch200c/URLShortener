namespace URLShortener.Application.UnitTests.Services;

public class ShortenedEntryCreationServiceTests
{
    [Fact]
    public async Task CreateAsync_RequestWithoutAlias_CreatesShortenedEntryWithGeneratedAlias()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        const string generatedAlias = "abcd";

        var aliasServiceMock = new Mock<IAliasService>();

        aliasServiceMock
            .Setup(aliasService => aliasService.GetAvailableAliasAsync(cancellationToken))
            .ReturnsAsync(generatedAlias);

        var sut = new ShortenedEntryCreationService(aliasServiceMock.Object);
        var request = new CreateShortenedEntryRequest(Alias: null, Url: string.Empty, Expiration: default);

        // Act
        var shortenedEntry = await sut.CreateAsync(request, cancellationToken);

        // Assert
        Assert.Equal(generatedAlias, shortenedEntry.Alias);
    }

    [Fact]
    public async Task CreateAsync_RequestWithAlias_CreatesShortenedEntryWithRequestAlias()
    {
        // Arrange
        const string alias = "abcd";
        var aliasServiceMock = new Mock<IAliasService>();

        var sut = new ShortenedEntryCreationService(aliasServiceMock.Object);
        var request = new CreateShortenedEntryRequest(Alias: alias, Url: string.Empty, Expiration: default);

        // Act
        var shortenedEntry = await sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(alias, shortenedEntry.Alias);
    }
}
