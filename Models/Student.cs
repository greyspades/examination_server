namespace Student.Models;

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
    public string? CurrSchool { get; set; }
    public string? LastSchool { get; set; }
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
    public string? NumberOfWives { get; set; }
    public string? NumberOfChildren { get; set; }
}

public class StudentAuth {
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set;}
}

public class EducationDto {
    public List<EducationInfo>? Data { get; set; }
}

public class EducationInfo {
    public string? Id { get; set; }
    public string? School { get; set; }
    public string? Address { get; set; }
    public string? Cert { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
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
    public string? Id { get; set ; }
    public string? FirstResult { get; set ; }
    public string? SecondResult { get; set ; }
    public string? ThirdResult { get; set ; }
    public string? AccountCard { get; set ; }
    public string? Passport { get; set ; }
    public string? MembershipCard { get; set ; }
    public string? BirthCert { get; set ; }
    public string? Attestation { get; set ; }
}

public class BankingInfo {
    public string? Id { get; set; }
    public string? MpkNo { get; set; }
    public string? BranchManager { get; set; }
    public string? Area { get; set; }
    public string? AccountName { get; set; }
    public string? BranchName { get; set; }
    public string? AccountUnion { get; set; }
}

public class Document {
    public string? Name { get; set; }
    public string? Id { get; set; }
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