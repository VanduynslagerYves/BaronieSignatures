using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.DirectoryServices.AccountManagement;
using System.Security.AccessControl;
using System.Text;

namespace BaronieSignatures;

public static class SignatureUpdater
{
    private static readonly Dictionary<string, string> _templates = new()
    {
        { "docx", "{0}.docx" },
        { "txt", "{0}.txt" },
        { "rtf", "{0}.rtf" },
        { "htm", "{0}.htm" }
    };

    private static readonly Dictionary<string, string> _templatesMobileIncluded = new()
    {
        { "docx", "{0} - Mobile Included.docx" },
        { "txt", "{0} - Mobile Included.txt" },
        { "rtf", "{0} - Mobile Included.rtf" },
        { "htm", "{0} - Mobile Included.htm" }
    };

    public static void UpdateSignature(string samAccountName, bool copyToCitrixProfile = false)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = Encoding.GetEncoding(1252); // Western European (Windows)

        samAccountName = samAccountName.ToLower();

        using var ctx = new PrincipalContext(ContextType.Domain);
        var userEx = UserPrincipalEx.FindByIdentity(ctx, IdentityType.SamAccountName, samAccountName);
        if (userEx == null) return;
        if (string.IsNullOrEmpty(userEx.EmailAddress)) return;

        var officeLocation = userEx.Office;
        if (string.IsNullOrEmpty(officeLocation))
        {
            Console.WriteLine($"User {samAccountName} does not have an office attribute.");
            return;
        }

        var companyName = $"Baronie {officeLocation}"; //TODO: this will not work for Alprose, should not contain Baronie and officeLocation will be Caslano in AD
        var sigInput = Path.Combine(AppContext.BaseDirectory, "Templates", officeLocation);
        var sigOutput = Path.Combine(AppContext.BaseDirectory, "Output", officeLocation);
        var defaultPhone = SignatureParamsList.DefaultPhones[officeLocation];

        string fullName = $"{userEx.GivenName} {userEx.Surname}";
        string title = userEx.Title ?? string.Empty;
        string phone = string.IsNullOrEmpty(userEx.VoiceTelephoneNumber) ? defaultPhone : userEx.VoiceTelephoneNumber;
        string mobile = userEx.Mobile ?? string.Empty;
        var email = userEx.EmailAddress;

        string outputUserPath = Path.Combine(sigOutput, samAccountName);
        Directory.CreateDirectory(outputUserPath);

        bool hasMobile = !string.IsNullOrEmpty(mobile);
        CopySignatureHtmFiles(hasMobile, sigInput, outputUserPath, companyName, email);

        var replacements = new Dictionary<string, string>
        {
            { "FirstLastName", fullName },
            { "Title", title },
            { "telephonenr", phone },
            { "mobilenr", mobile }
        };

        ProcessTemplates(hasMobile, sigInput, outputUserPath, companyName, email, replacements, encoding);
        SetDirectoryPermissions(outputUserPath, samAccountName);

