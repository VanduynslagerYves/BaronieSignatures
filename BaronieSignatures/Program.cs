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
            SigSource = @"C:\Signatures\_Scripts\Signature Files\Brugge",
            BaseLocal = @"C:\Signatures\Brugge",
            DefaultPhone = "+32 50 45 00 20",
        };

        var lokerenSignatureParams = new SignatureParams
        {
            GroupName = "SG_Lokeren_DESKTOP_OUTLOOK_SIGNATURE_DEFAULT",
            CompanyName = "Baronie Lokeren",
            SigSource = @"C:\Signatures\_Scripts\Signature Files\Lokeren",
            BaseLocal = @"C:\Signatures\Lokeren",
            DefaultPhone = "+32 93 26 82 70",
        };

        var signatureParamsList = new List<SignatureParams>
        {
            //bruggeSignatureParams,
            lokerenSignatureParams,
        };

        Console.WriteLine("Starting signature generation and deployment...");
        foreach(var signatureParams in signatureParamsList)
        {
            SignatureUpdater.UpdateSignatures(signatureParams, copyToCitrixProfile: false);
        }
        Console.WriteLine("Signature generation and deployment completed.");

        /*
         * Norderstedt
         * Switzerland
         * Eupen
         * Saalfeld
         * Berlin
         * Veurne
         * Alprose
         * 
         */
    }
}