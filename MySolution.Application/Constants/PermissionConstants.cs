namespace MySolution.Application.Constants;

public class PermissionConstants
{
    public static class User
    {
        public const string View = "USER_VIEW";
        public const string Create = "USER_CREATE";
        public const string Update = "USER_UPDATE";
        public const string Delete = "USER_DELETE";
    }

    public static class Role
    {
        public const string View = "ROLE_VIEW";
        public const string Create = "ROLE_CREATE";
        public const string Update = "ROLE_UPDATE";
        public const string Delete = "ROLE_DELETE";
    }

    public static class Permission
    {
        public const string View = "PERMISSION_VIEW";
        public const string Create = "PERMISSION_CREATE";
        public const string Update = "PERMISSION_UPDATE";
        public const string Delete = "PERMISSION_DELETE";
    }
}