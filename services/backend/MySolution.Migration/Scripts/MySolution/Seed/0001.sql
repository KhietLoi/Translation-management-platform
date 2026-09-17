BEGIN;

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ============================================================
-- ROLES
-- Scope:
-- 1 = System
-- 2 = Organization
-- ============================================================

INSERT INTO mysolution."Roles"
(
    "Id",
    "Name",
    "Description",
    "Scope",
    "CreatedAt",
    "UpdatedAt"
)
VALUES
    (
        '11111111-1111-1111-1111-111111111111',
        'SystemAdmin',
        'Platform system administrator',
        1,
        NOW(),
        NOW()
    ),
    (
        '22222222-2222-2222-2222-222222222222',
        'Admin',
        'Organization administrator',
        2,
        NOW(),
        NOW()
    ),
    (
        '33333333-3333-3333-3333-333333333333',
        'User',
        'Organization user',
        2,
        NOW(),
        NOW()
    )
    ON CONFLICT DO NOTHING;


-- ============================================================
-- PERMISSIONS
-- ============================================================

INSERT INTO mysolution."Permissions"
(
    "Id",
    "Code",
    "Description",
    "CreatedAt",
    "UpdatedAt"
)
VALUES

-- ============================================================
-- PLATFORM - USER
-- ============================================================

(
    '10000000-0000-0000-0000-000000000001',
    'USER_VIEW',
    'View platform users',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000002',
    'USER_CREATE',
    'Create platform users',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000003',
    'USER_UPDATE',
    'Update platform users',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000004',
    'USER_DELETE',
    'Delete platform users',
    NOW(),
    NOW()
),

-- ============================================================
-- PLATFORM - ROLE
-- ============================================================

(
    '10000000-0000-0000-0000-000000000005',
    'ROLE_VIEW',
    'View roles',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000006',
    'ROLE_CREATE',
    'Create roles',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000007',
    'ROLE_UPDATE',
    'Update roles',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000008',
    'ROLE_DELETE',
    'Delete roles',
    NOW(),
    NOW()
),

-- ============================================================
-- PLATFORM - PERMISSION
-- ============================================================

(
    '10000000-0000-0000-0000-000000000009',
    'PERMISSION_VIEW',
    'View permissions',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000010',
    'PERMISSION_CREATE',
    'Create permissions',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000011',
    'PERMISSION_UPDATE',
    'Update permissions',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000012',
    'PERMISSION_DELETE',
    'Delete permissions',
    NOW(),
    NOW()
),

-- ============================================================
-- PROJECT
-- ============================================================

(
    '10000000-0000-0000-0000-000000000013',
    'PROJECT_VIEW',
    'View projects',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000014',
    'PROJECT_CREATE',
    'Create projects',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000015',
    'PROJECT_UPDATE',
    'Update projects',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000016',
    'PROJECT_DELETE',
    'Delete projects',
    NOW(),
    NOW()
),

-- ============================================================
-- LANGUAGE
-- ============================================================

(
    '10000000-0000-0000-0000-000000000017',
    'LANGUAGE_VIEW',
    'View languages',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000018',
    'LANGUAGE_CREATE',
    'Create languages',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000019',
    'LANGUAGE_UPDATE',
    'Update languages',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000020',
    'LANGUAGE_DELETE',
    'Delete languages',
    NOW(),
    NOW()
),

-- ============================================================
-- TRANSLATION
-- ============================================================

(
    '10000000-0000-0000-0000-000000000021',
    'TRANSLATION_VIEW',
    'View translations',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000022',
    'TRANSLATION_CREATE',
    'Create translations',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000023',
    'TRANSLATION_UPDATE',
    'Update translations',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000024',
    'TRANSLATION_DELETE',
    'Delete translations',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000025',
    'TRANSLATION_REVIEW',
    'Review translations',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000026',
    'TRANSLATION_PUBLISH',
    'Publish translations',
    NOW(),
    NOW()
),

