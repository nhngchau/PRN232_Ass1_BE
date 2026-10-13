-- Assignment 2 Admin account seed for PostgreSQL.
-- Run this after Assignment2_Auth_Migration.sql.
-- POST /api/auth/register only creates Staff accounts, so the Admin account is inserted directly here.
-- The password is stored as a PBKDF2-SHA256 hash in the same format produced by Pbkdf2PasswordHasher
-- (never a plain-text password). The plain-text password is kept out of the repository; change it
-- after the first login from the /profile page if needed.
-- Emails are stored lowercase because the API normalizes emails before lookup.

INSERT INTO "SystemAccount" ("FullName", "Email", "PasswordHash", "Role", "CreatedDate")
VALUES (
    'System Administrator',
    'admin@tasktrack.com',
    'PBKDF2-SHA256$100000$pr4mcVdhjvNlUxRiCJg4Ug==$Gp8icgUJgBdOv7mIxSWYVL0iX+e7xUNA8HGW6Apy8Pg=',
    1,
    CURRENT_TIMESTAMP
)
ON CONFLICT ("Email") DO NOTHING;
