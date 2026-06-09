namespace BaronieSignatures;

public static class SignatureParamsList
{
    public static readonly Dictionary<string, string> DefaultPhones = new()
    {
        { "Brugge", "+32 50 45 00 20" },
        { "Lokeren", "+32 93 26 82 70" },
        { "Veurne", "+32 58 31 01 50" },
        { "Alprose", "+41 91 611 88 88" },
        { "Switzerland", "+41 91 611 88 88" },
        { "Norderstedt", "+49 40 52 60 20" },
        { "Eupen", "+32 50 45 00 30" },
        { "Saalfeld", "+49 40 526020" },
        { "Berlin", "+49 40 526020" }
    };

    // Company should be the same as the company name in the company folder template files
    public static readonly List<SignatureParams> All =
    [
        new SignatureParams
        {
            GroupName = "SG_Brugge_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            Company = "Baronie Brugge",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Brugge"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Brugge"),
            DefaultPhone = "+32 50 45 00 20",
        },
        new SignatureParams
        {
            GroupName = "SG_Lokeren_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            Company = "Baronie Lokeren",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Lokeren"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Lokeren"),
            DefaultPhone = "+32 93 26 82 70",
        },
        new SignatureParams
        {
            GroupName = "SG_Veurne_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            Company = "Baronie Veurne",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Veurne"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Veurne"),
            DefaultPhone = "+32 58 31 01 50",
        },
        new SignatureParams
        {
            GroupName = "SG_CASLANO_DESKTOP_OUTLOOK_SIGNATURE_ALPROSE",
            Company = "Alprose",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Alprose"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Alprose"),
            DefaultPhone = "+41 91 611 88 88",
        },
        new SignatureParams
        {
            GroupName = "SG_CASLANO_DESKTOP_OUTLOOK_SIGNATURE_BARONIE_SWITZERLAND",
            Company = "Baronie Switzerland",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Switzerland"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Switzerland"),
            DefaultPhone = "+41 91 611 88 88",
        },
        new SignatureParams
        {
            GroupName = "SG_Norderstedt_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            Company = "Baronie Norderstedt",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Norderstedt"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Norderstedt"),
            DefaultPhone = "+49 40 52 60 20"
        },
        new SignatureParams
        {
            GroupName = "SG_Eupen_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            Company = "Baronie Eupen",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Eupen"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Eupen"),
            DefaultPhone = "+32 50 45 00 30"
        },
        new SignatureParams
        {
            GroupName = "SG_Saalfeld_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            Company = "Baronie Saalfeld",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Saalfeld"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Saalfeld"),
            DefaultPhone = "+49 40 526020"
        },
        new SignatureParams
        {
            GroupName = "SG_Berlin_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            Company = "Baronie Berlin",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Berlin"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Berlin"),
            DefaultPhone = "+49 40 526020"
        },
    ];
}

public record SignatureParams
{
    public required string GroupName { get; init; }
    public required string Company { get; init; }
    public required string SigSource { get; init; }
    public required string BaseLocal { get; init; }
    public required string DefaultPhone { get; init; }
}
