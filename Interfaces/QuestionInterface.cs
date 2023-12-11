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
    }
}
