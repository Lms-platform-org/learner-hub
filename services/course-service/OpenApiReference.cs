using Microsoft.OpenApi;

internal class OpenApiReference
{
    public ReferenceType Type { get; set; }
    public string Id { get; set; }

    public static implicit operator OpenApiReferenceWithDescription(OpenApiReference v)
    {
        throw new NotImplementedException();
    }
}