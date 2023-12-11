using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using Student.Interface;
using Student.Models;

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
    public async Task<IEnumerable<string>> GetPassword(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<string>("Get_password", new { Id }, commandType: CommandType.StoredProcedure);

        return data;
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

        foreach (var doc in documents.Files)
        {
            if (doc.Length > 0)
            {
                var id = documentList.Where((item) => item.Name == doc.Name).First().Id;
                string extension = Path.GetExtension(doc.FileName).ToLower();
                var path = @$"documents/students/{id}{extension}";
                using (var stream = File.Create(path))
                {
                    await doc.CopyToAsync(stream);
                }
            }
        }
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Save_document", new { Id = studentId, FirstResult = firstId, SecondResult = secondId, ThirdResult = thirdId, AccountCard = accountCardId, Passport = passportId, MembershipCard = membershipCardId, Attestation = attestationId, BirthCert = birthCertId }, commandType: CommandType.StoredProcedure);
        await connection.ExecuteAsync("Complete_registration", new { Id = studentId }, commandType: CommandType.StoredProcedure);
    }
    public async Task ParseExcell(IFormFile file)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            stream.Position = 0;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first worksheet

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    // Generate a random number
                    int randomValue = random.Next(10000000, 99999999);

                    // Combine timestamp and random value to create a unique ID
                    string uniqueId = $"{timestamp}{randomValue}";
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
                        Id = uniqueId
                        // Map other properties accordingly
                    };
                    await connection.ExecuteAsync("Add_student", student, commandType: CommandType.StoredProcedure);
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

        var data = await connection.QueryAsync<StudentAuth>("Get_student_by_email", new { Email = email}, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<DocumentsDto>> GetDocuments(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<DocumentsDto>("Get_documents", new { Id }, commandType: CommandType.StoredProcedure);

        return data;
    }
}