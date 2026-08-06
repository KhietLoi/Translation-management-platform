CREATE SCHEMA IF NOT EXISTS mysolution;
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Users
CREATE TABLE IF NOT EXISTS mysolution."Users"
(
    "Id" UUID NOT NULL DEFAULT gen_random_uuid(),

    "Username" VARCHAR(256) NOT NULL,

    "Email" VARCHAR(256) NOT NULL,

    "PasswordHash" TEXT NOT NULL,
    
    "PasswordVersion" INT NOT NULL DEFAULT 1,
    
    "Status" INT NOT NULL DEFAULT 0,

    "SecurityStamp" VARCHAR(256) NOT NULL DEFAULT gen_random_uuid()::text,
    
    "IsEmailVerified" BOOLEAN NOT NULL DEFAULT FALSE,

    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT "PK_Users"
    PRIMARY KEY ("Id")
);

-- UserProfile:
CREATE TABLE IF NOT EXISTS mysolution."UserProfiles"
(
    "UserId" UUID NOT NULL,

    "FullName" VARCHAR(100),

    "BirthDate" DATE,

    "PhoneNumber" VARCHAR(20),

    "AvatarBlobName" VARCHAR(300),

    "Address" VARCHAR(255),

    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT "PK_UserProfiles"
    PRIMARY KEY ("UserId"),

    CONSTRAINT "FK_UserProfiles_Users_UserId"
    FOREIGN KEY ("UserId")
    REFERENCES mysolution."Users" ("Id")
    ON DELETE CASCADE
);

-- Roles
CREATE TABLE IF NOT EXISTS mysolution."Roles"
(
    "Id" UUID NOT NULL DEFAULT gen_random_uuid(),

    "Name" VARCHAR(200) NOT NULL,

    "Description" VARCHAR(500),

    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT "PK_Roles"
    PRIMARY KEY ("Id")
);

-- Permissions
CREATE TABLE IF NOT EXISTS mysolution."Permissions"
(
    "Id" UUID NOT NULL DEFAULT gen_random_uuid(),

    "Code" VARCHAR(200) NOT NULL,

    "Description" VARCHAR(500),

    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT "PK_Permissions"
    PRIMARY KEY ("Id")
);

-- RefreshTokens
CREATE TABLE IF NOT EXISTS mysolution."RefreshTokens"
(
    "Id" UUID NOT NULL DEFAULT gen_random_uuid(),

    "TokenHash" VARCHAR(64) NOT NULL,

    "ExpiredAt" TIMESTAMPTZ NOT NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,
    
    "RevokedAt" TIMESTAMPTZ NULL,

    "UserId" UUID NOT NULL,
    
    "Jti" VARCHAR(36) NOT NULL,

    CONSTRAINT "PK_RefreshTokens"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_RefreshTokens_Users_UserId"
    FOREIGN KEY ("UserId")
    REFERENCES mysolution."Users"("Id")
    ON DELETE CASCADE
);

-- UserRoles
CREATE TABLE IF NOT EXISTS mysolution."UserRoles"
(
    "UserId" UUID NOT NULL,

    "RoleId" UUID NOT NULL,

    CONSTRAINT "PK_UserRoles"
    PRIMARY KEY ("UserId", "RoleId"),

    CONSTRAINT "FK_UserRoles_Users_UserId"
    FOREIGN KEY ("UserId")
    REFERENCES mysolution."Users"("Id")
    ON DELETE CASCADE,

    CONSTRAINT "FK_UserRoles_Roles_RoleId"
    FOREIGN KEY ("RoleId")
    REFERENCES mysolution."Roles"("Id")
    ON DELETE CASCADE
);

