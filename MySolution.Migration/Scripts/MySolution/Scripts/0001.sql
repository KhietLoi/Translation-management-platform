CREATE SCHEMA IF NOT EXISTS mysolution;
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Users
CREATE TABLE IF NOT EXISTS mysolution."Users"
(
    "Id" UUID NOT NULL DEFAULT gen_random_uuid(),

    "Username" VARCHAR(256) NOT NULL,

    "Email" VARCHAR(256) NOT NULL,

    "PasswordHash" TEXT NOT NULL,

    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    
    "IsEmailVerified" BOOLEAN NOT NULL DEFAULT FALSE,

    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT "PK_Users"
    PRIMARY KEY ("Id")
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

    "Token" TEXT NOT NULL,

    "ExpiredAt" TIMESTAMPTZ NOT NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    "IsRevoked" BOOLEAN NOT NULL DEFAULT FALSE,

    "UserId" UUID NOT NULL,

    CONSTRAINT "PK_RefreshTokens"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_RefreshTokens_Users_UserId"
    FOREIGN KEY ("UserId")
    REFERENCES mysolution."Users"("Id")
    ON DELETE CASCADE
);

-- EmailVerificationTokens
CREATE TABLE IF NOT EXISTS mysolution."EmailVerificationTokens"
(
    "Id" UUID NOT NULL DEFAULT gen_random_uuid(),
    
    "UserId" UUID NOT NULL,

    "Token" TEXT NOT NULL,

    "ExpiresAt" TIMESTAMPTZ NOT NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,
    
    "IsUsed" BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT "PK_EmailVerificationTokens"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_EmailVerificationTokens_Users_UserId"
    FOREIGN KEY ("UserId")
    REFERENCES mysolution."Users" ("Id")
    ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS mysolution."PasswordResetTokens"
(
    "Id" UUID NOT NULL DEFAULT gen_random_uuid(),

    "UserId" UUID NOT NULL,

    "Token" TEXT NOT NULL,

    "CreatedAt" TIMESTAMPTZ NOT NULL,

    "ExpiresAt" TIMESTAMPTZ NOT NULL,

    "IsUsed" BOOLEAN NOT NULL DEFAULT FALSE,

    CONSTRAINT "PK_PassswordResetTokens"
    PRIMARY KEY ("Id"),

    CONSTRAINT "FK_PasswordResetTokens_Users"
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

CREATE UNIQUE INDEX IF NOT EXISTS
    "IX_EmailVerificationTokens_Token"
    ON mysolution."EmailVerificationTokens" ("Token");

CREATE INDEX IF NOT EXISTS
    "IX_EmailVerificationTokens_UserId"
    ON mysolution."EmailVerificationTokens" ("UserId");

