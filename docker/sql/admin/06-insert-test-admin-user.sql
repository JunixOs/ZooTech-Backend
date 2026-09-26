/*
    Usuario administrador inicial de ZooTech.

    La contraseña NO se almacena aquí.

    El valor $(ADMIN_PASSWORD_HASH) debe ser generado por:
        ZooTech.Infrastructure.Identity.PasswordHasher

    Es decir, utilizando:
        PasswordHasher<object>.HashPassword(...)
*/

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.admin_users
    WHERE email = N'admin@zootech.com'
       OR username = N'admin'
)
BEGIN

    INSERT INTO dbo.admin_users
    (
        email,
        username,
        password_hash,
        first_name,
        last_name,
        is_active,
        metadata,
        created_at
    )
    VALUES
    (
        N'admin@zootech.com',
        N'admin',
        N'$(APP_ADMIN_USER_TEST_PASSWORD_HASH)',
        N'Junior',
        N'Silva',
        1,
        N'{
            "department": "IT",
            "role": "Administrator"
        }',
        SYSUTCDATETIME()
    );

END;
GO