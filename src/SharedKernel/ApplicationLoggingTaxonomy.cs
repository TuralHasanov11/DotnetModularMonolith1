using Microsoft.Extensions.Compliance.Classification;

namespace SharedKernel;

public static class ApplicationLoggingTaxonomy
{
    public static string TaxonomyName => typeof(ApplicationLoggingTaxonomy).FullName!;

    public static DataClassification PersonalData => new(TaxonomyName, nameof(PersonalData));

    public static DataClassification SensitiveData => new(TaxonomyName, nameof(SensitiveData));
}