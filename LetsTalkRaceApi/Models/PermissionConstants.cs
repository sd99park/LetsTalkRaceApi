namespace LetsTalkRaceApi.Models;

public static class PermissionConstants
{
    // Levels of permissions, if a lower role has perms, the upper roles will have perms
    public const string SUPER_ADMIN = "SUPER_ADMIN";
    public const string ADMIN = "ADMIN,SUPER_ADMIN";
    public const string USER = "USER,ADMIN,SUPER_ADMIN";
}