-- RolePermissions
CREATE TABLE IF NOT EXISTS mysolution."RolePermissions"
(
    "RoleId" UUID NOT NULL,

    "PermissionId" UUID NOT NULL,

    CONSTRAINT "PK_RolePermissions"
    PRIMARY KEY ("RoleId", "PermissionId"),

    CONSTRAINT "FK_RolePermissions_Roles_RoleId"
    FOREIGN KEY ("RoleId")
    REFERENCES mysolution."Roles"("Id")
    ON DELETE CASCADE,

    CONSTRAINT "FK_RolePermissions_Permissions_PermissionId"
    FOREIGN KEY ("PermissionId")
    REFERENCES mysolution."Permissions"("Id")
    ON DELETE CASCADE
);
-- Project
CREATE TABLE IF NOT EXISTS mysolution."Projects"
(
    "Id" UUID NOT NULL,

    "Name" VARCHAR(100) NOT NULL,

    "Description" VARCHAR(500),

    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    "UpdatedAt" TIMESTAMPTZ NULL,

    CONSTRAINT "PK_Projects"
    PRIMARY KEY ("Id")
);

-- Language
CREATE TABLE IF NOT EXISTS mysolution."Languages"
(
    "Id" UUID NOT NULL,

    "Code" VARCHAR(30) NOT NULL,

    "Name" VARCHAR(100) NOT NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    "UpdatedAt" TIMESTAMPTZ NULL,

    CONSTRAINT "PK_Languages"
    PRIMARY KEY ("Id")
);

-- ProjectLanguage
CREATE TABLE IF NOT EXISTS mysolution."ProjectLanguages"
(
    "ProjectId" UUID NOT NULL,

    "LanguageId" UUID NOT NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    "UpdatedAt" TIMESTAMPTZ NULL,

    CONSTRAINT "PK_ProjectLanguages"
    PRIMARY KEY ("ProjectId", "LanguageId"),

    CONSTRAINT "FK_ProjectLanguages_Projects_ProjectId"
    FOREIGN KEY ("ProjectId")
    REFERENCES mysolution."Projects" ("Id")
    ON DELETE CASCADE,

    CONSTRAINT "FK_ProjectLanguages_Languages_LanguageId"
    FOREIGN KEY ("LanguageId")
    REFERENCES mysolution."Languages" ("Id")
    ON DELETE CASCADE
);

-- ProjectNamespace
CREATE TABLE IF NOT EXISTS mysolution."ProjectNamespaces"
(
    "Id" UUID NOT NULL,

    "ProjectId" UUID NOT NULL,

    "Name" VARCHAR(100) NOT NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    "UpdatedAt" TIMESTAMPTZ NULL,

    CONSTRAINT "PK_ProjectNamespaces"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_ProjectNamespaces_Projects_ProjectId"
    FOREIGN KEY ("ProjectId")
    REFERENCES mysolution."Projects" ("Id")
    ON DELETE CASCADE
);

-- ProjectMember
CREATE TABLE IF NOT EXISTS mysolution."ProjectMembers"
(
    "ProjectId" UUID NOT NULL,

    "UserId" UUID NOT NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    "UpdatedAt" TIMESTAMPTZ NULL,

    CONSTRAINT "PK_ProjectMembers"
    PRIMARY KEY ("ProjectId", "UserId"),

    CONSTRAINT "FK_ProjectMembers_Projects_ProjectId"
    FOREIGN KEY ("ProjectId")
    REFERENCES mysolution."Projects" ("Id")
    ON DELETE CASCADE,

    CONSTRAINT "FK_ProjectMembers_Users_UserId"
    FOREIGN KEY ("UserId")
    REFERENCES mysolution."Users" ("Id")
    ON DELETE CASCADE
);


-- TranslationKeys
CREATE TABLE IF NOT EXISTS mysolution."TranslationKeys"
(
    "Id" UUID NOT NULL,

    "ProjectId" UUID NOT NULL,

    "NamespaceId" UUID NOT NULL,

    "Key" VARCHAR(200) NOT NULL,

    "Description" VARCHAR(500),

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    "UpdatedAt" TIMESTAMPTZ NULL,

    CONSTRAINT "PK_TranslationKeys"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_TranslationKeys_Projects_ProjectId"
    FOREIGN KEY ("ProjectId")
    REFERENCES mysolution."Projects" ("Id")
    ON DELETE CASCADE,

    CONSTRAINT "FK_TranslationKeys_ProjectNamespaces_NamespaceId"
    FOREIGN KEY ("NamespaceId")
    REFERENCES mysolution."ProjectNamespaces" ("Id")
    ON DELETE CASCADE
    );

