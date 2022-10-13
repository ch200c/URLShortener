namespace URLShortener.Application.Services;

public class AliasGenerator : IAliasGenerator
{
    private static readonly Random _random = new();

    public string Generate(GenerateAliasRequest request)
    {
        var alias = new char[request.Length];

        for (var i = 0; i < request.Length; i++)
        {
            var charIndex = _random.Next(0, request.AllowedChars.Length);
            alias[i] = request.AllowedChars[charIndex];
        }

        return new string(alias);
    }
}