-- ============================================================
-- API KEY
-- ============================================================

(
    '10000000-0000-0000-0000-000000000027',
    'APIKEY_VIEW',
    'View API keys',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000028',
    'APIKEY_CREATE',
    'Create API keys',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000029',
    'APIKEY_UPDATE',
    'Update API keys',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000030',
    'APIKEY_DELETE',
    'Delete API keys',
    NOW(),
    NOW()
),

-- ============================================================
-- AUDIT LOG
-- ============================================================

(
    '10000000-0000-0000-0000-000000000031',
    'AUDITLOG_VIEW',
    'View audit logs',
    NOW(),
    NOW()
),

-- ============================================================
-- SYSTEM ORGANIZATION
-- ============================================================

(
    '10000000-0000-0000-0000-000000000032',
    'SYSTEM_ORGANIZATION_VIEW',
    'View organizations at platform level',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000033',
    'SYSTEM_ORGANIZATION_MANAGE',
    'Manage organizations at platform level',
    NOW(),
    NOW()
),

-- ============================================================
-- ORGANIZATION
-- ============================================================

(
    '10000000-0000-0000-0000-000000000034',
    'ORGANIZATION_VIEW',
    'View organization information',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000035',
    'ORGANIZATION_UPDATE',
    'Update organization information',
    NOW(),
    NOW()
),

-- ============================================================
-- ORGANIZATION MEMBER
-- ============================================================

(
    '10000000-0000-0000-0000-000000000036',
    'ORGANIZATION_MEMBER_VIEW',
    'View organization members',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000037',
    'ORGANIZATION_MEMBER_INVITE',
    'Invite organization members',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000038',
    'ORGANIZATION_MEMBER_REMOVE',
    'Remove organization members',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000039',
    'ORGANIZATION_MEMBER_CHANGE_ROLE',
    'Change organization member role',
    NOW(),
    NOW()
),

-- ============================================================
-- PROJECT ACCESS
-- ============================================================

(
    '10000000-0000-0000-0000-000000000040',
    'PROJECT_ACCESS_ALL',
    'Access all projects in organization',
    NOW(),
    NOW()
),

-- ============================================================
-- IMPORT / EXPORT
-- ============================================================

(
    '10000000-0000-0000-0000-000000000041',
    'TRANSLATION_IMPORT',
    'Import translations',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000042',
    'TRANSLATION_EXPORT',
    'Export translations',
    NOW(),
    NOW()
),

-- ============================================================
-- APPLICATION
-- ============================================================

(
    '10000000-0000-0000-0000-000000000043',
    'APPLICATION_VIEW',
    'View applications',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000044',
    'APPLICATION_CREATE',
    'Create applications',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000045',
    'APPLICATION_UPDATE',
    'Update applications',
    NOW(),
    NOW()
),
(
    '10000000-0000-0000-0000-000000000046',
    'APPLICATION_DELETE',
    'Delete applications',
    NOW(),
    NOW()
)

    ON CONFLICT DO NOTHING;


-- ============================================================
-- USERS
-- ============================================================

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


-- ============================================================
-- USER PROFILES
-- ============================================================

INSERT INTO mysolution."UserProfiles"
(
    "UserId",
    "FullName",
    "CreatedAt",
    "UpdatedAt"
)
SELECT
    u."Id",
    x."FullName",
    NOW(),
    NOW()
FROM
    (
        VALUES
            ('admin', 'System Administrator'),
            ('translator1', 'Organization User 1'),
            ('reviewer1', 'Organization User 2')
    ) AS x("Username", "FullName")
        JOIN mysolution."Users" u
             ON u."Username" = x."Username"
    ON CONFLICT DO NOTHING;


-- ============================================================
-- USER ROLES
--
-- IMPORTANT:
-- UserRoles chỉ dùng cho SYSTEM ROLE.
-- ============================================================

