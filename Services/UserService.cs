using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using Npgsql;

public class UserService
{
    private readonly string _connectionString;

    public UserService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void CreateUser(string username, string password)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = $"CREATE USER {username} WITH PASSWORD '{password}'";
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void DropUser(string username)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = $"DROP USER {username}";
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void GrantPrivileges(string username, string database)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = $"GRANT ALL PRIVILEGES ON DATABASE {database} TO {username}";
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void RevokePrivileges(string username, string database)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = $"REVOKE ALL PRIVILEGES ON DATABASE {database} FROM {username}";
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void GrantSchemaPrivileges(string username, string schema)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = $"GRANT ALL PRIVILEGES ON SCHEMA {schema} TO {username}";
                cmd.ExecuteNonQuery();
            }
        }
    }

    public string GetUsers()
    {
        List<User> users = new List<User>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = @"SELECT 
                    u.usename AS username,
                    r.rolcreatedb AS creation_time,
                    r.rolvaliduntil AS expiration_time,
                    CASE WHEN r.rolsuper THEN 'true' ELSE '' END AS superuser,
                    CASE WHEN r.rolcreaterole THEN 'true' ELSE '' END AS createrole,
                    CASE WHEN r.rolcreatedb THEN 'true' ELSE '' END AS createdb,
                    CASE WHEN r.rolcanlogin THEN 'true' ELSE '' END AS canlogin,
                    CASE WHEN r.rolreplication THEN 'true' ELSE '' END AS replication,
                    CASE WHEN r.rolbypassrls THEN 'true' ELSE '' END AS bypassrls,
                    CASE WHEN r.rolinherit THEN 'true' ELSE '' END AS inherit,
                    CASE WHEN r.rolconnlimit <> -1 THEN 'connection limit: ' || r.rolconnlimit ELSE '' END AS connlimit
                    FROM
                        pg_catalog.pg_user u
                    JOIN
                        pg_catalog.pg_roles r ON u.usename = r.rolname;";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User
                        {
                            Username = reader.GetString(0),
                            Superuser = reader.GetString(3),
                            Createrole = reader.GetString(4),
                            Createdb = reader.GetString(5),
                            Canlogin = reader.GetString(6),
                            Replication = reader.GetString(7),
                            Bypassrls = reader.GetString(8),
                            Inherit = reader.GetString(9),
                            Connlimit = reader.GetString(10)
                        };
                        users.Add(user);

                    }
                }
            }
        }
        string json = JsonConvert.SerializeObject(users, Formatting.None);


        Console.WriteLine(json);
        return json;
    }
}

public class User
{
    public required string Username { get; set; }
    public string? Superuser { get; set; }
    public string? Createrole { get; set; }
    public string? Createdb { get; set; }
    public string? Canlogin { get; set; }
    public string? Replication { get; set; }
    public string? Bypassrls { get; set; }
    public string? Inherit { get; set; }
    public string? Connlimit { get; set; }
}