        if (copyToCitrixProfile)
        {
            CopyToCitrixProfile(outputUserPath, samAccountName);
        }
    }

    public static void UpdateSignatures(SignatureParams options, bool copyToCitrixProfileEnabled = false)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = Encoding.GetEncoding(1252); // Western European (Windows)

        Console.WriteLine($"Starting signature generation for {options.Company}...");

        // Get AD group members
        Console.WriteLine($"Retrieving members of group {options.GroupName}...");
        using (var ctx = new PrincipalContext(ContextType.Domain))
        using (var group = GroupPrincipal.FindByIdentity(ctx, options.GroupName))
        {
            if (group == null)
            {
                Console.WriteLine($"Group {options.GroupName} not found.");
                return;
            }

            foreach (var principal in group.GetMembers(true))
            {
                if (principal is not UserPrincipal user) continue;
                if (string.IsNullOrEmpty(user.EmailAddress)) continue; // Skip users without email
                var email = user.EmailAddress;
                var userEx = UserPrincipalEx.FindByIdentity(ctx, IdentityType.SamAccountName, user.SamAccountName); // Use UserPrincipalEx to get mobile
                if (userEx == null) continue;

                var userName = userEx.SamAccountName.ToLower();
                Console.WriteLine($"Processing user: {userName}");

                string fullName = $"{userEx.GivenName} {userEx.Surname}";
                string title = userEx.Title ?? string.Empty;
                string phone = string.IsNullOrEmpty(user.VoiceTelephoneNumber) ? options.DefaultPhone : user.VoiceTelephoneNumber;
                string mobile = userEx.Mobile ?? string.Empty;

                string sigTargetPath = Path.Combine(options.BaseLocal, userName);
                Directory.CreateDirectory(sigTargetPath);

                bool hasMobile = !string.IsNullOrEmpty(mobile);
                CopySignatureHtmFiles(hasMobile, options.SigSource, sigTargetPath, options.Company, email);

                var replacements = new Dictionary<string, string>
                {
                    { "FirstLastName", fullName },
                    { "Title", title },
                    { "telephonenr", phone },
                    { "mobilenr", mobile }
                };

                ProcessTemplates(hasMobile, options.SigSource, sigTargetPath, options.Company, email, replacements, encoding);
                SetDirectoryPermissions(sigTargetPath, userName);
                if (copyToCitrixProfileEnabled) CopyToCitrixProfile(sigTargetPath, userName);
            }
        }

        Console.WriteLine($"Signature generation for {options.Company} completed.");
        Console.WriteLine();
    }

    private static void ProcessTemplates(bool hasMobile, string sigSourcePath, string sigTargetPath, string companyName, string email, Dictionary<string, string> replacements, Encoding encoding)
    {
        var templateDict = hasMobile ? _templatesMobileIncluded : _templates;
        foreach (var ext in templateDict.Keys)
        {
            string templateFileName = string.Format(templateDict[ext], companyName);
            string sourceFile = Path.Combine(sigSourcePath, templateFileName);
            string targetFile = Path.Combine(sigTargetPath, $"{companyName} ({email}).{ext}");

            if (File.Exists(sourceFile))
            {
                File.Copy(sourceFile, targetFile, true);
                if (ext == "docx")
                {
                    SetDocxPlaceholders(targetFile, replacements);
                }
                else
                {
                    string content = File.ReadAllText(targetFile, encoding);
                    foreach (var kvp in replacements)
                    {
                        content = content.Replace(kvp.Key, kvp.Value);
                    }

                    // Fix HTML file references to the renamed folder
                    if (ext == "htm")
                    {
                        string oldFolderName = hasMobile
                            ? $"{companyName} - Mobile Included_files"
                            : $"{companyName}_files";
                        string newFolderName = hasMobile
                            ? $"{companyName} ({email}) - Mobile Included_files"
                            : $"{companyName} ({email})_files";

                        // Replace both regular and URL-encoded folder names
                        content = content.Replace(oldFolderName, newFolderName);
                        content = content.Replace(Uri.EscapeDataString(oldFolderName), Uri.EscapeDataString(newFolderName));
                    }

                    File.WriteAllText(targetFile, content, encoding);
                }
            }
        }
    }

    private static void SetDirectoryPermissions(string directoryPath, string samAccountName)
    {
        try
        {
            var dirInfo = new DirectoryInfo(directoryPath);
            var dirSecurity = dirInfo.GetAccessControl();
            var rule = new FileSystemAccessRule(samAccountName, FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None, AccessControlType.Allow);
            dirSecurity.AddAccessRule(rule);
            dirInfo.SetAccessControl(dirSecurity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to set permissions for {directoryPath}: {ex.Message}");
        }
    }

    private static void CopyToCitrixProfile(string sourceLocalUserPath, string samAccountName)
    {
        string citrixTargetPath = $@"\\baroniegroup.com\profiles\CITRIX PROFILES\{samAccountName}\AppData\Microsoft\Signatures";
        Directory.CreateDirectory(citrixTargetPath);
        CopyDirectory(sourceLocalUserPath, citrixTargetPath);
    }

    public static void CopySignatureHtmFiles(bool hasMobile, string sigSource, string sigTargetPath, string companyName, string email)
    {
        string sigTargetHtmFilesToCopy = hasMobile
            ? Path.Combine(sigTargetPath, $"{companyName} ({email}) - Mobile Included_files")
            : Path.Combine(sigTargetPath, $"{companyName} ({email})_files");

        Directory.CreateDirectory(sigTargetHtmFilesToCopy);

        string sigSourceHtmFiles = hasMobile
            ? Path.Combine(sigSource, $"{companyName} - Mobile Included_files")
            : Path.Combine(sigSource, $"{companyName}_files");

        if (Directory.Exists(sigSourceHtmFiles)) CopyDirectory(sigSourceHtmFiles, sigTargetHtmFilesToCopy);
    }

    private static void CopyDirectory(string sourceDir, string destDir)
    {
        foreach (var dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));
        }

        foreach (var newPath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourceDir, destDir), true);
        }
    }

    private static void SetDocxPlaceholders(string docxPath, Dictionary<string, string> replacements)
    {
        // Use OpenXML SDK to replace placeholders in all text elements
        using var wordDoc = WordprocessingDocument.Open(docxPath, true);
        if (wordDoc.MainDocumentPart?.Document == null) return;
        var body = wordDoc.MainDocumentPart.Document.Body;
        if (body == null) return;

        foreach (var text in body.Descendants<Text>())
        {
            if (text.Text == null) continue;
            foreach (var kvp in replacements)
            {
                if (!text.Text.Contains(kvp.Key)) continue;
                text.Text = text.Text.Replace(kvp.Key, kvp.Value);
            }
        }

        wordDoc.MainDocumentPart.Document.Save();
    }
}
