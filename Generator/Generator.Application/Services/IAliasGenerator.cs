namespace Generator.Application.Services;

public interface IAliasGenerator
{
    string Generate(GenerateAliasRequest request);
}