INSERT INTO mysolution."UserRoles"
(
    "UserId",
    "RoleId"
)
SELECT
    u."Id",
    r."Id"
FROM mysolution."Users" u
         CROSS JOIN mysolution."Roles" r
WHERE u."Username" = 'admin'
  AND r."Name" = 'SystemAdmin'
  AND r."Scope" = 1
    ON CONFLICT DO NOTHING;


-- ============================================================
-- SYSTEM ADMIN PERMISSIONS
--
-- SystemAdmin quản lý platform.
-- Không tự động có quyền tenant business data.
-- ============================================================

INSERT INTO mysolution."RolePermissions"
(
    "RoleId",
    "PermissionId"
)
SELECT
    r."Id",
    p."Id"
FROM mysolution."Roles" r
         CROSS JOIN mysolution."Permissions" p
WHERE r."Name" = 'SystemAdmin'
  AND r."Scope" = 1
  AND p."Code" IN
      (
       'USER_VIEW',
       'USER_CREATE',
       'USER_UPDATE',
       'USER_DELETE',

       'ROLE_VIEW',
       'ROLE_CREATE',
       'ROLE_UPDATE',
       'ROLE_DELETE',

       'PERMISSION_VIEW',
       'PERMISSION_CREATE',
       'PERMISSION_UPDATE',
       'PERMISSION_DELETE',

       'LANGUAGE_VIEW',
       'LANGUAGE_CREATE',
       'LANGUAGE_UPDATE',
       'LANGUAGE_DELETE',

       'SYSTEM_ORGANIZATION_VIEW',
       'SYSTEM_ORGANIZATION_MANAGE',

       'AUDITLOG_VIEW'
          )
    ON CONFLICT DO NOTHING;


-- ============================================================
-- ORGANIZATION ADMIN PERMISSIONS
-- ============================================================

INSERT INTO mysolution."RolePermissions"
(
    "RoleId",
    "PermissionId"
)
SELECT
    r."Id",
    p."Id"
FROM mysolution."Roles" r
         CROSS JOIN mysolution."Permissions" p
WHERE r."Name" = 'Admin'
  AND r."Scope" = 2
  AND p."Code" IN
      (
       'ORGANIZATION_VIEW',
       'ORGANIZATION_UPDATE',

       'ORGANIZATION_MEMBER_VIEW',
       'ORGANIZATION_MEMBER_INVITE',
       'ORGANIZATION_MEMBER_REMOVE',
       'ORGANIZATION_MEMBER_CHANGE_ROLE',

       'PROJECT_VIEW',
       'PROJECT_CREATE',
       'PROJECT_UPDATE',
       'PROJECT_DELETE',
       'PROJECT_ACCESS_ALL',

       'LANGUAGE_VIEW',

       'TRANSLATION_VIEW',
       'TRANSLATION_CREATE',
       'TRANSLATION_UPDATE',
       'TRANSLATION_DELETE',
       'TRANSLATION_REVIEW',
       'TRANSLATION_PUBLISH',
       'TRANSLATION_IMPORT',
       'TRANSLATION_EXPORT',

       'APPLICATION_VIEW',
       'APPLICATION_CREATE',
       'APPLICATION_UPDATE',
       'APPLICATION_DELETE',

       'APIKEY_VIEW',
       'APIKEY_CREATE',
       'APIKEY_UPDATE',
       'APIKEY_DELETE'
          )
    ON CONFLICT DO NOTHING;


-- ============================================================
-- ORGANIZATION USER PERMISSIONS
-- ============================================================

INSERT INTO mysolution."RolePermissions"
(
    "RoleId",
    "PermissionId"
)
SELECT
    r."Id",
    p."Id"
FROM mysolution."Roles" r
         CROSS JOIN mysolution."Permissions" p
