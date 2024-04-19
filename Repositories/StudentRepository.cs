using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using Student.Interface;
using Student.Models;
using Credentials.Models;
using Newtonsoft.Json.Linq;
using AES;
using System.Text.Json;
using System.Text;
using System.Net.Mime;
using HTML;
using User.Models;
namespace Student.Repository;

public class StudentRepository : IStudentRepository
{
    private readonly IConfiguration _config;
    Guid guid = Guid.NewGuid();
    private static readonly Random random = new();
    public StudentRepository(IConfiguration config)
    {
        this._config = config;
    }
    public async Task RegisterUser(StudentAuth payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Register_student", new { Id = payload.Id, Email = payload.Email }, commandType: CommandType.StoredProcedure);
        await connection.ExecuteAsync("Save_password", new { Id = payload.Id, Password = payload.Password }, commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<StudentAuth>> CheckRegistration(StudentAuth payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<StudentAuth>("Check_registration", new { Email = payload.Email }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<string> GetPassword(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<string>("Get_password", new { Id }, commandType: CommandType.StoredProcedure);

        return data.First();
    }
    public async Task SavePersonalInfo(BasicInfo payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Save_personal", payload, commandType: CommandType.StoredProcedure);
    }
    public async Task UpdatePersonalInfo(BasicInfo payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Update_personal", payload, commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<BasicInfo>> GetPersonalInfo(string id)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<BasicInfo>("Get_personal_info", new { Id = id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task SaveParentsInfo(ParentsInfo payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Save_parents", payload, commandType: CommandType.StoredProcedure);
    }
    public async Task UpdateParentsInfo(ParentsInfo payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Update_parents", payload, commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<ParentsInfo>> GetParentsInfo(string id)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<ParentsInfo>("Get_parents_info", new { Id = id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task SaveEducationInfo(EducationInfo payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Save_education", payload, commandType: CommandType.StoredProcedure);
    }
    public async Task DeleteEducationInfo(string id)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Delete_education_info", new { Id = id }, commandType: CommandType.StoredProcedure);
    }
    public async Task UpdateEducationInfo(EducationInfo payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Update_education", payload, commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<EducationInfo>> GetEducationInfo(string id)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<EducationInfo>("Get_education_info", new { Id = id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task SaveBankingInfo(BankingInfo payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Save_banking", payload, commandType: CommandType.StoredProcedure);
    }
    public async Task UpdateBankingInfo(BankingInfo payload)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Update_banking", payload, commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<BankingInfo>> GetBankingInfo(string id)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<BankingInfo>("Get_banking_info", new { Id = id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<string> GetDocumentExtension(string id, string name)
    {

        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<string>("Get_document_extension", new { Id = id, Name = name }, commandType: CommandType.StoredProcedure);

        return data.First();
    }

    public async Task SaveDocument(IFormCollection documents)
    {
        var firstId = Guid.NewGuid().ToString();
        var secondId = Guid.NewGuid().ToString();
        var thirdId = Guid.NewGuid().ToString();
        var accountCardId = Guid.NewGuid().ToString();
        var passportId = Guid.NewGuid().ToString();
        var membershipCardId = Guid.NewGuid().ToString();
        var attestationId = Guid.NewGuid().ToString();
        var birthCertId = Guid.NewGuid().ToString();
        var principalRefId = Guid.NewGuid().ToString();
        var studentId = documents["id"].ToString();

        List<Document> documentList = new() {
            new Document {Name="referenceByPrincipal", Id=principalRefId},
            new Document {Name="accountCard", Id=accountCardId},
            new Document {Name="thirdResult", Id=thirdId},
            new Document {Name="attestation", Id=attestationId},
            new Document {Name="birthCert", Id=birthCertId},
            new Document {Name="membershipCard", Id=membershipCardId},
            new Document {Name="passport", Id=passportId},
            new Document {Name="secondResult", Id=secondId},
            new Document {Name="firstResult", Id=firstId},
        };
        // Console.WriteLine( documents["id"]);
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        foreach (var doc in documents.Files)
        {
            if (doc.Length > 0)
            {
                var id = documentList.Where((item) => item.Name == doc.Name).First().Id;
                string extension = Path.GetExtension(doc.FileName).ToLower();
                // path document is saved to
                var filePath = @$"documents/students/{id}{extension}";
                // path saved to the database
                var dbPath = @$"{id}{extension}";
                using (var stream = File.Create(filePath))
                {
                    await doc.CopyToAsync(stream);
                }
                await connection.ExecuteAsync("Save_document", new { doc.Name, Id = studentId, Extension = extension, Path = dbPath, FileId = id }, commandType: CommandType.StoredProcedure);
            }
        }

        await connection.ExecuteAsync("Complete_registration", new { Id = studentId, Date = DateTime.Now }, commandType: CommandType.StoredProcedure);
    }

