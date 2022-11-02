namespace URLShortener.Application.UnitTests.Services;

public class AliasGeneratorTests
{
    [Fact]
    public void Generate_SpecificLength_GeneratesSpecifiedLengthAlias()
    {
        // Arrange
        var sut = new AliasGenerator();
        const int aliasLength = 1;
        var request = new GenerateAliasRequest(Length: aliasLength, AllowedChars: new char[] { 'a' });

        // Act
        var alias = sut.Generate(request);

        // Assert
        Assert.Equal(aliasLength, alias.Length);
    }

    [Fact]
    public void Generate_SpecificAllowedChars_GeneratesSpecifiedAllowedCharsAlias()
    {
        // Arrange
        var sut = new AliasGenerator();

        const int aliasLength = 1;
        const char allowedChar = 'a';
        var request = new GenerateAliasRequest(Length: aliasLength, AllowedChars: new char[] { allowedChar });

        // Act
        var alias = sut.Generate(request);

        // Assert
        Assert.Equal(aliasLength, alias.Count(@char => @char == allowedChar));
    }
}
