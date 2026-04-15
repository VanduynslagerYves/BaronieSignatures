namespace BaronieSignatures;

public class Program
{
    private static void Main(string[] args)
    {
        string samAccountName = args.Length > 0 ? args[0] : string.Empty;
        var copyToCitrix = false;

        if (string.IsNullOrEmpty(samAccountName))
        {
            var signatureParamsList = SignatureParamsList.All;

            Parallel.ForEach(signatureParamsList, signatureParams =>
            {
                SignatureUpdater.UpdateSignatures(signatureParams, copyToCitrixProfile: copyToCitrix);
            });
            Console.WriteLine("Signature generation and deployment completed.");
        }
        else // Process a single user
        {
            SignatureUpdater.UpdateSignature(samAccountName, copyToCitrixProfile: copyToCitrix);
        }
    }
}
