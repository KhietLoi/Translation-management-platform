namespace MySolution.Application.Constants;

public static class PermissionConstants
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

    public static class Project
    {
        public const string View = "PROJECT_VIEW";
        public const string Create = "PROJECT_CREATE";
        public const string Update = "PROJECT_UPDATE";
        public const string Delete = "PROJECT_DELETE";
    }

    public static class Language
    {
        public const string View = "LANGUAGE_VIEW";
        public const string Create = "LANGUAGE_CREATE";
        public const string Update = "LANGUAGE_UPDATE";
        public const string Delete = "LANGUAGE_DELETE";
    }

    public static class Translation
    {
        public const string View = "TRANSLATION_VIEW";
        public const string Create = "TRANSLATION_CREATE";
        public const string Update = "TRANSLATION_UPDATE";
        public const string Delete = "TRANSLATION_DELETE";
        public const string Review = "TRANSLATION_REVIEW";
        public const string Publish = "TRANSLATION_PUBLISH";
    }

    public static class ApiKey
    {
        public const string View = "APIKEY_VIEW";
        public const string Create = "APIKEY_CREATE";
        public const string Update = "APIKEY_UPDATE";
        public const string Delete = "APIKEY_DELETE";
    }

    public static class AuditLog
    {
        public const string View = "AUDITLOG_VIEW";
    }
}