WHERE r."Name" = 'User'
  AND r."Scope" = 2
  AND p."Code" IN
      (
       'ORGANIZATION_VIEW',

       'PROJECT_VIEW',

       'LANGUAGE_VIEW',

       'TRANSLATION_VIEW',
       'TRANSLATION_CREATE',
       'TRANSLATION_UPDATE',
       'TRANSLATION_EXPORT',

       'APPLICATION_VIEW'
          )
    ON CONFLICT DO NOTHING;


-- ============================================================
-- ORGANIZATION
-- Status:
-- 1 = Active
-- ============================================================

INSERT INTO mysolution."Organizations"
(
    "Id",
    "Name",
    "Slug",
    "Status",
    "CreatedAt",
    "UpdatedAt"
)
VALUES
    (
        '88888888-8888-8888-8888-888888888888',
        'MySolution Demo Organization',
        'mysolution-demo',
        1,
        NOW(),
        NOW()
    )
    ON CONFLICT DO NOTHING;


-- ============================================================
-- ORGANIZATION MEMBERS
--
-- admin       = Admin
-- translator1 = User
-- reviewer1   = User
-- ============================================================

INSERT INTO mysolution."OrganizationMembers"
(
    "OrganizationId",
    "UserId",
    "RoleId",
    "CreatedAt",
    "UpdatedAt"
)
SELECT
    '88888888-8888-8888-8888-888888888888',
    u."Id",
    r."Id",
    NOW(),
    NOW()
FROM mysolution."Users" u
         JOIN mysolution."Roles" r
              ON r."Name" =
                 CASE u."Username"
                     WHEN 'admin' THEN 'Admin'
                     WHEN 'translator1' THEN 'User'
                     WHEN 'reviewer1' THEN 'User'
                     END
                  AND r."Scope" = 2
WHERE u."Username" IN
      (
       'admin',
       'translator1',
       'reviewer1'
          )
    ON CONFLICT DO NOTHING;


-- ============================================================
-- LANGUAGES
-- ============================================================

INSERT INTO mysolution."Languages"
(
    "Id",
    "Code",
    "Name",
    "CreatedAt"
)
VALUES
    (gen_random_uuid(), 'vi-VN', 'Vietnamese', NOW()),
    (gen_random_uuid(), 'en-US', 'English', NOW()),
    (gen_random_uuid(), 'ja-JP', 'Japanese', NOW()),
    (gen_random_uuid(), 'ko-KR', 'Korean', NOW()),
    (gen_random_uuid(), 'zh-CN', 'Chinese (Simplified)', NOW()),
    (gen_random_uuid(), 'zh-TW', 'Chinese (Traditional)', NOW()),
    (gen_random_uuid(), 'fr-FR', 'French', NOW()),
    (gen_random_uuid(), 'de-DE', 'German', NOW())
    ON CONFLICT ("Code") DO NOTHING;


-- ============================================================
-- PROJECTS
--
-- Tất cả Project phải thuộc Organization.
-- ============================================================

INSERT INTO mysolution."Projects"
(
    "Id",
    "OrganizationId",
    "Name",
    "Description",
    "IsActive",
    "CreatedAt"
)
VALUES
    (
        '66666666-6666-6666-6666-666666666666',
        '88888888-8888-8888-8888-888888888888',
        'Ecommerce Platform',
        'Translation project for ecommerce platform',
        TRUE,
        NOW()
    ),
    (
        '77777777-7777-7777-7777-777777777777',
        '88888888-8888-8888-8888-888888888888',
        'Mobile Banking',
        'Translation project for banking application',
        TRUE,
        NOW()
    )
    ON CONFLICT DO NOTHING;


-- ============================================================
-- PROJECT NAMESPACES
-- ============================================================

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


-- ============================================================
-- PROJECT LANGUAGES
-- ============================================================

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
WHERE l."Code" IN
      (
       'vi-VN',
       'en-US'
          )
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
WHERE l."Code" IN
      (
       'vi-VN',
       'en-US',
       'ko-KR'
          )
    ON CONFLICT DO NOTHING;


