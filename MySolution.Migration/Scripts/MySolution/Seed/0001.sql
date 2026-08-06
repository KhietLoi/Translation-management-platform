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


-- ==========================================
-- TRANSLATION KEYS
-- ==========================================

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
             ('Common','button.save','Save button'),
             ('Common','button.cancel','Cancel button'),
             ('Common','button.search','Search button'),
             ('Common','button.delete','Delete button'),

             ('Auth','login.title','Login title'),
             ('Auth','login.username','Username'),
             ('Auth','login.password','Password'),

             ('Product','product.name','Product Name'),
             ('Product','product.price','Product Price'),
             ('Product','product.description','Product Description'),

             ('Transaction','transaction.title','Transaction title'),
             ('Transaction','transaction.amount','Transaction amount'),
             ('Transaction','transaction.history','Transaction history')
     ) AS t
         (
          "Namespace",
          "Key",
          "Description"
             )
     ON n."Name" = t."Namespace"

    ON CONFLICT DO NOTHING;
-- Translation Values

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
                 ) THEN 3
        WHEN l."Code" = 'ko-KR' THEN 2
        ELSE 1
        END,

    CASE
        WHEN l."Code" = 'ko-KR'
            THEN NULL::uuid
        ELSE
            'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'::uuid
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
            THEN 'cccccccc-cccc-cccc-cccc-cccccccccccc'::uuid
        ELSE NULL::uuid
        END,

    NOW(),

    CASE
        WHEN l."Code" = 'ko-KR'
            THEN NULL
        ELSE NOW()
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

             ('button.save','en-US','Save'),
             ('button.save','vi-VN','Lưu'),
             ('button.save','ko-KR','저장'),

             ('button.cancel','en-US','Cancel'),
             ('button.cancel','vi-VN','Hủy'),
             ('button.cancel','ko-KR','취소'),

             ('button.search','en-US','Search'),
             ('button.search','vi-VN','Tìm kiếm'),
             ('button.search','ko-KR','검색'),

             ('button.delete','en-US','Delete'),
             ('button.delete','vi-VN','Xóa'),
             ('button.delete','ko-KR','삭제'),

             ('login.title','en-US','Login'),
             ('login.title','vi-VN','Đăng nhập'),
             ('login.title','ko-KR','로그인'),

             ('login.username','en-US','Username'),
             ('login.username','vi-VN','Tên đăng nhập'),
             ('login.username','ko-KR','사용자 이름'),

             ('login.password','en-US','Password'),
             ('login.password','vi-VN','Mật khẩu'),
             ('login.password','ko-KR','비밀번호'),

             ('product.name','en-US','Product Name'),
             ('product.name','vi-VN','Tên sản phẩm'),
             ('product.name','ko-KR','제품명'),

             ('product.price','en-US','Price'),
             ('product.price','vi-VN','Giá'),
             ('product.price','ko-KR','가격'),

             ('product.description','en-US','Description'),
             ('product.description','vi-VN','Mô tả'),
             ('product.description','ko-KR','제품 설명'),

             ('transaction.title','en-US','Transaction'),
             ('transaction.title','vi-VN','Giao dịch'),
             ('transaction.title','ko-KR','거래'),

             ('transaction.amount','en-US','Amount'),
             ('transaction.amount','vi-VN','Số tiền'),
             ('transaction.amount','ko-KR','금액'),

             ('transaction.history','en-US','History'),
             ('transaction.history','vi-VN','Lịch sử'),
             ('transaction.history','ko-KR','거래 내역')

     ) AS v("Key","LanguageCode","Value")
     ON v."Key" = tk."Key"
         AND v."LanguageCode" = l."Code"

    ON CONFLICT DO NOTHING;

-- ==========================================
-- AUDIT LOGS
-- ==========================================

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

-- ==========================================
-- APPLICATIONS
-- ==========================================

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