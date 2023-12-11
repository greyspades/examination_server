using Microsoft.Extensions.Options;

namespace Question.Model;

public class Option {
    public string? Id { get; set; }
    public string? Character { get; set; }
    public string? Value { get; set; }
}
public class ExamDTO {
    public string? Subject { get; set; }
    public int? Number { get; set; }
}
public class ExamQuestion {
    public string? StudentId { get; set; }
    public string? SubjectId { get; set; }
    public QuestionModel? Question { get; set; }
}

public class QuestionModel {
    public string? Id { get; set; }
    public string? Question { get; set; }
    public List<Option>? Options { get; set; }
    public string? OptionsId { get; set; }
    public string? Answer { get; set; }
    public string? Bank { get; set; }
    public string? Instructions { get; set; }
    public string? Subject { get; set; }
}
public enum SubjectScope {JUNIOR, SENIOR}
public class Subject {
    public string? Id { get; set; }
    public string? Name { get; set; }
    public int? Questions { get; set; }
    public string? Scope { get; set; }
}

public class QuestionBank {
    public string? Name { get; set; }
    public string? Id { get; set; }
    public string? Description { get; set; }
    public bool? IsDefault { get; set; }
}