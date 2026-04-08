using System.DirectoryServices.AccountManagement;

namespace BaronieSignatures;

[DirectoryObjectClass("user")]
[DirectoryRdnPrefix("CN")]
public class UserPrincipalEx(PrincipalContext context) : UserPrincipal(context)
{
    [DirectoryProperty("mobile")]
    public string Mobile
    {
        get
        {
            var result = ExtensionGet("mobile");
            return result.Length > 0 && result[0] != null ? (result[0].ToString() ?? string.Empty) : string.Empty;
        }
        set { ExtensionSet("mobile", value); }
    }

    public string Title
    {
        get
        {
            var result = ExtensionGet("Title");
            return result.Length > 0 && result[0] != null ? (result[0].ToString() ?? string.Empty) : string.Empty;
        }
    }

    public static new UserPrincipalEx FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
    {
        return (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityType, identityValue);
    }
}