using Microsoft.Extensions.Compliance.Classification;

namespace SharedKernel;

[AttributeUsage(AttributeTargets.Property)]
public class SensitiveDataAttribute : DataClassificationAttribute
{
    public SensitiveDataAttribute() : base(ApplicationLoggingTaxonomy.SensitiveData)
    {
    }
}