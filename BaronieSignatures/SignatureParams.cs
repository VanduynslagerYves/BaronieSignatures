namespace BaronieSignatures;

public record SignatureParams
{
    public required string GroupName { get; init; }
    public required string CompanyName { get; init; }
    public required string SigSource { get; init; }
    public required string BaseLocal { get; init; }
    public required string DefaultPhone { get; init; }
}