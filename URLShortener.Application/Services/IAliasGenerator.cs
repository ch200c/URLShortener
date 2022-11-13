namespace UrlShortener.Application.Services;

public interface IAliasGenerator
{
    string Generate(GenerateAliasRequest request);
}
