
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MySqlConnector;
using Dapper;
using TransactAPI.Models;
using System.Data;

namespace TransactAPI.Services;

public class MariaDBConn : IAsyncDisposable
{

    #region Properties

    private int connections = 0;
    public readonly string Host;
    public readonly int Port;
    public readonly string DBName;
    public readonly string User;
    private readonly string Password;



    #endregion


    #region LifeCycle

    private static SemaphoreSlim _lockObj = new(1, 1);
    private static List<MariaDBConn> _instances { get; set; } = new();
    public ReadOnlyCollection<MariaDBConn> Instances { get => _instances.AsReadOnly(); }

    public static MariaDBConn GetConnection(string host, int port, string dbName, string user, string password)
    {
        if (_instances.Any(d => d.Host == host && d.Port == port && d.DBName == dbName && d.User == user))
        {
            var dbConn = _instances.First(d => d.Host == host && d.Port == port && d.DBName == dbName && d.User == user);

            Interlocked.Increment(ref dbConn.connections);
            return dbConn;
        }
        var newConn = new MariaDBConn(host, port, dbName, user, password);
        newConn.TestConn();
        return newConn;
    }
    public MariaDBConn() => throw new NotImplementedException();

    private MariaDBConn(string host, int port, string dbName, string user, string password)
    {
        Host = host;
        Port = port;
        DBName = dbName;
        User = user;
        Password = password;
    }
    public async ValueTask DisposeAsync()
    {
        Interlocked.Decrement(ref connections);
        if (connections == 0)
        {
            _instances.Remove(this);
        }
    }

    #endregion

    #region Query Data Methods


    public async Task<List<TrasnactionDataModel>> GetTransactions(DateOnly startDate, DateOnly? endDate = null)
    {
        string storedProcedure = "";
        List<TrasnactionDataModel> results = new();
        await using MySqlConnection conn = new(AssembleConnString());
        await conn.OpenAsync();

        await using MySqlCommand cmd = new(storedProcedure, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("start_date", startDate);
        cmd.Parameters.AddWithValue("end_date", endDate ?? DateOnly.FromDateTime(DateTime.Now));

        await using MySqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new TrasnactionDataModel
            {
                ID = reader.GetString(reader.GetOrdinal("ID")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                PurchaseTotal = reader.GetDouble(reader.GetOrdinal("PurchaseTotal")),
                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                Currency = reader.GetString(reader.GetOrdinal("Currency")),
            });
        }
        return results;
    }



    #endregion


    #region Methods
    private string AssembleConnString() => $"Server={Host};Port={Port};Database={DBName};Uid={User};Pwd={Password};SslMode=none;Connection Timeout = 20; GuidFormat=Binary16;";

    private void TestConn()
    {
        using (MySqlConnection conn = new MySqlConnection(AssembleConnString()))
        {
            conn.Open();
            _ = conn.Query("SELECT NOW();");
            conn.Close();

        }
    }


    #endregion



}
