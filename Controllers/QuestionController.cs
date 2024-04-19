using Microsoft.AspNetCore.Mvc;
using Question.Interface;
using Question.Model;
using User.Models;

namespace Question.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController : ControllerBase
    {

        private readonly ILogger<QuestionController> _logger;
        Guid guid = Guid.NewGuid();
        IQuestionRepository _repo;
        public QuestionController(ILogger<QuestionController> logger, IQuestionRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromForm] QuestionModel payload) {
            try {
                var questionId = guid.ToString();
                payload.Id = questionId;
                await _repo.CreateQuestion(payload);

                foreach(Option option in payload.Options) {
                    option.Id = questionId;
                    await _repo.CreateOption(option);
                }

                return Ok(new {
                    code = 200,
                    message = "Question Created Successfully",
                });

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }
        [HttpPost("update")]
        public async Task<ActionResult> Update([FromForm] QuestionModel payload) {
            try {
                await _repo.UpdateQuestion(payload);
                await _repo.DeleteOptions(payload.Id);
                foreach(Option option in payload.Options) {
                    option.Id = payload.Id;
                    await _repo.CreateOption(option);
                }
                return Ok(new {
                    code = 200,
                    message = "Question Updated Successfully",
                });

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }
        [HttpPost("get_exam/question")]
        public async Task<ActionResult> GetExamQuestion(ExamDTO payload) {
                try {
                 var questions = await _repo.GetQuestions(payload.Subject);

                 List<dynamic> questionList = questions.ToList();

                return Ok(new {
                    code = 200,
                    message = "Question Created Successfully",
                    data = questionList[(int)payload.Number]
                });

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("bank")]
        public async Task<ActionResult> CreateBank(QuestionBank payload) {
            try {
                var banks = await _repo.GetBanks();
                bool nameExists = banks.Any((bank) => bank.Name == payload.Name);
                if(!nameExists) {
                    var bankId = guid.ToString();
                payload.Id = bankId;
                await _repo.CreateBank(payload);

                return Ok(new {
                    code = 200,
                    message = "Question bank Created Successfully",
                });
                } else {
                    return Ok(new {
                    code = 400,
                    message = "That name is taken, use a different name",
                });
                }

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("question/subject/{subject}")]
        public async Task<ActionResult> GetQuestions(string subject) {
            try {
                var data = await _repo.GetQuestions(subject);

                foreach(var question in data) {
                    string[] optionsArr = question.options.Split(",");
                    string[] charactersArr = question.characters.Split(",");
                    List<dynamic> options = new();
                    for(int i = 0; i< optionsArr.Length; i++) {
                        options.Add(new {Character = charactersArr[i], Value = optionsArr[i]});
                    }
                    question.options = options;
                }

                return Ok(new {
                    code = 200,
                    data
                });

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

         [HttpGet("subject/{scope}")]
        public async Task<ActionResult> GetSubjects(string scope) {
            try {
                var data = await _repo.GetSubjects(scope);

                return Ok(new {
                    code = 200,
                    data
                });

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }
        
        [HttpPost("subject")]
        public async Task<ActionResult> CreateSubject(Subject payload) {
            try {
                payload.Id = guid.ToString();

                await _repo.CreateSubject(payload);

                return Ok(new {
                    code = 200,
                    message = "Subject created successfully"
                });

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("banks")]
        public async Task<ActionResult<IEnumerable<QuestionBank>>> GetBanks() {
            try {
                var data = await _repo.GetBanks();
                
                return Ok(new {
                    code = 200,
                    data
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("upload")]
        public async Task<ActionResult<IEnumerable<QuestionBank>>> UploadQuestions([FromForm] UploadQuestionDto payload) {
            try {
                if (payload.Excel == null || payload.Excel.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }
                // Check if the uploaded file is an Excel file
                if (Path.GetExtension(payload.Excel.FileName).ToLower() != ".xlsx")
                {
                    return BadRequest("Invalid file format. Please upload a valid Excel file.");
                }
                Console.WriteLine(payload.Class);
                Console.WriteLine(payload.Subject);
                Console.WriteLine(payload.Excel);
                // await _repo.ParseQuestionFromExcell(payload.Excel, payload.Subject, payload.Class);

                return Ok(new {
                    code = 200,
                    message = "Uploaded Successfully"
                });

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }
        [HttpPost("exam/single")]
        public async Task<ActionResult<IEnumerable<QuestionBank>>> GetSingleExamQuestion(ExamQuestionsDTO payload) {
            try {
                var data = await _repo.GetExamQuestions(payload.Subject, payload.Class);

                var currentQuestion = data.ElementAt((int)payload.Index);

                currentQuestion.Options = (List<Option>?)await _repo.GetExamOptions(currentQuestion.Id);
                
                return Ok(new {
                    code = 200,
                    data = currentQuestion
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("draft/save")]
        public async Task<ActionResult> SaveDraft(Draft payload) {
            try {
                var draft = await _repo.GetDraft(payload.Id);
                if(draft != null) {
                    await _repo.UpdateDraft(payload);
                } else {
                    await _repo.SaveDraft(payload);
                }
                
                return Ok(new {
                    code = 200,
                    message = "Draft saved"
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("draft/{id}")]
        public async Task<ActionResult> GetDraft(string id) {
            try {
                var data = await _repo.GetDraft(id);
                
                return Ok(new {
                    code = 200,
                    data
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("draft/update")]
        public async Task<ActionResult> UpdateDraft(Draft payload) {
            try {
                await _repo.UpdateDraft(payload);
                
                return Ok(new {
                    code = 200,
                    message = "Draft saved"
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("question/single")]
        public async Task<ActionResult<IEnumerable<QuestionBank>>> GetQuestionBySubject(ExamQuestionsDTO payload) {
            try {
                QuestionModel question = await _repo.GetExamQuestion(payload.Subject, payload.Class);

                question.Options = (List<Option>?)await _repo.GetExamOptions(question.Id);
                
                return Ok(new {
                    code = 200,
                    data = question
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("exam")]
        public async Task<ActionResult<IEnumerable<QuestionBank>>> GetExamQuestions(ExamQuestionsDTO payload) {
            try {
                var data = await _repo.GetExamQuestions(payload.Subject, payload.Class);

                if(data.Any()) {
                    foreach(QuestionModel question in data) {
                        question.Options = (List<Option>?)await _repo.GetExamOptions(question.Id);
                    }
                }
                return Ok(new {
                    code = 200,
                    data
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }
        [HttpPost("Submit")]
        public async Task<ActionResult<IEnumerable<QuestionBank>>> SubmitAnswers(AnswersDto payload) {
            try {
                foreach(AnswerDto answer in payload.Answers) {
                var ans = await _repo.GetAnswer(answer.Question);
                if(ans.Answer == answer.Value) {
                        await _repo.MarkScore(payload.Id, answer.Subject, payload.Class);
                }     
                }
                foreach(AnswerDto answer in payload.Answers) {
                    await _repo.AddAttemptedQuestion(payload.Id, answer.Subject);
                }
                await _repo.CompleteExam(payload.Id);
                return Ok(new {
                    code = 200,
                    message = "Exam completed successfully"
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, e.Message);
            }
        }
    }
}