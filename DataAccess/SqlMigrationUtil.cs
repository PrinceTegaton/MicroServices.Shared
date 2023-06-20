using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Linq;

namespace MicroServices.Shared.DataAccess
{

    public class SqlMigrationUtil
    {
        /// <summary>
        /// Execute specific SQL script from a folder
        /// </summary>
        /// <param name="db"></param>
        /// <param name="path"></param>
        /// <param name="objectNames"></param>
        public static void InjectViews(DatabaseFacade db, string path = null, params string[] objectNames)
        {
            foreach (var i in objectNames)
            {
                path ??= AppDomain.CurrentDomain.BaseDirectory + "/SqlObjects";
                string sqlFile = i.Trim() + ".sql";
                string sqlQuery = System.IO.File.ReadAllText($"{path}/{sqlFile}");
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return;
                }

                db.ExecuteSqlRaw(sqlQuery);
            }
        }

        /// <summary>
        /// Execute all SQL scripts in a folder. By default it checks from the root/SqlObjects folder. 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="path"></param>
        public static void InjectAllViews(DatabaseFacade db, string path = null)
        {
            path ??= AppDomain.CurrentDomain.BaseDirectory + "/SqlObjects";
            if (!System.IO.Directory.Exists(path))
            {
                return;
            }

            var files = System.IO.Directory.GetFiles(path, "*.sql").Where(a => System.IO.Path.GetFileName(a).ToLower().StartsWith("view_"));
            if (!files.Any())
            {
                return;
            }

            foreach (var sqlFile in files)
            {
                string objName = System.IO.Path.GetFileNameWithoutExtension(sqlFile);
                if (!objName.ToLower().StartsWith("view_"))
                {
                    continue;
                }

                string sqlQuery = System.IO.File.ReadAllText(sqlFile);
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    continue;
                }

                db.ExecuteSqlRaw(sqlQuery);
            }
        }

        /// <summary>
        /// Execute specific SQL script from a folder
        /// </summary>
        /// <param name="db"></param>
        /// <param name="path"></param>
        /// <param name="objectNames"></param>
        public static void InjectStoredProcedures(DatabaseFacade db, string path = null, params string[] objectNames)
        {
            foreach (var i in objectNames)
            {
                path ??= AppDomain.CurrentDomain.BaseDirectory + "/SqlObjects";
                string sqlFile = i.Trim() + ".sql";
                string sqlQuery = System.IO.File.ReadAllText($"{path}/{sqlFile}");
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    return;
                }

                db.ExecuteSqlRaw(sqlQuery);
            }
        }

        /// <summary>
        /// Execute all SQL scripts in a folder. By default it checks from the root/SqlObjects folder. 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="path"></param>
        public static void InjectAllStoredProcedures(DatabaseFacade db, string path = null)
        {
            path ??= AppDomain.CurrentDomain.BaseDirectory + "/SqlObjects";
            var files = System.IO.Directory.GetFiles(path, "*.sql").Where(a => System.IO.Path.GetFileName(a).ToLower().StartsWith("sp_"));
            if (!files.Any())
            {
                return;
            }

            foreach (var sqlFile in files)
            {
                string objName = System.IO.Path.GetFileNameWithoutExtension(sqlFile);
                string sqlQuery = System.IO.File.ReadAllText(sqlFile);
                if (string.IsNullOrEmpty(sqlQuery))
                {
                    continue;
                }

                db.ExecuteSqlRaw(sqlQuery);
            }
        }
    }
}