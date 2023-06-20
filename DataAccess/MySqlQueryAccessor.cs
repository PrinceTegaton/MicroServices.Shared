using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MicroServices.Shared.DataAccess
{
    public class MySqlQueryAccessor : IDatabaseQueryAccessor
    {
        public string ConnectionString { get; set; }

        public MySqlQueryAccessor(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public async Task<Dictionary<string, object>> GetSingleQueryToDictionaryAsync(SqlQueryType type, string query, IDictionary<string, object> parameters = null)
        {
            using var conn = new MySqlConnection(this.ConnectionString);
            using var cmd = new MySqlCommand(query, conn);

            cmd.CommandText = query;
            cmd.CommandType = type == SqlQueryType.Text ? CommandType.Text : CommandType.StoredProcedure;

            if (parameters != null && parameters.Any())
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
            }

            await conn.OpenAsync();
            var reader = await cmd.ExecuteReaderAsync();

            if (reader.HasRows && await reader.ReadAsync())
            {
                var columns = Enumerable.Range(0, reader.FieldCount).Select(a => new KeyValuePair<string, object>(reader.GetName(a), reader.GetValue(a))).ToList();

                return new Dictionary<string, object>(columns);
            }

            return null;
        }

        public async Task<Q> GetSingleQueryAsync<Q>(SqlQueryType type, string query, IDictionary<string, object> parameters = null)
        {
            var result = await GetListFromQueryAsync<Q>(type, query, parameters);
            if (result != null && result.Any())
            {
                return result[0];
            }

            return default;
        }

        public async Task<List<Q>> GetListFromQueryAsync<Q>(SqlQueryType type, string query, IDictionary<string, object> parameters = null)
        {
            using var conn = new MySqlConnection(this.ConnectionString);
            using var cmd = new MySqlCommand(query, conn);

            cmd.CommandText = query;
            cmd.CommandType = type == SqlQueryType.Text ? CommandType.Text : CommandType.StoredProcedure;

            if (parameters != null && parameters.Any())
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
            }

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            var columns = Enumerable.Range(0, reader.FieldCount).Select(a => reader.GetName(a).ToLower()).ToList();

            if (reader.HasRows)
            {
                List<Q> list = new();

                while (reader.Read())
                {
                    var obj = Activator.CreateInstance<Q>();

                    foreach (PropertyInfo prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        try
                        {
                            if (!columns.Contains(prop.Name.ToLower()))
                            {
                                continue;
                            }

                            if (object.Equals(reader[prop.Name], DBNull.Value))
                            {
                                continue;
                            }

                            if (double.TryParse(reader[prop.Name].ToString(), out _))
                            {
                                if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                                {
                                    prop.SetValue(obj, Convert.ToInt32(reader[prop.Name]), null);
                                }
                                else if (prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?))
                                {
                                    prop.SetValue(obj, Convert.ToInt64(reader[prop.Name]), null);
                                }
                                else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                                {
                                    prop.SetValue(obj, Convert.ToDouble(reader[prop.Name]), null);
                                }
                                else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                                {
                                    prop.SetValue(obj, Convert.ToDecimal(reader[prop.Name]), null);
                                }
                                else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                                {
                                    prop.SetValue(obj, Convert.ToBoolean(reader[prop.Name]), null);
                                }
                                else
                                {
                                    prop.SetValue(obj, reader[prop.Name], null);
                                }
                            }
                            else
                            {
                                prop.SetValue(obj, reader[prop.Name], null);
                            }
                        }
                        catch (Exception)
                        {
                            throw new NotImplementedException(String.Format("Unable to map field {0}. Type '{1}' not implemented yet!", prop.Name, prop.PropertyType.Name));
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }

            return default;
        }
    }
}