-- TranslationValues
CREATE TABLE IF NOT EXISTS mysolution."TranslationValues"
(
    "Id" UUID NOT NULL,

    "TranslationKeyId" UUID NOT NULL,

    "LanguageId" UUID NOT NULL,

    "Value" TEXT,

    "Status" INT NOT NULL DEFAULT 0,

    "TranslatedBy" UUID NULL,
    
    "RejectionReason" VARCHAR(1000) NULL,

    "ReviewedBy" UUID NULL,

    "PublishedBy" UUID NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    "UpdatedAt" TIMESTAMPTZ NULL,

    "TranslatedAt" TIMESTAMPTZ NULL,

    "ReviewedAt" TIMESTAMPTZ NULL,

    "PublishedAt" TIMESTAMPTZ NULL,

    CONSTRAINT "PK_TranslationValues"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_TranslationValues_TranslationKeys_TranslationKeyId"
    FOREIGN KEY ("TranslationKeyId")
    REFERENCES mysolution."TranslationKeys" ("Id")
    ON DELETE CASCADE,

    CONSTRAINT "FK_TranslationValues_Languages_LanguageId"
    FOREIGN KEY ("LanguageId")
    REFERENCES mysolution."Languages" ("Id")
    ON DELETE RESTRICT,

    CONSTRAINT "FK_TranslationValues_Translator"
    FOREIGN KEY ("TranslatedBy")
    REFERENCES mysolution."Users" ("Id")
    ON DELETE SET NULL,

    CONSTRAINT "FK_TranslationValues_Reviewer"
    FOREIGN KEY ("ReviewedBy")
    REFERENCES mysolution."Users" ("Id")
    ON DELETE SET NULL,

    CONSTRAINT "FK_TranslationValues_Publisher"
    FOREIGN KEY ("PublishedBy")
    REFERENCES mysolution."Users" ("Id")
    ON DELETE SET NULL
    );

-- AuditLogs
CREATE TABLE IF NOT EXISTS mysolution."AuditLogs"
(
    "Id" UUID NOT NULL,

    "UserId" UUID NOT NULL,

    "Action" INT NOT NULL,

    "EntityName" VARCHAR(100) NOT NULL,

    "EntityId" UUID NOT NULL,

    "OldValue" TEXT NULL,

    "NewValue" TEXT NULL,
    
    "Reason" VARCHAR(1000) NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    CONSTRAINT "PK_AuditLogs"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_AuditLogs_Users_UserId"
    FOREIGN KEY ("UserId")
    REFERENCES mysolution."Users" ("Id")
    ON DELETE RESTRICT
    );

-- SPRINT 4:
-- Application
CREATE TABLE IF NOT EXISTS mysolution."Applications"
(
    "Id" UUID NOT NULL,

    "ProjectId" UUID NOT NULL,

    "Name" VARCHAR(100) NOT NULL,

    "Description" VARCHAR(1000) NULL,

    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,

    "CreatedAt" TIMESTAMPTZ NOT NULL,
    
    "CreatedBy" UUID NOT NULL,
    
    "UpdatedAt" TIMESTAMPTZ NULL,
    
    "UpdatedBy" UUID NULL,

    CONSTRAINT "PK_Applications"
    PRIMARY KEY ("Id"),
    
    CONSTRAINT "FK_Applications_Projects_ProjectId"
    FOREIGN KEY ("ProjectId")
    REFERENCES mysolution."Projects" ("Id")
    ON DELETE CASCADE
);



CREATE UNIQUE INDEX IF NOT EXISTS "UX_Applications_Name"
    ON mysolution."Applications"
    (
        "Name"
    );

CREATE INDEX IF NOT EXISTS "IX_Applications_Id"
    ON mysolution."Applications" ("Id");

