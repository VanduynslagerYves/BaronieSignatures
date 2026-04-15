using System.CommandLine;

namespace BaronieSignatures;

public class Program
{
    public static int Main(string[] args)
    {
        var userNameOption = new Option<string?>("--userName")
        {
            DefaultValueFactory = _ => string.Empty,
            Description = "The user name to look up in AD"
        };

        var copyToCitrixOption = new Option<bool>("--copyToCitrix")
        {
            DefaultValueFactory = _ => false,
            Description = "Whether to copy the generated signatures to the Citrix profile directory"
        };

        var rootCommand = new RootCommand("BaronieSignatures app")
        {
            userNameOption,
            copyToCitrixOption
        };

        rootCommand.SetAction(parseResult =>
        {
            var samAccountName = parseResult.GetValue(userNameOption);
            var copyToCitrix = parseResult.GetValue(copyToCitrixOption);

            if (string.IsNullOrEmpty(samAccountName))
            {
                var signatureParamsList = SignatureParamsList.All;

                Parallel.ForEach(signatureParamsList, signatureParams =>
                {
                    SignatureUpdater.UpdateSignatures(signatureParams, copyToCitrixProfile: copyToCitrix);
                });

                Console.WriteLine("Signature generation and deployment completed.");
            }
            else
            {
                SignatureUpdater.UpdateSignature(samAccountName, copyToCitrixProfile: copyToCitrix);
            }
            return 0;
        });

        var parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}
