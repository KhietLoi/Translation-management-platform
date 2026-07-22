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

    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    
    "SecurityStamp" VARCHAR(256) NOT NULL DEFAULT gen_random_uuid()::text,
    
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


