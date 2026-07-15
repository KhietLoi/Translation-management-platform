-- ROLES
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
    )
ON CONFLICT DO NOTHING;

-- PERMISSIONS
INSERT INTO mysolution."Permissions"
(
    "Id",
    "Code",
    "Description",
    "CreatedAt",
    "UpdatedAt"
)
VALUES

    ('10000000-0000-0000-0000-000000000001','USER_VIEW','View users',NOW(),NOW()),
    ('10000000-0000-0000-0000-000000000002','USER_CREATE','Create users',NOW(),NOW()),
    ('10000000-0000-0000-0000-000000000003','USER_UPDATE','Update users',NOW(),NOW()),
    ('10000000-0000-0000-0000-000000000004','USER_DELETE','Delete users',NOW(),NOW()),

    ('10000000-0000-0000-0000-000000000005','ROLE_VIEW','View roles',NOW(),NOW()),
    ('10000000-0000-0000-0000-000000000006','ROLE_CREATE','Create roles',NOW(),NOW()),
    ('10000000-0000-0000-0000-000000000007','ROLE_UPDATE','Update roles',NOW(),NOW()),
    ('10000000-0000-0000-0000-000000000008','ROLE_DELETE','Delete roles',NOW(),NOW()),

    ('10000000-0000-0000-0000-000000000009','PERMISSION_VIEW','View permissions',NOW(),NOW()),
    ('10000000-0000-0000-0000-000000000010','PERMISSION_CREATE','Create permissions',NOW(),NOW()),
    ('10000000-0000-0000-0000-000000000011','PERMISSION_UPDATE','Update permissions',NOW(),NOW()),
    ('10000000-0000-0000-0000-000000000012','PERMISSION_DELETE','Delete permissions',NOW(),NOW())

ON CONFLICT DO NOTHING;

--USER 
INSERT INTO mysolution."Users"
(
    "Id",
    "Username",
    "Email",
    "PasswordHash",
    "IsActive",
    "CreatedAt",
    "IsEmailVerified"
)
VALUES
    (
        'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        'admin',
        'admin@gmail.com',
        'AQAAAAIAAYagAAAAEDJZPvgqHdLIqrE3FCcpVszs8kbdPi1ELbwAIFVvvecfgPqBISyzB5qyu51oeFqvGw==',
        TRUE,
        NOW(),
     TRUE
    )
ON CONFLICT DO NOTHING;

-- USER ROLE
INSERT INTO mysolution."UserRoles"
(
    "UserId",
    "RoleId"
)
VALUES
(
    'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    '11111111-1111-1111-1111-111111111111'
)
ON CONFLICT DO NOTHING;

-- ROLE PERMISSIONS
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


