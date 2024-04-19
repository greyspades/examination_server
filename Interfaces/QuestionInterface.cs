using Question.Model;

namespace Question.Interface
{
    public interface IQuestionRepository
    {
        public Task CreateOption(Option payload);
        public Task CreateQuestion(QuestionModel payload);
        public Task CreateBank(QuestionBank payload);
        public Task<IEnumerable<QuestionBank>> GetBanks();
        public Task CreateSubject(Subject payload);
        public Task<IEnumerable<Subject>> GetSubjects(string scope);
        public Task<IEnumerable<dynamic>> GetQuestions(string subject);
        public Task UpdateQuestion(QuestionModel payload);
        public Task UpdateOption(Option payload);
        public Task DeleteOptions(string Id);
        public Task<IEnumerable<QuestionModel>> GetExamQuestions(string Subject, string Class);
        public Task<IEnumerable<Option>> GetExamOptions(string Id);
        public Task ParseQuestionFromExcell(IFormFile file, string subject, string studentClass);
        public Task<AnswerModel> GetAnswer(string Id);
        public Task MarkScore(string Id, string Subject, string scope);
        public Task CompleteExam(string Id);
        public Task<QuestionModel> GetExamQuestion(string Subject, string Class);
        public Task SaveDraft(Draft payload);
        public Task UpdateDraft(Draft payload);
        public Task<Draft> GetDraft(string Id);
        public Task AddAttemptedQuestion(string Id, string Subject);
    }
}