-- API Keys
CREATE TABLE IF NOT EXISTS mysolution."ApiKeys"
(
    "Id" UUID NOT NULL,

    "ApplicationId" UUID NOT NULL,

    "Name" VARCHAR(100) NOT NULL,

    "KeyHash" VARCHAR(500) NOT NULL,

    "KeyPrefix" VARCHAR(50) NOT NULL,

    "ExpiresAt" TIMESTAMPTZ NULL,
    
    "RevokedAt" TIMESTAMPTZ NULL,

    "RevokedBy" UUID NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    "CreatedBy" UUID NOT NULL,

    CONSTRAINT "PK_ApiKeys"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_ApiKeys_Applications_ApplicationId"
    FOREIGN KEY ("ApplicationId")
    REFERENCES mysolution."Applications" ("Id")
    ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_ApiKeys_ApplicationId"
    ON mysolution."ApiKeys" ("ApplicationId");

CREATE INDEX IF NOT EXISTS "IX_ApiKeys_KeyPrefix"
    ON mysolution."ApiKeys" ("KeyPrefix");

CREATE INDEX IF NOT EXISTS "IX_ApiKeys_KeyHash"
    ON mysolution."ApiKeys" ("KeyHash");

-- API Key Permissions
CREATE TABLE IF NOT EXISTS mysolution."ApiKeyPermissions"
(
    "Id" UUID NOT NULL,

    "ApiKeyId" UUID NOT NULL,

    "Permission" INT NOT NULL,

    CONSTRAINT "PK_ApiKeyPermissions"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_ApiKeyPermissions_ApiKeys_ApiKeyId"
    FOREIGN KEY ("ApiKeyId")
    REFERENCES mysolution."ApiKeys" ("Id")
    ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_ApiKeyPermissions_ApiKeyId"
    ON mysolution."ApiKeyPermissions" ("ApiKeyId");

-- Duplicate permission
CREATE UNIQUE INDEX IF NOT EXISTS "UX_ApiKeyPermissions_ApiKeyId_Permission"
    ON mysolution."ApiKeyPermissions"
(
    "ApiKeyId",
    "Permission"
);
-- API Key Usage Logs
CREATE TABLE IF NOT EXISTS mysolution."ApiKeyUsageLogs"
(
    "Id" UUID NOT NULL,

    "ApiKeyId" UUID NOT NULL,
    
    "ApplicationId" UUID NOT NULL,

    "Endpoint" VARCHAR(500) NOT NULL,

    "Method" VARCHAR(20) NOT NULL,

    "StatusCode" INT NOT NULL,
    
    "DurationMs" INT NOT NULL,

    "IpAddress" VARCHAR(100) NULL,
    
    "UserAgent" TEXT NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    CONSTRAINT "PK_ApiKeyUsageLogs"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_ApiKeyUsageLogs_ApiKeys_ApiKeyId"
    FOREIGN KEY ("ApiKeyId")
    REFERENCES mysolution."ApiKeys" ("Id")
    ON DELETE CASCADE,
    
    CONSTRAINT "FK_ApiKeyUsageLogs_Applications_ApplicationId"
    FOREIGN KEY ("ApplicationId")
    REFERENCES mysolution."Applications" ("Id")
    ON DELETE CASCADE
);


CREATE INDEX IF NOT EXISTS "IX_ApiKeyUsageLogs_ApiKeyId"
    ON mysolution."ApiKeyUsageLogs" ("ApiKeyId");

CREATE INDEX IF NOT EXISTS "IX_ApiKeyUsageLogs_CreatedAt"
    ON mysolution."ApiKeyUsageLogs" ("CreatedAt");

CREATE INDEX IF NOT EXISTS "IX_ApiKeyUsageLogs_ApiKeyId_CreatedAt"
    ON mysolution."ApiKeyUsageLogs"
    (
    "ApiKeyId",
    "CreatedAt" DESC
    );
CREATE INDEX IF NOT EXISTS "IX_ApiKeyUsageLogs_ApplicationId"
    ON mysolution."ApiKeyUsageLogs" ("ApplicationId");

-- TranslationKeys Indexes
CREATE UNIQUE INDEX IF NOT EXISTS "IX_TranslationKeys_ProjectId_NamespaceId_Key"
    ON mysolution."TranslationKeys"
    (
    "ProjectId",
    "NamespaceId",
    "Key"
    );

CREATE INDEX IF NOT EXISTS "IX_TranslationKeys_ProjectId"
    ON mysolution."TranslationKeys" ("ProjectId");

CREATE INDEX IF NOT EXISTS "IX_TranslationKeys_NamespaceId"
    ON mysolution."TranslationKeys" ("NamespaceId");

-- TranslationValues Indexes
CREATE UNIQUE INDEX IF NOT EXISTS "IX_TranslationValues_TranslationKeyId_LanguageId"
    ON mysolution."TranslationValues"
    (
    "TranslationKeyId",
    "LanguageId"
    );

CREATE INDEX IF NOT EXISTS "IX_TranslationValues_LanguageId"
    ON mysolution."TranslationValues" ("LanguageId");

CREATE INDEX IF NOT EXISTS "IX_TranslationValues_Status"
    ON mysolution."TranslationValues" ("Status");

CREATE INDEX IF NOT EXISTS "IX_TranslationValues_TranslatedBy"
    ON mysolution."TranslationValues" ("TranslatedBy");

CREATE INDEX IF NOT EXISTS "IX_TranslationValues_ReviewedBy"
    ON mysolution."TranslationValues" ("ReviewedBy");

CREATE INDEX IF NOT EXISTS "IX_TranslationValues_PublishedBy"
    ON mysolution."TranslationValues" ("PublishedBy");

-- AuditLogs Indexes
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_UserId"
    ON mysolution."AuditLogs" ("UserId");

CREATE INDEX IF NOT EXISTS "IX_AuditLogs_CreatedAt"
    ON mysolution."AuditLogs" ("CreatedAt");

CREATE INDEX IF NOT EXISTS "IX_AuditLogs_EntityName_EntityId"
    ON mysolution."AuditLogs"
    (
    "EntityName",
    "EntityId"
    );

CREATE INDEX IF NOT EXISTS "IX_AuditLogs_Action"
    ON mysolution."AuditLogs" ("Action");



-- Indexes
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Username"
    ON mysolution."Users" ("Username");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email"
    ON mysolution."Users" ("Email");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Roles_Name"
    ON mysolution."Roles" ("Name");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Permissions_Code"
    ON mysolution."Permissions" ("Code");

CREATE INDEX IF NOT EXISTS "IX_UserRoles_RoleId"
    ON mysolution."UserRoles" ("RoleId");

CREATE INDEX IF NOT EXISTS "IX_RolePermissions_PermissionId"
    ON mysolution."RolePermissions" ("PermissionId");

CREATE INDEX IF NOT EXISTS "IX_RefreshTokens_UserId"
    ON mysolution."RefreshTokens" ("UserId");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_RefreshTokens_Jti"
    ON mysolution."RefreshTokens" ("Jti");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_RefreshTokens_TokenHash"
    ON mysolution."RefreshTokens" ("TokenHash");

CREATE INDEX IF NOT EXISTS "IX_RefreshTokens_ExpiredAt"
    ON mysolution."RefreshTokens" ("ExpiredAt");

CREATE INDEX IF NOT EXISTS "IX_RefreshTokens_RevokedAt"
    ON mysolution."RefreshTokens" ("RevokedAt");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_UserProfiles_PhoneNumber"
    ON mysolution."UserProfiles" ("PhoneNumber")
    WHERE "PhoneNumber" IS NOT NULL;

CREATE INDEX IF NOT EXISTS "IX_Projects_Name"
    ON mysolution."Projects" ("Name");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Languages_Code"
    ON mysolution."Languages" ("Code");
CREATE INDEX IF NOT EXISTS "IX_ProjectLanguages_LanguageId"
    ON mysolution."ProjectLanguages" ("LanguageId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_ProjectNamespaces_ProjectId_Name"
    ON mysolution."ProjectNamespaces"
    (
        "ProjectId",
        "Name"
    );
CREATE INDEX IF NOT EXISTS "IX_ProjectMembers_UserId"
    ON mysolution."ProjectMembers" ("UserId");