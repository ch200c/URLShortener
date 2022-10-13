namespace URLShortener.Application.Services;

public interface IAliasGenerator
{
    string Generate(GenerateAliasRequest request);
}