-- ============================================================
-- PROJECT MEMBERS
--
-- Admin KHÔNG cần ProjectMember vì có PROJECT_ACCESS_ALL.
--
-- translator1 → Ecommerce
-- reviewer1   → Mobile Banking
-- ============================================================

INSERT INTO mysolution."ProjectMembers"
(
    "ProjectId",
    "UserId",
    "CreatedAt"
)
VALUES
    (
        '66666666-6666-6666-6666-666666666666',
        'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        NOW()
    ),
    (
        '77777777-7777-7777-7777-777777777777',
        'cccccccc-cccc-cccc-cccc-cccccccccccc',
        NOW()
    )
    ON CONFLICT DO NOTHING;


-- ============================================================
-- TRANSLATION KEYS
-- ============================================================

INSERT INTO mysolution."TranslationKeys"
(
    "Id",
    "ProjectId",
    "NamespaceId",
    "Key",
    "Description",
    "CreatedAt"
)
SELECT
    gen_random_uuid(),
    p."Id",
    n."Id",
    t."Key",
    t."Description",
    NOW()
FROM mysolution."Projects" p

         JOIN mysolution."ProjectNamespaces" n
              ON n."ProjectId" = p."Id"

         JOIN
     (
         VALUES
             ('Common', 'button.save', 'Save button'),
             ('Common', 'button.cancel', 'Cancel button'),
             ('Common', 'button.search', 'Search button'),
             ('Common', 'button.delete', 'Delete button'),

             ('Auth', 'login.title', 'Login title'),
             ('Auth', 'login.username', 'Username'),
             ('Auth', 'login.password', 'Password'),

             ('Product', 'product.name', 'Product Name'),
             ('Product', 'product.price', 'Product Price'),
             ('Product', 'product.description', 'Product Description'),

             ('Transaction', 'transaction.title', 'Transaction title'),
             ('Transaction', 'transaction.amount', 'Transaction amount'),
             ('Transaction', 'transaction.history', 'Transaction history')
     ) AS t
         (
          "Namespace",
          "Key",
          "Description"
             )
     ON n."Name" = t."Namespace"

    ON CONFLICT DO NOTHING;


-- ============================================================
-- TRANSLATION VALUES
--
-- ReviewedBy = Organization Admin
--
-- Ecommerce translated by translator1
-- Mobile Banking translated by reviewer1
-- ============================================================

INSERT INTO mysolution."TranslationValues"
(
    "Id",
    "TranslationKeyId",
    "LanguageId",
    "Value",
    "Status",
    "TranslatedBy",
    "ReviewedBy",
    "CreatedAt",
    "TranslatedAt",
    "ReviewedAt"
)
SELECT
    gen_random_uuid(),
    tk."Id",
    l."Id",
    v."Value",

    CASE
        WHEN tk."Key" IN
             (
              'button.save',
              'button.cancel',
              'button.search',
              'button.delete',
              'login.title'
                 )
            THEN 3 -- Reviewed

        ELSE 2 -- Translated
        END,

    CASE
        WHEN tk."ProjectId" =
             '66666666-6666-6666-6666-666666666666'
            THEN
            'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'::uuid

        WHEN tk."ProjectId" =
             '77777777-7777-7777-7777-777777777777'
            THEN
            'cccccccc-cccc-cccc-cccc-cccccccccccc'::uuid
        END,

    CASE
        WHEN tk."Key" IN
             (
              'button.save',
              'button.cancel',
              'button.search',
              'button.delete',
              'login.title'
                 )
            THEN
            'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'::uuid

        ELSE NULL::uuid
        END,

    NOW(),

    NOW(),

    CASE
        WHEN tk."Key" IN
             (
              'button.save',
              'button.cancel',
              'button.search',
              'button.delete',
              'login.title'
                 )
            THEN NOW()

        ELSE NULL
        END

