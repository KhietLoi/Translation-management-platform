-- ==========================================
-- ROLES
-- ==========================================
INSERT INTO mysolution."Roles"
(
    "Id",
    "Name",
    "Description",
    "CreatedAt",
    "UpdatedAt"
)
VALUES
    (
        '11111111-1111-1111-1111-111111111111',
        'Admin',
        'System Administrator',
        NOW(),
        NOW()
    ),
    (
        '22222222-2222-2222-2222-222222222222',
        'User',
        'Regular User',
        NOW(),
        NOW()
    ),
    (
        '33333333-3333-3333-3333-333333333333',
        'ProjectManager',
        'Manage translation projects',
        NOW(),
        NOW()
    ),
    (
        '44444444-4444-4444-4444-444444444444',
        'Translator',
        'Translate content',
        NOW(),
        NOW()
    ),
    (
        '55555555-5555-5555-5555-555555555555',
        'Reviewer',
        'Review translated content',
        NOW(),
        NOW()
    )
    ON CONFLICT DO NOTHING;

-- ==========================================
-- PERMISSIONS
-- ==========================================
INSERT INTO mysolution."Permissions"
(
    "Id",
    "Code",
    "Description",
    "CreatedAt",
    "UpdatedAt"
)
VALUES

-- User
('10000000-0000-0000-0000-000000000001','USER_VIEW','View users',NOW(),NOW()),
('10000000-0000-0000-0000-000000000002','USER_CREATE','Create users',NOW(),NOW()),
('10000000-0000-0000-0000-000000000003','USER_UPDATE','Update users',NOW(),NOW()),
('10000000-0000-0000-0000-000000000004','USER_DELETE','Delete users',NOW(),NOW()),

-- Role
('10000000-0000-0000-0000-000000000005','ROLE_VIEW','View roles',NOW(),NOW()),
('10000000-0000-0000-0000-000000000006','ROLE_CREATE','Create roles',NOW(),NOW()),
('10000000-0000-0000-0000-000000000007','ROLE_UPDATE','Update roles',NOW(),NOW()),
('10000000-0000-0000-0000-000000000008','ROLE_DELETE','Delete roles',NOW(),NOW()),

-- Permission
('10000000-0000-0000-0000-000000000009','PERMISSION_VIEW','View permissions',NOW(),NOW()),
('10000000-0000-0000-0000-000000000010','PERMISSION_CREATE','Create permissions',NOW(),NOW()),
('10000000-0000-0000-0000-000000000011','PERMISSION_UPDATE','Update permissions',NOW(),NOW()),
('10000000-0000-0000-0000-000000000012','PERMISSION_DELETE','Delete permissions',NOW(),NOW()),

-- Project
('10000000-0000-0000-0000-000000000013','PROJECT_VIEW','View projects',NOW(),NOW()),
('10000000-0000-0000-0000-000000000014','PROJECT_CREATE','Create projects',NOW(),NOW()),
('10000000-0000-0000-0000-000000000015','PROJECT_UPDATE','Update projects',NOW(),NOW()),
('10000000-0000-0000-0000-000000000016','PROJECT_DELETE','Delete projects',NOW(),NOW()),

-- Language
('10000000-0000-0000-0000-000000000017','LANGUAGE_VIEW','View languages',NOW(),NOW()),
('10000000-0000-0000-0000-000000000018','LANGUAGE_CREATE','Create languages',NOW(),NOW()),
('10000000-0000-0000-0000-000000000019','LANGUAGE_UPDATE','Update languages',NOW(),NOW()),
('10000000-0000-0000-0000-000000000020','LANGUAGE_DELETE','Delete languages',NOW(),NOW()),

-- Translation
('10000000-0000-0000-0000-000000000021','TRANSLATION_VIEW','View translations',NOW(),NOW()),
('10000000-0000-0000-0000-000000000022','TRANSLATION_CREATE','Create translations',NOW(),NOW()),
('10000000-0000-0000-0000-000000000023','TRANSLATION_UPDATE','Update translations',NOW(),NOW()),
('10000000-0000-0000-0000-000000000024','TRANSLATION_DELETE','Delete translations',NOW(),NOW()),
('10000000-0000-0000-0000-000000000025','TRANSLATION_REVIEW','Review translations',NOW(),NOW()),
('10000000-0000-0000-0000-000000000026','TRANSLATION_PUBLISH','Publish translations',NOW(),NOW())

