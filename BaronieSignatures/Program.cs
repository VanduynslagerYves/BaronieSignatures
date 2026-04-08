namespace BaronieSignatures;

public class Program
{
    private static void Main()
    {
        // Call the signature update logic
        // Pass true to copy to Citrix profile, false otherwise
        var bruggeSignatureParams = new SignatureParams
        {
            GroupName = "SG_Brugge_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Brugge",
            SigSource = @"C:\Signatures\Templates\Brugge",
            BaseLocal = @"C:\Signatures\Output\Brugge",
            DefaultPhone = "+32 50 45 00 20",
        };

        var lokerenSignatureParams = new SignatureParams
        {
            GroupName = "SG_Lokeren_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Lokeren",
            SigSource = @"C:\Signatures\Templates\Lokeren",
            BaseLocal = @"C:\Signatures\Output\Lokeren",
            DefaultPhone = "+32 93 26 82 70",
        };

        var veurneSignatureParams = new SignatureParams
        {
            GroupName = "SG_Veurne_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Veurne",
            SigSource = @"C:\Signatures\Templates\Veurne",
            BaseLocal = @"C:\Signatures\Output\Veurne",
            DefaultPhone = "+32 58 31 01 50",
        };

        var alproseSignatureParams = new SignatureParams
        {
            GroupName = "SG_CASLANO_DESKTOP_OUTLOOK_SIGNATURE_ALPROSE",
            CompanyName = "Alprose",
            SigSource = @"C:\Signatures\Templates\Alprose",
            BaseLocal = @"C:\Signatures\Output\Alprose",
            DefaultPhone = "+41 91 611 88 88",
        };

        var signatureParamsList = new List<SignatureParams>
        {
            bruggeSignatureParams,
            lokerenSignatureParams,
            veurneSignatureParams,
            alproseSignatureParams,
        };

        Console.WriteLine("Starting signature generation and deployment...");
        foreach(var signatureParams in signatureParamsList)
        {
            SignatureUpdater.UpdateSignatures(signatureParams, copyToCitrixProfile: false);
        }
        Console.WriteLine("Signature generation and deployment completed.");

        /*
         * Norderstedt
         * Switzerland ?
         * Eupen
         * Saalfeld
         * Berlin
         */
    }
}