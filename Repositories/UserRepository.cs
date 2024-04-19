using System.Collections;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using Credentials.Models;
using CredentialsHandler;
using System.Text.Json;
using AES;
using System.Text;
using Newtonsoft.Json.Linq;
using User.Interface;
using User.Models;

namespace User.Repository;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _config;

    public UserRepository(IConfiguration config)
    {
        this._config = config;
    }
    public async Task<Settings> GetSettings() {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<Settings>("Get_settings", commandType: CommandType.StoredProcedure);

        return data.First();
    }
     public async Task SaveSettings(Settings payload) {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Save_settings", payload, commandType: CommandType.StoredProcedure);
    }
}
