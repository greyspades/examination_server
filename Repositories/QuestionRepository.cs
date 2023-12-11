using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Question.Interface;
using Question.Model;

namespace Question.Repository;

public class QuestionRepository : IQuestionRepository
{
    private readonly IConfiguration _config;

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

        await connection.ExecuteAsync("Create_question", new { Id = payload.Id, Question = payload.Question, Bank = payload.Bank, Answer = payload.Answer, Instructions = payload.Instructions, Subject = payload.Subject }, commandType: CommandType.StoredProcedure);
        await connection.ExecuteAsync("Create_answer", new { Id = payload.Id, Answer = payload.Answer }, commandType: CommandType.StoredProcedure);
    }
    public async Task CreateOption(Option payload)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        await connection.ExecuteAsync("Create_options", new { Id = payload.Id, Character = payload.Character, Value = payload.Value }, commandType: CommandType.StoredProcedure);
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
     public async Task<IEnumerable<QuestionBank>> GetBanks()
    {
        using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        var data = await connection.QueryAsync<QuestionBank>("Get_question_banks", commandType: CommandType.StoredProcedure);

        return data;
    }
}
