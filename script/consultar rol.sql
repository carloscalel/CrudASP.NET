SELECT u.UserId,u.UserName, r.Name AS Rol, m.Name AS Modulo, p.Action
FROM Users u
JOIN UserRoles ur ON u.UserId = ur.UserId
JOIN Roles r ON ur.RoleId = r.RoleId
JOIN RolePermissions rp ON r.RoleId = rp.RoleId
JOIN Permissions p ON rp.PermissionId = p.PermissionId
JOIN Modules m ON p.ModuleId = m.ModuleId
WHERE u.UserName = 'CALEL\calel';


--UPDATE Users SET UserName = 'CALEL\calel' WHERE UserId = 1