FROM mysolution."TranslationKeys" tk

         JOIN mysolution."ProjectLanguages" pl
              ON pl."ProjectId" = tk."ProjectId"

         JOIN mysolution."Languages" l
              ON l."Id" = pl."LanguageId"

         JOIN
     (
         VALUES

             ('button.save', 'en-US', 'Save'),
             ('button.save', 'vi-VN', 'Lưu'),
             ('button.save', 'ko-KR', '저장'),

             ('button.cancel', 'en-US', 'Cancel'),
             ('button.cancel', 'vi-VN', 'Hủy'),
             ('button.cancel', 'ko-KR', '취소'),

             ('button.search', 'en-US', 'Search'),
             ('button.search', 'vi-VN', 'Tìm kiếm'),
             ('button.search', 'ko-KR', '검색'),

             ('button.delete', 'en-US', 'Delete'),
             ('button.delete', 'vi-VN', 'Xóa'),
             ('button.delete', 'ko-KR', '삭제'),

             ('login.title', 'en-US', 'Login'),
             ('login.title', 'vi-VN', 'Đăng nhập'),
             ('login.title', 'ko-KR', '로그인'),

             ('login.username', 'en-US', 'Username'),
             ('login.username', 'vi-VN', 'Tên đăng nhập'),
             ('login.username', 'ko-KR', '사용자 이름'),

             ('login.password', 'en-US', 'Password'),
             ('login.password', 'vi-VN', 'Mật khẩu'),
             ('login.password', 'ko-KR', '비밀번호'),

             ('product.name', 'en-US', 'Product Name'),
             ('product.name', 'vi-VN', 'Tên sản phẩm'),
             ('product.name', 'ko-KR', '제품명'),

             ('product.price', 'en-US', 'Price'),
             ('product.price', 'vi-VN', 'Giá'),
             ('product.price', 'ko-KR', '가격'),

             ('product.description', 'en-US', 'Description'),
             ('product.description', 'vi-VN', 'Mô tả'),
             ('product.description', 'ko-KR', '제품 설명'),

             ('transaction.title', 'en-US', 'Transaction'),
             ('transaction.title', 'vi-VN', 'Giao dịch'),
             ('transaction.title', 'ko-KR', '거래'),

             ('transaction.amount', 'en-US', 'Amount'),
             ('transaction.amount', 'vi-VN', 'Số tiền'),
             ('transaction.amount', 'ko-KR', '금액'),

             ('transaction.history', 'en-US', 'History'),
             ('transaction.history', 'vi-VN', 'Lịch sử'),
             ('transaction.history', 'ko-KR', '거래 내역')

     ) AS v
         (
          "Key",
          "LanguageCode",
          "Value"
             )
     ON v."Key" = tk."Key"
         AND v."LanguageCode" = l."Code"

    ON CONFLICT DO NOTHING;


-- ============================================================
-- APPLICATIONS
-- ============================================================

INSERT INTO mysolution."Applications"
(
    "Id",
    "ProjectId",
    "Name",
    "Description",
    "CreatedAt",
    "CreatedBy"
)
VALUES
    (
        gen_random_uuid(),
        '66666666-6666-6666-6666-666666666666',
        'Ecommerce Web',
        'Frontend application',
        NOW(),
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'
    ),
    (
        gen_random_uuid(),
        '77777777-7777-7777-7777-777777777777',
        'Mobile Banking API',
        'Public API',
        NOW(),
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'
    )
    ON CONFLICT DO NOTHING;


-- ============================================================
-- AUDIT LOGS
-- ============================================================

INSERT INTO mysolution."AuditLogs"
(
    "Id",
    "UserId",
    "Action",
    "EntityName",
    "EntityId",
    "OldValue",
    "NewValue",
    "CreatedAt"
)
SELECT
    gen_random_uuid(),
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    1,
    'Project',
    p."Id",
    NULL,
    p."Name",
    NOW()
FROM mysolution."Projects" p

    ON CONFLICT DO NOTHING;


COMMIT;