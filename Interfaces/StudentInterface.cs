using Student.Models;

namespace Student.Interface
{
    public interface IStudentRepository
    {
        public Task SaveDocument(IFormCollection documents);
        public Task SavePersonalInfo(BasicInfo payload);
        public Task SaveParentsInfo(ParentsInfo payload);
        public Task SaveEducationInfo(EducationInfo payload);
        public Task SaveBankingInfo(BankingInfo payload);
        public Task<IEnumerable<BasicInfo>> GetPersonalInfo(string id);
        public Task<IEnumerable<ParentsInfo>> GetParentsInfo(string id);
        public Task<IEnumerable<EducationInfo>> GetEducationInfo(string id);
        public Task<IEnumerable<BankingInfo>> GetBankingInfo(string id);
        // public void ParseExcell(IFormFile file);
        public Task ParseExcell(IFormFile file);
        public Task<IEnumerable<StudentData>> GetStudents();
        public Task<IEnumerable<StudentData>> GetStudentById(string id);
        public Task UpdatePersonalInfo(BasicInfo payload);
        public Task UpdateParentsInfo(ParentsInfo payload);
        public Task UpdateEducationInfo(EducationInfo payload);
        public Task UpdateBankingInfo(BankingInfo payload);
        public Task DeleteEducationInfo(string id);
        public Task RegisterUser(StudentAuth payload);
        public Task<IEnumerable<StudentAuth>> CheckRegistration(StudentAuth payload);
        public Task<IEnumerable<string>> GetPassword(string Id);
        public Task<IEnumerable<BasicInfo>> GetPersonalInfo();
        public Task<IEnumerable<StudentAuth>> GetStudentByEmail(string email);
        public Task<IEnumerable<StudentData>> GetPendingVerifications();
        public Task<IEnumerable<DocumentsDto>> GetDocuments(string Id);
    }
}
