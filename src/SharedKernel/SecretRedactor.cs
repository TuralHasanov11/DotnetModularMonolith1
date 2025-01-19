using Microsoft.Extensions.Compliance.Redaction;

namespace SharedKernel.Security;

public class SecretRedactor : Redactor
{
    private const string _secretValue = "*****";

    public override int GetRedactedLength(ReadOnlySpan<char> input)
    {
        return _secretValue.Length;
    }

    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        _secretValue.CopyTo(destination);
        return _secretValue.Length;
    }
}