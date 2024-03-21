using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Question.Interface;
using Question.Model;
using OfficeOpenXml;

namespace Question.Repository;

public class QuestionRepository : IQuestionRepository
{
    private readonly IConfiguration _config;
    Guid guid = Guid.NewGuid();

    public QuestionRepository(IConfiguration config)
    {
        this._config = config;
    }
    public async Task CreateQuestion(QuestionModel payload)
    {  
        //  if(payload.Image != null) {
        //     var path = @$"images/questions/{payload.Id}.jpg";
        //     using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        //     {
        //         await payload.Image.CopyToAsync(stream);
        //     }
        // }
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Create_question", new { Id = payload.Id, Question = payload.Question, Bank = payload.Bank, Answer = payload.Answer, Instructions = payload.Instructions, Subject = payload.Subject, Class = payload.Class }, commandType: CommandType.StoredProcedure);
        await connection.ExecuteAsync("Create_answer", new { Id = payload.Id, Answer = payload.Answer }, commandType: CommandType.StoredProcedure);
    }
    public async Task CreateOption(Option payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Create_options", new { Id = payload.Id, Character = payload.Character, Value = payload.Value }, commandType: CommandType.StoredProcedure);
    }
    public async Task UpdateOption(Option payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Update_options", new { Id = payload.Id, Character = payload.Character, Value = payload.Value }, commandType: CommandType.StoredProcedure);
    }
     public async Task DeleteOptions(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Delete_options", new { Id }, commandType: CommandType.StoredProcedure);
    }
    public async Task CreateSubject(Subject payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Create_senior_subject", new { payload.Id,  payload.Name, Questions = 0, payload.Scope }, commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<Subject>> GetSubjects(string scope)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

       var data = await connection.QueryAsync<Subject>("Get_subjects", new { Scope = scope }, commandType: CommandType.StoredProcedure);
    
        return data;
    }
        public async Task<IEnumerable<dynamic>> GetQuestions(string subject)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

       var data = await connection.QueryAsync<dynamic>("Get_questions_by_subject", new { Subject = subject }, commandType: CommandType.StoredProcedure);
    
        return data;
    }
    public async Task CreateBank(QuestionBank payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Create_questionbank", new { Id = payload.Id, Name = payload.Name, Description = payload.Description, IsDefault = payload.IsDefault }, commandType: CommandType.StoredProcedure);
    }
    public async Task UpdateQuestion(QuestionModel payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Update_question", new { Id = payload.Id, Question = payload.Question, Instructions = payload.Instructions, Bank = payload.Bank, Subject = payload.Subject, Answer = payload.Answer }, commandType: CommandType.StoredProcedure);
    }
     public async Task<IEnumerable<QuestionBank>> GetBanks()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<QuestionBank>("Get_question_banks", commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<QuestionModel>> GetExamQuestions(string Subject, string Class)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<QuestionModel>("Get_exam_questions", new { Subject, Class }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<IEnumerable<Option>> GetExamOptions(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<Option>("Get_exam_options", new { Id }, commandType: CommandType.StoredProcedure);

        return data;
    }
    public async Task<AnswerModel> GetAnswer(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<AnswerModel>("Get_answer", new { Id }, commandType: CommandType.StoredProcedure);

        return data.First();
    }
    public async Task CompleteExam(string Id)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Complete_exam", new { Id }, commandType: CommandType.StoredProcedure);
    }
    public async Task MarkScore(string Id, string Subject, string Scope)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var score = await connection.QueryAsync<Result>("Get_score", new { Id, Subject }, commandType: CommandType.StoredProcedure);

        if(!score.Any()) {
            await connection.ExecuteAsync("Create_score", new { Id, Subject, Score = 1, Scope }, commandType: CommandType.StoredProcedure);
        } else {
            await connection.ExecuteAsync("Update_score", new { Id, Subject}, commandType: CommandType.StoredProcedure);
        }
    }
    public async Task ParseQuestionFromExcell(IFormFile file, string subject, string studentClass)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var id = Guid.NewGuid().ToString();
        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            stream.Position = 0;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming the data is in the first worksheet
                Console.WriteLine("got to excell");
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    worksheet.Cells[row, 1].Value = Guid.NewGuid().ToString();

                    List<Option> options = new List<Option>{
                        new Option {
                        Character = "A",
                        Value = worksheet.Cells[row, 3].Value.ToString(),
                        Id = worksheet.Cells[row, 1].Value.ToString(),
                    },
                    new Option {
                        Character = "B",
                        Value = worksheet.Cells[row, 4].Value.ToString(),
                        Id = worksheet.Cells[row, 1].Value.ToString(),
                    },
                    new Option {
                        Character = "C",
                        Value = worksheet.Cells[row, 5].Value.ToString(),
                        Id = worksheet.Cells[row, 1].Value.ToString(),
                    },
                    new Option{
                        Character = "D",
                        Value = worksheet.Cells[row, 6].Value.ToString(),
                        Id = worksheet.Cells[row, 1].Value.ToString(),
                    }
                    } ;
                    var question = new QuestionModel
                    {   Id = worksheet.Cells[row, 1].Value.ToString(),
                        Question = worksheet.Cells[row, 2].Value.ToString(),
                        Answer = worksheet.Cells[row, 7].Value.ToString(),
                        Class = studentClass,
                        Instructions = worksheet.Cells[row, 8].Value.ToString(),
                        Subject = subject,
                    };

                    await connection.ExecuteAsync("Create_question", new { Id = question.Id, Question = question.Question, Answer = question.Answer, Class = question.Class, Instructions = question.Instructions, Subject = question.Subject }, commandType: CommandType.StoredProcedure);
                    await connection.ExecuteAsync("Create_answer", new { Id = question.Id, Answer = question.Answer }, commandType: CommandType.StoredProcedure);

                    foreach(Option option in options) {
                        await connection.ExecuteAsync("Create_options", option, commandType: CommandType.StoredProcedure);
                    }

                    // await connection.ExecuteAsync("Add_client", new {Branch = student.Branch, Id = student.Id, FirstName= student.FirstName, LastName=student.LastName, BUnion=student.BUnion, ClientId = student.ClientId,Email=student.Email,Phone=student.Phone, ProductType=student.ProductType,Sn = student.Sn, Zone= student.Zone}, commandType: CommandType.StoredProcedure);
                }
            }
        }
    }
}