ON CONFLICT DO NOTHING;

-- ==========================================
-- USERS
-- ==========================================
INSERT INTO mysolution."Users"
(
    "Id",
    "Username",
    "Email",
    "PasswordHash",
    "PasswordVersion",
    "Status",
    "CreatedAt",
    "IsEmailVerified"
)
VALUES
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        'admin',
        'admin@gmail.com',
        'AQAAAAIAAYagAAAAEDJZPvgqHdLIqrE3FCcpVszs8kbdPi1ELbwAIFVvvecfgPqBISyzB5qyu51oeFqvGw==',
        1,
        1,
        NOW(),
        TRUE
    ),
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        'translator1',
        'translator1@gmail.com',
        'AQAAAAIAAYagAAAAEDJZPvgqHdLIqrE3FCcpVszs8kbdPi1ELbwAIFVvvecfgPqBISyzB5qyu51oeFqvGw==',
        1,
        1,
        NOW(),
        TRUE
    ),
    (
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        'reviewer1',
        'reviewer1@gmail.com',
        'AQAAAAIAAYagAAAAEDJZPvgqHdLIqrE3FCcpVszs8kbdPi1ELbwAIFVvvecfgPqBISyzB5qyu51oeFqvGw==',
        1,
        1,
        NOW(),
        TRUE
    )
    ON CONFLICT DO NOTHING;

-- ==========================================
-- USER PROFILES
-- ==========================================
INSERT INTO mysolution."UserProfiles"
(
    "UserId",
    "FullName",
    "CreatedAt",
    "UpdatedAt"
)
VALUES
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        'System Administrator',
        NOW(),
        NOW()
    ),
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        'Translator User',
        NOW(),
        NOW()
    ),
    (
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        'Reviewer User',
        NOW(),
        NOW()
    )
    ON CONFLICT DO NOTHING;

-- ==========================================
-- USER ROLES
-- ==========================================
INSERT INTO mysolution."UserRoles"
(
    "UserId",
    "RoleId"
)
VALUES
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        '11111111-1111-1111-1111-111111111111'
    ),
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        '33333333-3333-3333-3333-333333333333'
    ),
    (
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        '44444444-4444-4444-4444-444444444444'
    ),
    (
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        '55555555-5555-5555-5555-555555555555'
    )
    ON CONFLICT DO NOTHING;

-- ==========================================
-- ADMIN FULL PERMISSIONS
-- ==========================================
INSERT INTO mysolution."RolePermissions"
(
    "RoleId",
    "PermissionId"
)
SELECT
    '11111111-1111-1111-1111-111111111111',
    p."Id"
FROM mysolution."Permissions" p
    ON CONFLICT DO NOTHING;
INSERT INTO mysolution."RolePermissions"
(
    "RoleId",
    "PermissionId"
)
--Project Manager:
SELECT
    '33333333-3333-3333-3333-333333333333',
    p."Id"
FROM mysolution."Permissions" p
WHERE p."Code" IN
      (
       'PROJECT_VIEW',
       'PROJECT_CREATE',
       'PROJECT_UPDATE',
       'PROJECT_DELETE',

       'LANGUAGE_VIEW',
       'LANGUAGE_CREATE',
       'LANGUAGE_UPDATE',
       'LANGUAGE_DELETE'
          )
    ON CONFLICT DO NOTHING;
--Translator:
INSERT INTO mysolution."RolePermissions"
(
    "RoleId",
    "PermissionId"
)
SELECT
    '44444444-4444-4444-4444-444444444444',
    p."Id"
FROM mysolution."Permissions" p
WHERE p."Code" IN
      (
       'PROJECT_VIEW',
       'LANGUAGE_VIEW'
          )
ON CONFLICT DO NOTHING;  

-- Reviewer:
INSERT INTO mysolution."RolePermissions"
(
    "RoleId",
    "PermissionId"
)
SELECT
    '55555555-5555-5555-5555-555555555555',
    p."Id"
