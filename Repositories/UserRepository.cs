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
    public async Task<dynamic> AdminAuth(AdminDto payload)
    {
        HttpClient client = new();

        var cred = new CredHandler(_config);

        var credData = await cred.MakeContract();

        var token = new
        {
            tk = credData?[0],
            src = "AS-IN-D659B-e3M",
        };

        var body = new
        {
            UsN = payload.Id,
            Pwd = payload.Password,
            xAppSource = "AS-IN-D659B-e3M"
        };

        var jsonBody = JsonSerializer.Serialize(body);

        var encryptedBody = AEShandler.Encrypt(jsonBody, credData?[1], credData?[2]);

        byte[] bodyBytes = Convert.FromBase64String(encryptedBody);

        string bodyHexString = BitConverter.ToString(bodyBytes).Replace("-", "").ToLower();

        var jsonHeader = JsonSerializer.Serialize(token);

        var encryptedHeader = AEShandler.Encrypt(jsonHeader, credData?[1], credData?[2]);

        byte[] bytes = Convert.FromBase64String(encryptedHeader);

        string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        using StringContent jsonContent = new(
        bodyHexString,
        Encoding.UTF8,
        "application/json");

        client.DefaultRequestHeaders.Add("x-lapo-eve-proc", hexString + credData?[0]);

        using HttpResponseMessage response = await client.PostAsync(_config.GetValue<string>("E360:Signin_url"), jsonContent);

        var resData = await response.Content.ReadAsStringAsync();

        var jsonData = JObject.Parse(resData);

        // Console.WriteLine(jsonData);

        if (jsonData.Value<string>("status") == "200")
        {
            byte[] stringBytes = Convert.FromHexString(jsonData.Value<string>("data"));

            string bytes64 = Convert.ToBase64String(stringBytes);

            var decrypted = AEShandler.Decrypt(bytes64, credData?[1], credData?[2]);

            var data = JsonSerializer.Deserialize<dynamic>(decrypted);

            var res = new
            {
                code = 200,
                message = "Successful",
                credentials = credData,
                data
            };

            return res;
        }
        else if (jsonData.Value<string>("status") != "200")
        {
            // Console.WriteLine("something went wrong");
            var res = new
            {
                code = 400,
                message = jsonData.Value<string>("message_description"),
            };
            return res;
        }

        return Array.Empty<IEnumerable<dynamic>>();
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
