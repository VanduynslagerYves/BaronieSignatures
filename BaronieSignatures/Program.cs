namespace BaronieSignatures;

public class Program
{
    private static void Main()
    {
        var bruggeSignatureParams = new SignatureParams
        {
            GroupName = "SG_Brugge_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Brugge",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Brugge"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Brugge"),
            DefaultPhone = "+32 50 45 00 20",
        };

        var lokerenSignatureParams = new SignatureParams
        {
            GroupName = "SG_Lokeren_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Lokeren",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Lokeren"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Lokeren"),
            DefaultPhone = "+32 93 26 82 70",
        };

        var veurneSignatureParams = new SignatureParams
        {
            GroupName = "SG_Veurne_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Veurne",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Veurne"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Veurne"),
            DefaultPhone = "+32 58 31 01 50",
        };

        var alproseSignatureParams = new SignatureParams
        {
            GroupName = "SG_CASLANO_DESKTOP_OUTLOOK_SIGNATURE_ALPROSE",
            CompanyName = "Alprose",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Alprose"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Alprose"),
            DefaultPhone = "+41 91 611 88 88",
        };

        var switzerlandSignatureParams = new SignatureParams
        {
            GroupName = "SG_CASLANO_DESKTOP_OUTLOOK_SIGNATURE_BARONIE_SWITZERLAND",
            CompanyName = "Baronie Switzerland",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Switzerland"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Switzerland"),
            DefaultPhone = "+41 91 611 88 88",
        };

        var norderstedtSignatureParams = new SignatureParams
        {
            GroupName = "SG_Norderstedt_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Norderstedt",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Norderstedt"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Norderstedt"),
            DefaultPhone = "+49 40 52 60 20"
        };

        var eupenSignatureParams = new SignatureParams
        {
            GroupName = "SG_Eupen_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Eupen",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Eupen"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Eupen"),
            DefaultPhone = "+32 50 45 00 30"
        };

        var saalfeldSignatureParams = new SignatureParams
        {
            GroupName = "SG_Saalfeld_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Saalfeld",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Saalfeld"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Saalfeld"),
            DefaultPhone = "+49 40 526020"
        };

        var berlinSignatureParams = new SignatureParams
        {
            GroupName = "SG_Berlin_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Berlin",
            SigSource = Path.Combine(AppContext.BaseDirectory, "Templates", "Berlin"),
            BaseLocal = Path.Combine(AppContext.BaseDirectory, "Output", "Berlin"),
            DefaultPhone = "+49 40 526020"
        };

        var signatureParamsList = new List<SignatureParams>
        {
            bruggeSignatureParams,
            lokerenSignatureParams,
            veurneSignatureParams,
            alproseSignatureParams,
            norderstedtSignatureParams,
            eupenSignatureParams,
            switzerlandSignatureParams,
            saalfeldSignatureParams,
            berlinSignatureParams,
        };

        Console.WriteLine("Starting signature generation and deployment...");
        foreach(var signatureParams in signatureParamsList)
        {
            SignatureUpdater.UpdateSignatures(signatureParams, copyToCitrixProfile: false);
        }
        Console.WriteLine("Signature generation and deployment completed.");
    }
}