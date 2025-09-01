CREATE TABLE Modules (
    ModuleId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL -- Ejemplo: 'Productos', 'Ventas'
);

CREATE TABLE Permissions (
    PermissionId INT PRIMARY KEY IDENTITY,
    ModuleId INT FOREIGN KEY REFERENCES Modules(ModuleId),
    Action NVARCHAR(50) NOT NULL -- Ejemplo: 'Create', 'Edit', 'Delete', 'View'
);

CREATE TABLE Roles (
    RoleId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE RolePermissions (
    RolePermissionId INT PRIMARY KEY IDENTITY,
    RoleId INT FOREIGN KEY REFERENCES Roles(RoleId),
    PermissionId INT FOREIGN KEY REFERENCES Permissions(PermissionId),
    CONSTRAINT UQ_RolePermission UNIQUE(RoleId, PermissionId) -- evita duplicados
);

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    UserName NVARCHAR(100) NOT NULL
);

CREATE TABLE UserRoles (
    UserRoleId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    RoleId INT FOREIGN KEY REFERENCES Roles(RoleId),
    CONSTRAINT UQ_UserRole UNIQUE(UserId, RoleId)
);


-- Módulos
INSERT INTO Modules (Name) VALUES ('Productos'), ('Ventas');

-- Permisos
INSERT INTO Permissions (ModuleId, Action) VALUES
(1, 'View'), (1, 'Create'), (1, 'Edit'), (1, 'Delete'), -- Productos
(2, 'View'), (2, 'Create'), (2, 'Edit'), (2, 'Delete'); -- Ventas

-- Roles
INSERT INTO Roles (Name) VALUES ('Administrador'), ('Editar'), ('Consulta');

-- Relación Roles-Permisos
-- Admin todo
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT 1, PermissionId FROM Permissions;

-- Editar: CRUD en Ventas, solo View en Productos
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT 2, PermissionId FROM Permissions WHERE ModuleId = 2; -- Ventas full
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT 2, PermissionId FROM Permissions WHERE ModuleId = 1 AND Action = 'View';

-- Consulta: solo ver
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT 3, PermissionId FROM Permissions WHERE Action = 'View';

INSERT INTO Users (UserName)
VALUES ('CALEL\calel'); -- Usuario Windows

INSERT INTO UserRoles (UserId, RoleId)
VALUES (1, 1); -- Usuario 1 con Rol Administrador

