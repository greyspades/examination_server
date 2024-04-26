namespace Student.Models;

public class IdDTO {
    public string? Id { get; set; }
    public string? Support { get; set; }
}

public class BasicInfo {
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateTime? Dob { get; set; }
    public string? Pob { get; set; }
    public string? LivesWith { get; set; }
    public string? Email { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Religion { get; set; }
}

public class ParentsInfo {
    public string? Id { get; set; }
    public string? NameOfFather { get; set; }
    public string? NameOfMother { get; set; }
    public string? FathersOccupation { get; set; }
    public string? MothersOccupation { get; set; }
    public string? FathersWorkAddress { get; set; }
    public string? MothersWorkAddress { get; set; }
    public string? FathersHomeAddress { get; set; }
    public string? MothersHomeAddress { get; set; }
    public string? NumberOfChildren { get; set; }
}

public class StudentAuth {
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set;}
    public bool? Registered { get; set;}
    public bool? Validated { get; set; }
}

public class ResultsDto {
    public string? Scope { get; set; }
    public int? Page { get; set; }
    public string? SearchVal { get; set; }
}

public class EducationDto {
    public List<EducationInfo>? Data { get; set; }
}

public class ExamDetails {
    public bool? Approved { get; set; }
    public string? Cert { get; set; }
}

public class EducationInfo {
    public string? Id { get; set; }
    public string? School { get; set; }
    public string? Address { get; set; }
    public string? Cert { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? currentSchool { get; set; }
}
public class StudentData {
    public string? Id { get; set; }
    public string? Sn { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? BUnion { get; set; }
    public string? Branch { get; set; }
    public string? Zone { get; set; }
    public string? ClientId { get; set; }
    public string? ProductType { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Passport { get; set; }
    public bool? Registered { get; set; }
    public bool? Approved { get; set; }
    public string? Status { get; set; }
    public string? ExamId { get; set; }
    public bool? TakenExam { get; set; }
}

public class Documents {
    public string? Id { get; set ; }
    public IFormCollection? Docs { get; set; }
    // public IFormFile? FirstResult { get; set ; }
    // public IFormFile? SecondResult { get; set ; }
    // public IFormFile? ThirdResult { get; set ; }
    // public IFormFile? AccountCard { get; set ; }
    // public IFormFile? Passport { get; set ; }
    // public IFormFile? MembershipCard { get; set ; }
    // public IFormFile? BirthCert { get; set ; }
    // public IFormFile? Attestation { get; set ; }
}


public class DocumentsDto {
    public string? Id { get; set; }
    public string? Path { get; set; }
    public string? Name { get; set; }
    public string? FileId { get; set; }
    public string? Extension { get; set; }
}

public class BankingInfo {
    public string? Id { get; set; }
    public string? MpkAccNo { get; set; }
    public string? BranchManager { get; set; }
    public string? Area { get; set; }
    public string? AccountName { get; set; }
    public string? Branch { get; set; }
    public string? Zone { get; set; }
}
public class Document {
    public string? Name { get; set; }
    public string? Id { get; set; }
    public string? Extension { get; set; }
}

public class StudentDto {
    public string? Scope { get; set; }
    public string? Id { get; set; }
    public BasicInfo? Basic { get; set; }
    public ParentsInfo? Parents { get; set; }
    public List<EducationInfo>? Education { get; set; }
    public Documents? Documents { get; set; }
    public DocumentsDto? Docs { get; set; }
    public BankingInfo? Banking { get; set; }
}

public class EmailDto {
    public string? subject { get; set; }
    public string? emailaddress { get; set; }
    public string? body { get; set; }
    public string? hasFile { get; set; }
}

public class AccountInfo
{
    public string? account_number { get; set; }
    public string? account_name { get; set; }
    public string? branch_code { get; set; }
    public string? branch_name { get; set; }
    public string? customer_number { get; set; }
    public string? bank_verification_number { get; set; }
    public string? account_type { get; set; }
    public string? account_class { get; set; }
    public string? account_status { get; set; }
    public string? gender { get; set; }
    public string? primary_phone_number { get; set; }
    public string? primary_email_address { get; set; }
    public string? primary_physical_address { get; set; }
}

public class DeclineDto {
    public string? Id { get; set; }
    public string? Reason { get; set; }
}
