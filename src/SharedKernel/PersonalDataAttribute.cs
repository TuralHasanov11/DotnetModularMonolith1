using Microsoft.Extensions.Compliance.Classification;

namespace SharedKernel;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PersonalDataAttribute : DataClassificationAttribute
{
    public PersonalDataAttribute() : base(ApplicationLoggingTaxonomy.PersonalData)
    {
    }
}