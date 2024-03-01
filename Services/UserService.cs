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
                cmd.CommandText = "SELECT usename FROM pg_catalog.pg_user";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(reader.GetString(0));
                    }
                }
            }
        }

        return users;
    }
}
