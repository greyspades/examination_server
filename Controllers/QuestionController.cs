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
        
    }
}