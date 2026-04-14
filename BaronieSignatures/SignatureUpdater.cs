using System.DirectoryServices.AccountManagement;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.AccessControl;
using System.Text;

namespace BaronieSignatures;

public static class SignatureUpdater
{
    private static readonly Dictionary<string, string> Templates = new()
    {
        { "docx", "{0}.docx" },
        { "txt", "{0}.txt" },
        { "rtf", "{0}.rtf" },
        { "htm", "{0}.htm" }
    };

    private static readonly Dictionary<string, string> TemplatesMobileIncluded = new()
    {
        { "docx", "{0} - Mobile Included.docx" },
        { "txt", "{0} - Mobile Included.txt" },
        { "rtf", "{0} - Mobile Included.rtf" },
        { "htm", "{0} - Mobile Included.htm" }
    };

    public static void UpdateSignatures(SignatureParams options, bool copyToCitrixProfile = false)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = Encoding.GetEncoding(1252); // Western European (Windows)

        // Configuration
        string groupName = options.GroupName;
        string companyName = options.CompanyName;
        string sigSource = options.SigSource;
        string baseLocal = options.BaseLocal;
        string defaultPhone = options.DefaultPhone;

        Console.WriteLine($"Starting signature generation for {options.CompanyName}...");

        // Get AD group members
        Console.WriteLine($"Retrieving members of group {groupName}...");
        using (var ctx = new PrincipalContext(ContextType.Domain))
        using (var group = GroupPrincipal.FindByIdentity(ctx, groupName))
        {
            if (group == null)
            {
                Console.WriteLine($"Group {groupName} not found.");
                return;
            }
            foreach (var principal in group.GetMembers(true))
            {
                if (principal is not UserPrincipal user) continue;
                if (string.IsNullOrEmpty(user.EmailAddress)) continue; // Skip users without email
                var userEx = UserPrincipalEx.FindByIdentity(ctx, IdentityType.SamAccountName, user.SamAccountName); // Use UserPrincipalEx to get mobile

                var userName = user.SamAccountName.ToLower();
                Console.WriteLine($"Processing user: {userName}");

                string fullName = $"{user.GivenName} {user.Surname}";
                string title = userEx?.Title ?? string.Empty;
                string phone = string.IsNullOrEmpty(user.VoiceTelephoneNumber) ? defaultPhone : user.VoiceTelephoneNumber;
                string mobile = userEx?.Mobile ?? string.Empty;

                string localUserPath = Path.Combine(baseLocal, userName);
                Directory.CreateDirectory(localUserPath);

                bool hasMobile = !string.IsNullOrEmpty(mobile);
                CopySignatureFiles(hasMobile, sigSource, localUserPath, companyName);

                var replacements = new Dictionary<string, string>
                {
                    { "FirstLastName", fullName },
                    { "Title", title },
                    { "telephonenr", phone },
                    { "mobilenr", mobile }
                };

                var templateDict = hasMobile ? TemplatesMobileIncluded : Templates;
                foreach (var ext in templateDict.Keys)
                {
                    string templateFileName = string.Format(templateDict[ext], companyName);
                    string templateFile = Path.Combine(sigSource, templateFileName);
                    string localFile = Path.Combine(localUserPath, string.Format(Templates[ext], companyName));
                    if (File.Exists(templateFile))
                    {
                        File.Copy(templateFile, localFile, true);
                        if (ext == "docx")
                        {
                            SetDocxPlaceholders(localFile, replacements);
                        }
                        else
                        {
                            string content = File.ReadAllText(localFile, encoding);
                            foreach (var kvp in replacements)
                            {
                                content = content.Replace(kvp.Key, kvp.Value);
                            }
                            File.WriteAllText(localFile, content, encoding);
                        }
                    }
                }

                // Set NTFS permissions
                try
                {
                    var dirInfo = new DirectoryInfo(localUserPath);
                    var dirSecurity = dirInfo.GetAccessControl();
                    var rule = new FileSystemAccessRule(userName, FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None, AccessControlType.Allow);
                    dirSecurity.AddAccessRule(rule);
                    dirInfo.SetAccessControl(dirSecurity);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to set permissions for {localUserPath}: {ex.Message}");
                }

                //Optional: copy to Citrix profile
                if (copyToCitrixProfile)
                {
                    string citrixPath = $@"\\baroniegroup.com\profiles\CITRIX PROFILES\{userName}\AppData\Microsoft\Signatures";
                    Directory.CreateDirectory(citrixPath);
                    CopyDirectory(localUserPath, citrixPath);
                }
            }
        }
        Console.WriteLine($"Signature generation for {options.CompanyName} completed.");
        Console.WriteLine();
    }

    public static void CopySignatureFiles(bool hasMobile, string sigSource, string localUserPath, string companyName)
    {
        string sigTargetHtmFilesToCopy = hasMobile
    ? Path.Combine(localUserPath, $"{companyName} - Mobile Included_files")
    : Path.Combine(localUserPath, $"{companyName}_files");
        Directory.CreateDirectory(sigTargetHtmFilesToCopy);

        string sigSourceHtmFiles = hasMobile
            ? Path.Combine(sigSource, $"{companyName} - Mobile Included_files")
            : Path.Combine(sigSource, $"{companyName}_files");

        if (Directory.Exists(sigSourceHtmFiles))
        {
            CopyDirectory(sigSourceHtmFiles, sigTargetHtmFilesToCopy);
        }
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
            if (text.Text != null)
            {
                foreach (var kvp in replacements)
                {
                    if (text.Text.Contains(kvp.Key))
                    {
                        text.Text = text.Text.Replace(kvp.Key, kvp.Value);
                    }
                }
            }
        }
        wordDoc.MainDocumentPart.Document.Save();
    }
}
