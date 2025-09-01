using CrudASP.NET.Models;
using System;
using System.Linq;
using System.Web;

public static class AuthHelper
{
    /// <summary>
    /// Verifica si el usuario autenticado tiene un permiso sobre un módulo
    /// </summary>
    public static bool UserHasPermission(string moduleName, string action)
    {
        var username = HttpContext.Current.User.Identity.Name; // DOMAIN\usuario

        using (var db = new BitacoraMvcDbEntities())
        {
            // Buscar usuario
            var user = db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null) return false;

            // Verificar si existe el permiso asignado vía alguno de sus roles
            var hasPermission = (from ur in db.UserRoles
                                 join rp in db.RolePermissions on ur.RoleId equals rp.RoleId
                                 join p in db.Permissions on rp.PermissionId equals p.PermissionId
                                 join m in db.Modules on p.ModuleId equals m.ModuleId
                                 where ur.UserId == user.UserId
                                       && m.Name == moduleName
                                       && p.Action == action
                                 select p).Any();

            return hasPermission;
        }
    }
}