    public static string GenerateUniqueId()
    {
        string prefix = "LS";
        int currentYear = DateTime.Now.Year;
        string uniqueNumbers = GenerateUniqueNumbers(5);
        string uniqueId = $"{prefix}{currentYear}{uniqueNumbers}";
        return uniqueId;
    }

    private static string GenerateUniqueNumbers(int length)
    {
        Random random = new Random();
        string uniqueNumbers = "";

        for (int i = 0; i < length; i++)
        {
            uniqueNumbers += random.Next(10); // Generate a random digit from 0 to 9
        }

        return uniqueNumbers;
    }
    public async Task ParseExcell(IFormFile file)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        CredentialsObj cred = await GetCredentials();
        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            stream.Position = 0;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first worksheet
                if (worksheet.Dimension.End.Row > 2000)
                {
                    return;
                }

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    var student = new StudentData
                    {
                        Branch = worksheet.Cells[row, 5].Value.ToString(),
                        FirstName = worksheet.Cells[row, 2].Value.ToString(),
                        LastName = worksheet.Cells[row, 3].Value.ToString(),
                        BUnion = worksheet.Cells[row, 4].Value.ToString(),
                        ClientId = worksheet.Cells[row, 7].Value.ToString(),
                        Email = worksheet.Cells[row, 9].Value.ToString(),
                        Phone = worksheet.Cells[row, 10].Value.ToString(),
                        ProductType = worksheet.Cells[row, 8].Value.ToString(),
                        Sn = worksheet.Cells[row, 1].Value.ToString(),
                        Zone = worksheet.Cells[row, 6].Value.ToString(),
                        Id = GenerateUniqueId()
                    };
                    await connection.ExecuteAsync("Add_client", new { Branch = student.Branch, Id = student.Id, FirstName = student.FirstName, LastName = student.LastName, BUnion = student.BUnion, ClientId = student.ClientId, Email = student.Email, Phone = student.Phone, ProductType = student.ProductType, Sn = student.Sn, Zone = student.Zone }, commandType: CommandType.StoredProcedure);
                    var mailObj = new EmailDto
                    {
                        emailaddress = student.Email,
                        subject = "Scholarship Id",
                        hasFile = "No",
                        body = HTMLHelper.SendId(student.FirstName, student.Id),
                    };
                    await SendMail(mailObj, cred);
                }
            }
        }
    }
    public async Task<IEnumerable<StudentData>> GetStudents()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<StudentData>("Get_all_students", commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<dynamic> GetApplicationInfo(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<dynamic>("Get_application_info", new { Id }, commandType: CommandType.StoredProcedure);

        return data.First();
    }
    public async Task<IEnumerable<StudentData>> GetStudentById(string id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<StudentData>("Get_student_by_id", new { Id = id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<BasicInfo>> GetPersonalInfo()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<BasicInfo>("Get_applications", commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<StudentData>> GetPendingVerifications()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<StudentData>("Get_applications", commandType: CommandType.StoredProcedure);
        // Console.WriteLine(data.First().Passport);
        return data;
    }
    public async Task<IEnumerable<StudentAuth>> GetStudentByEmail(string email)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<StudentAuth>("Get_student_by_email", new { Email = email }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<DocumentsDto>> GetDocuments(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<DocumentsDto>("Get_documents", new { Id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<dynamic>> GetCandidates()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<dynamic>("Get_candidates", commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<Settings> GetSettings()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<Settings>("Get_settings", commandType: CommandType.StoredProcedure);

        return data.First();
    }
    public async Task ApproveClient(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        string currentYear = DateTime.Now.Year.ToString();
        string uniquePart = Guid.NewGuid().ToString("N").Substring(0, 8); // Get the first 8 characters of a new GUID

        string uniqueId = currentYear + uniquePart;

        await connection.ExecuteAsync("Approve_client", new { Id, ExamId = uniqueId }, commandType: CommandType.StoredProcedure);
    }
    public async Task RejectClient(string Id, string Reason)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Reject_client", new { Id, Reason }, commandType: CommandType.StoredProcedure);
    }
    public async Task SaveExcelFile(IFormFile file)
    {
        string extension = Path.GetExtension(file.FileName).ToLower();
        var filePath = @$"documents/excel/{file.FileName}";
        // path saved to the database
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);

            File.SetLastWriteTime(filePath, DateTime.Now);
        }
    }
    public async Task<IEnumerable<ExamDetails>> GetExamDetails(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<ExamDetails>("Validate_exam_id", new { Id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<dynamic>> GetDashboardDetails(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<dynamic>("Get_dashBoard_details", new { Id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<dynamic>> GetScoresById(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<dynamic>("Get_score_by_id", new { Id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<dynamic>> ValidateInfo(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<dynamic>("Get_dashBoard_details", new { Id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<CredentialsObj> GetCredentials()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<CredentialsObj>("Get_credentials", commandType: CommandType.StoredProcedure);

        return data.First();
    }
    public async Task<dynamic> GetMetrics()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<dynamic>("Get_metrics", commandType: CommandType.StoredProcedure);

        return data.First();
    }
    public async Task UpdateStatus(string Id, string Status)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Update_status", new { Id, Status}, commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<dynamic>> GetAllScores(string Scope)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        return await connection.QueryAsync<dynamic>("Get_all_scores", new { Scope }, commandType: CommandType.StoredProcedure);
    }
    public async Task<dynamic> GetAccountInfo(string accountNumber, CredentialsObj cred)
    {

        var content = JsonSerializer.Serialize(new
        {
            AccountNo = accountNumber
        });

        var encryptedBody = AEShandler.Encrypt(content, cred.AesKey, cred.AesIv);

        byte[] bytes = Convert.FromBase64String(encryptedBody);

        string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        var stringifiedBody = JsonSerializer.Serialize(new
        {
            xPayload = hexString
        });

        HttpClient client = new();

        client.DefaultRequestHeaders.Add("x-lapo-eve-proc", cred.Token);

        var requestBody = new StringContent(stringifiedBody, System.Text.Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await client.PostAsync(
           _config.GetValue<string>("LASM_cred:Acc_details_url"), requestBody
       );

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jsonData = JObject.Parse(jsonResponse);

        if (jsonData.Value<string>("status") == "200")
        {
            byte[] stringBytes = Convert.FromHexString(jsonData.Value<string>("data"));

            string bytes64 = Convert.ToBase64String(stringBytes);

            var decrypted = AEShandler.Decrypt(bytes64, cred.AesKey, cred.AesIv);

            var data = JsonSerializer.Deserialize<AccountInfo>(decrypted);

            return data;
        }
        return null;
    }

    public async Task<string> SendMail(EmailDto payload, CredentialsObj cred)
    {
        var content = JsonSerializer.Serialize(payload);

        var encryptedBody = AEShandler.Encrypt(content, cred.AesKey, cred.AesIv);

        byte[] bytes = Convert.FromBase64String(encryptedBody);

        string hexString = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        HttpClient client = new();

        client.DefaultRequestHeaders.Add("x-lapo-eve-proc", cred.Token);

        using MultipartFormDataContent multipartContent = new()
        {
            { new StringContent(hexString, Encoding.UTF8, MediaTypeNames.Text.Plain), "xPayload" }
        };

        using HttpResponseMessage response = await client.PostAsync(
            _config.GetValue<string>("LASM_cred:Mail_url"), multipartContent
        );

        var jsonResponse = await response.Content.ReadAsStringAsync();

        return jsonResponse;

        // if(payload.HasFile == "Yes") {
        //     var fileStream = File.OpenRead("templates/letter.pdf");
        //     var options = new RestClientOptions(_config.GetValue<string>("E360:Mail_url"))
        //     {
        //     MaxTimeout = -1,
        //     };
        //     var rest = new RestClient(options);
        //     var request = new RestRequest(_config.GetValue<string>("E360:Mail_url"), Method.Post);
        //     request.AddHeader("x-lapo-eve-proc", cred.Token);
        //     request.AlwaysMultipartFormData = true;
        //     request.AddParameter("xPayload", hexString);

        //     request.AddFile("xFile", "templates/letter.pdf");

        //     RestResponse response = await rest.ExecuteAsync(request);

        //     return response.Content;

        // } else {
        //     HttpClient client = new();

        //     client.DefaultRequestHeaders.Add("x-lapo-eve-proc", cred.Token);

        //     using MultipartFormDataContent multipartContent = new()
        // {
        //     { new StringContent(hexString, Encoding.UTF8, MediaTypeNames.Text.Plain), "xPayload" }

        // };

        // using HttpResponseMessage response = await client.PostAsync(
        //     _config.GetValue<string>("E360:Mail_url"), multipartContent
        // );

        // var jsonResponse = await response.Content.ReadAsStringAsync();

        // return jsonResponse;
        // }
    }
}