FROM mysolution."Permissions" p
WHERE p."Code" IN
      (
       'PROJECT_VIEW',
       'LANGUAGE_VIEW'
          )
ON CONFLICT DO NOTHING;
-- ==========================================
-- LANGUAGES
-- ==========================================
INSERT INTO mysolution."Languages"
(
    "Id",
    "Code",
    "Name",
    "CreatedAt"
)
VALUES
    (gen_random_uuid(),'vi-VN','Vietnamese',NOW()),
    (gen_random_uuid(),'en-US','English',NOW()),
    (gen_random_uuid(),'ja-JP','Japanese',NOW()),
    (gen_random_uuid(),'ko-KR','Korean',NOW()),
    (gen_random_uuid(),'zh-CN','Chinese (Simplified)',NOW()),
    (gen_random_uuid(),'zh-TW','Chinese (Traditional)',NOW()),
    (gen_random_uuid(),'fr-FR','French',NOW()),
    (gen_random_uuid(),'de-DE','German',NOW())
    ON CONFLICT ("Code") DO NOTHING;

-- ==========================================
-- PROJECTS
-- ==========================================
INSERT INTO mysolution."Projects"
(
    "Id",
    "Name",
    "Description",
    "IsActive",
    "CreatedAt"
)
VALUES
    (
        '66666666-6666-6666-6666-666666666666',
        'Ecommerce Platform',
        'Translation project for ecommerce platform',
        TRUE,
        NOW()
    ),
    (
        '77777777-7777-7777-7777-777777777777',
        'Mobile Banking',
        'Translation project for banking application',
        TRUE,
        NOW()
    )
    ON CONFLICT DO NOTHING;

-- ==========================================
-- PROJECT NAMESPACES
-- ==========================================
INSERT INTO mysolution."ProjectNamespaces"
(
    "Id",
    "ProjectId",
    "Name",
    "CreatedAt"
)
VALUES
    (
        gen_random_uuid(),
        '66666666-6666-6666-6666-666666666666',
        'Common',
        NOW()
    ),
    (
        gen_random_uuid(),
        '66666666-6666-6666-6666-666666666666',
        'Auth',
        NOW()
    ),
    (
        gen_random_uuid(),
        '66666666-6666-6666-6666-666666666666',
        'Product',
        NOW()
    ),
    (
        gen_random_uuid(),
        '77777777-7777-7777-7777-777777777777',
        'Common',
        NOW()
    ),
    (
        gen_random_uuid(),
        '77777777-7777-7777-7777-777777777777',
        'Transaction',
        NOW()
    )
    ON CONFLICT DO NOTHING;

-- ==========================================
-- PROJECT LANGUAGES
-- ==========================================

-- Ecommerce
INSERT INTO mysolution."ProjectLanguages"
(
    "ProjectId",
    "LanguageId",
    "CreatedAt"
)
SELECT
    '66666666-6666-6666-6666-666666666666',
    l."Id",
    NOW()
FROM mysolution."Languages" l
WHERE l."Code" IN ('vi-VN','en-US')
    ON CONFLICT DO NOTHING;

-- Mobile Banking
INSERT INTO mysolution."ProjectLanguages"
(
    "ProjectId",
    "LanguageId",
    "CreatedAt"
)
SELECT
    '77777777-7777-7777-7777-777777777777',
    l."Id",
    NOW()
FROM mysolution."Languages" l
WHERE l."Code" IN ('vi-VN','en-US','ko-KR')
    ON CONFLICT DO NOTHING;

-- ==========================================
-- PROJECT MEMBERS
-- ==========================================
INSERT INTO mysolution."ProjectMembers"
(
    "ProjectId",
    "UserId",
    "CreatedAt"
)
VALUES
    (
        '66666666-6666-6666-6666-666666666666',
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        NOW()
    ),
    (
        '66666666-6666-6666-6666-666666666666',
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        NOW()
    ),
    (
        '77777777-7777-7777-7777-777777777777',
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        NOW()
    ),
    (
        '77777777-7777-7777-7777-777777777777',
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        NOW()
    )
    ON CONFLICT DO NOTHING;