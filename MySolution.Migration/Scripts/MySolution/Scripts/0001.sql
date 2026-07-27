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

    "Role" INT NOT NULL,

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