using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Web;
using CrudASP.NET.Models;

namespace CrudASP.NET.Models // <-- usa el namespace real de tu EDMX
{
    public partial class BitacoraMvcDbEntities: DbContext
    {
        public override int SaveChanges()
        {
            // Datos del usuario y request actuales
            var http = HttpContext.Current;
            var userName = http?.User?.Identity?.Name ?? Environment.UserName;
            var ip = GetClientIp(http);
            var userAgent = http?.Request?.UserAgent;

            // Controller/Action/Url si estamos en una petición MVC
            string url = http?.Request?.Url?.ToString();
            string ctrl = (string)http?.Items["__ctrl"];
            string act = (string)http?.Items["__act"];

            ChangeTracker.DetectChanges();

            var auditableEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in auditableEntries)
            {
                var entityName = entry.Entity.GetType().Name;
                var action = entry.State.ToString(); // Added|Modified|Deleted

                var before = entry.State == EntityState.Added ? null : ValuesToDictionary(entry.OriginalValues);
                var after = entry.State == EntityState.Deleted ? null : ValuesToDictionary(entry.CurrentValues);

                var log = new AuditLogs
                {
                    EntityName = entityName,
                    EntityKey = GetEntityKey(entry), // puede ser null en Added si PK se genera después
                    Action = action,
                    UserName = userName,
                    IpAddress = ip,
                    UserAgent = userAgent,
                    Controller = ctrl,
                    ActionName = act,
                    Url = url,
                    Timestamp = DateTime.Now,
                    BeforeJson = before == null ? null : JsonConvert.SerializeObject(before),
                    AfterJson = after == null ? null : JsonConvert.SerializeObject(after)
                };

                this.AuditLogs.Add(log);
                //this.Set<AuditLogs>().Add(log);
            }

            return base.SaveChanges();
        }

        private static string GetClientIp(HttpContext ctx)
        {
            if (ctx == null) return null;
            var ip = ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(ip))
                ip = ctx.Request.UserHostAddress;
            return ip;
        }

        private static Dictionary<string, object> ValuesToDictionary(DbPropertyValues values)
        {
            return values.PropertyNames.ToDictionary(pn => pn, pn => values[pn]);
        }

        // Obtiene PK por convención: NombreTipo + "Id" o "Id"
        private static string GetEntityKey(DbEntityEntry entry)
        {
            var t = entry.Entity.GetType();
            var byNameId = t.GetProperty(t.Name + "Id", BindingFlags.Public | BindingFlags.Instance);
            var byId = t.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);

            object keyVal = null;
            if (byNameId != null) keyVal = byNameId.GetValue(entry.Entity);
            else if (byId != null) keyVal = byId.GetValue(entry.Entity);

            return keyVal?.ToString();
        }
    }
}
