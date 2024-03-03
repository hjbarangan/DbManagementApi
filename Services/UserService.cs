using Npgsql;
using System.Collections.Generic;
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

public IEnumerable<string> GetUsers()
{
    List<string> users = new List<string>();

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
                Console.WriteLine("Users: {0}", reader);
                while (reader.Read())
                {
                    // Concatenate the values of CASE expressions and add them to the users list
                    string user = $"{reader.GetString(0)} - Superuser: {reader.GetString(3)}, Create Role: {reader.GetString(4)}, Create DB: {reader.GetString(5)}, Can Login: {reader.GetString(6)}, Replication: {reader.GetString(7)}, Bypass RLS: {reader.GetString(8)}, Inherit: {reader.GetString(9)}, Conn Limit: {reader.GetString(10)}";
                    users.Add(user);
                }
            }
        }
    }

    return users;
}
}
