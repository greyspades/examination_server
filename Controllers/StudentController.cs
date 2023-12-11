using Microsoft.AspNetCore.Mvc;
using Student.Interface;
using Student.Models;
using BC = BCrypt.Net.BCrypt;

namespace Student.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {

        private readonly ILogger<StudentController> _logger;
        private IStudentRepository _repo;
        Guid guid = Guid.NewGuid();

        public StudentController(ILogger<StudentController> logger, IStudentRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpPost("register")]
                public async Task<ActionResult> Register(StudentAuth payload) {
            try {
                var duplicate = await _repo.CheckRegistration(payload);
                if(duplicate.Any()) {
                    return BadRequest( new {
                        code = 400,
                        message = "You have already registered on the portal",
                    });
                } else {
                    payload.Password = BC.HashPassword(payload.Password);

                    await _repo.RegisterUser(payload);

                    return Ok(new {
                        code = 200,
                        message = "Registered Successfully"
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

         [HttpPost("login")]
            public async Task<ActionResult> Login(StudentAuth payload) {
            try {
                var user = await _repo.GetStudentByEmail(payload.Email);
                var password = await _repo.GetPassword(user.First().Id);
                Console.WriteLine(payload.Password);
                if(!user.Any()) {
                    return BadRequest( new {
                        code = 400,
                        message = "This user does not exist on the portal",
                    });
                } else if(user.Any()
                 && BC.Verify(payload.Password, password.First()) == true
                 ) {

                    return Ok(new {
                        code = 200,
                        message = "Loged in successfully"
                    });
                } else {
                    Console.WriteLine(password.First());
                    Console.WriteLine(user.First().Id);
                    return BadRequest(new {
                        code = 500,
                        message = "An error occured processing your request"
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

        [HttpPost]
        public async Task<ActionResult> Onboard([FromForm] StudentDto payload) {
            try {
                if(payload.Scope == "DOCUMENTS") {
                    if(payload.Documents != null) {
                        await _repo.SaveDocument(payload.Documents.Docs);

                    return Ok(new {
                        code = 200,
                        message = "Documents added successfully"
                    });
                    } else {
                        return Ok(new {
                        code = 400,
                        message = "Failed upload"
                    });
                    }
                } else {
                    return Ok(new {
                        code = 400,
                        message = "Invalid scope provided"
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


        [HttpPost("documents")]
        public async Task<ActionResult> SaveDocuments(IFormCollection payload) {
            try {
                if(payload != null) {
                    await _repo.SaveDocument(payload);
                    
                    return Ok(new {
                        code = 200,
                        message = "Documents added successfully"
                    });
                    } else {
                        return Ok(new {
                        code = 400,
                        message = "Failed upload"
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

        [HttpPost("document")]
        public async Task<ActionResult> SaveDocument([FromForm] IFormFile payload) {
            try {
                Console.WriteLine("got to documents");
                Console.WriteLine(payload.Name);
                // Console.WriteLine(payload.First().Name);
                var files = HttpContext.Request.Form.Files;
                Console.WriteLine(files.Count);
                Console.WriteLine(files[0].Name);
                if(payload != null) {
                    return Ok(new {
                        code = 200,
                        message = "Documents added successfully"
                    });
                    } else {
                        return Ok(new {
                        code = 400,
                        message = "Failed upload"
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

        [HttpPost("upload/students")]
        public async Task<ActionResult> UploadStudents([FromForm] IFormFile file) {
            try {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Check if the uploaded file is an Excel file
                if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
                {
                    return BadRequest("Invalid file format. Please upload a valid Excel file.");
                }

                await _repo.ParseExcell(file);

                return Ok( new {
                    code = 200,
                    message = "Successfully uploaded candidates",
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

        [HttpGet("students/all/{page}/{take}")]
        public async Task<ActionResult> GetAllStudents(int page, int take) {
            try {
                var data = await _repo.GetStudents();

                var count = page * 10;

                var slicedList = data.Skip((int)count).Take(count: take);

                return Ok( new {
                    code = 200,
                    message = "Successfully uploaded students",
                    data = slicedList
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

        [HttpGet("search/{field}/{value}")]
        public async Task<ActionResult> SearchStudents(string field, string value) {
            try {
                var data = await _repo.GetStudents();

                var results = new List<StudentData>();

                if(field == "firstname") {
                    results = data.Where((item) => item.FirstName.ToLower().Contains(value.ToLower())).ToList<StudentData>();
                } else if(field == "lastname") {
                    results = data.Where((item) => item.LastName.ToLower().Contains(value.ToLower())).ToList<StudentData>();
                } else if(field == "branch") {
                     results = data.Where((item) => item.Branch.ToLower().Contains(value.ToLower())).ToList<StudentData>();
                }

                return Ok( new {
                    code = 200,
                    message = "Successfully uploaded students",
                    data = results
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

        [HttpGet("student/{id}")]
        public async Task<ActionResult> GetStudentById(string id) {
            try {
                var data = await _repo.GetStudentById(id);

                if(data.Any()) {
                    return Ok( new {
                    code = 200,
                    message = "Valid Id",
                });
                } else {
                    return Ok( new {
                    code = 404,
                    message = "The Id entered is invalid, please check the Id and try again",
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

        [HttpPost("personal")]
        public async Task<ActionResult> AddPersonalInfo(BasicInfo payload) {
            try {
                var data = await _repo.GetPersonalInfo(payload.Id);
                if(data.Any()) {
                    await _repo.UpdatePersonalInfo(payload);

                    return Ok(new {
                        code = 200,
                        message = "Successfully updated personal info"
                    });
                } else {
                    await _repo.SavePersonalInfo(payload);

                    return Ok(new {
                        code = 200,
                        message = "Successfully saved personal info"
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

        [HttpPost("parents")]
        public async Task<ActionResult> AddParentsInfo(ParentsInfo payload) {
            try {
                var data = await _repo.GetParentsInfo(payload.Id);
                if(data.Any()) {
                    await _repo.UpdateParentsInfo(payload);

                    return Ok(new {
                        code = 200,
                        message = "Successfully updated personal info"
                    });
                } else {
                    await _repo.SaveParentsInfo(payload);

                    return Ok(new {
                        code = 200,
                        message = "Successfully saved personal info"
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

        [HttpPost("education")]
        public async Task<ActionResult> AddEducationInfo(EducationDto payload) {
            try {
                var data = await _repo.GetEducationInfo(payload.Data.First().Id);
                if(data.Any()) {
                    await _repo.DeleteEducationInfo(payload.Data.First().Id);

                    foreach(EducationInfo education in payload.Data) {
                        await _repo.SaveEducationInfo(education);
                    }

                    return Ok(new {
                        code = 200,
                        message = "Successfully updated educational info"
                    });
                } else {
                    foreach(EducationInfo education in payload.Data) {
                        await _repo.SaveEducationInfo(education);
                    }

                    return Ok(new {
                        code = 200,
                        message = "Successfully saved educational info"
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

        [HttpPost("banking")]
        public async Task<ActionResult> AddBankingInfo(BankingInfo payload) {
            try {
                var data = await _repo.GetBankingInfo(payload.Id);
                if(data.Any()) {
                    await _repo.UpdateBankingInfo(payload);

                    return Ok(new {
                        code = 200,
                        message = "Successfully updated personal info"
                    });
                } else {
                    await _repo.SaveBankingInfo(payload);

                    return Ok(new {
                        code = 200,
                        message = "Successfully saved personal info"
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
        [HttpGet("pending/{page}/{take}")]
        public async Task<ActionResult<IEnumerable<StudentData>>> GetPendingVerification(int page, int take) {
            try {
                var data = await _repo.GetPendingVerifications();

                var count = page * 10;

                var slicedList = data.Skip((int)count).Take(count: take);

                return Ok( new {
                    code = 200,
                    message = "Success",
                    data = slicedList
                });

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new ("logs.txt", append: true);

                await outputFile.WriteAsync(e.Message);

                var response = new {
                    code = 500,
                    message = "Unnable to process your request",
                };

                return StatusCode(500, e.Message);
            }
        }
        [HttpGet("passport/{id}")]
        public async Task<ActionResult> GetImage(string id) {
            try {
                var jpgPath = @$"documents/students/{id}.jpg";
                var jpegPath = @$"documents/students/{id}.jpeg";
                var pngPath = @$"documents/students/{id}.png";

                if (System.IO.File.Exists(jpgPath))
                {
                    var fileStream = new FileStream(jpgPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                    return File(fileStream, "image/jpeg");
                } else if (System.IO.File.Exists(jpegPath))
                {
                    var fileStream = new FileStream(jpegPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                    return File(fileStream, "image/jpeg");
                } else if (System.IO.File.Exists(pngPath))
                {
                    var fileStream = new FileStream(pngPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                    return File(fileStream, "image/png");
                }
                else
                {
                    Console.WriteLine("File not found.");
                    return NotFound("File not found");
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
                using StreamWriter outputFile = new("logs.txt", true);

                await outputFile.WriteAsync(e.Message);

                var response = new {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, response);
            }
        }

        [HttpGet("info/{field}/{id}")]
        public async Task<ActionResult<IEnumerable<StudentData>>> GetStudentInfo(string field, string id) {
            try {
                if(field == "PERSONAL") {
                    var response = await _repo.GetPersonalInfo(id);
                    var data = response.First();
                    return Ok( new {
                    code = 200,
                    message = "Success",
                    data
                });
                } else if(field == "PARENTS") {
                    var response = await _repo.GetParentsInfo(id);
                    var data = response.First();
                    return Ok( new {
                    code = 200,
                    message = "Success",
                    data
                });
                } else if(field == "BANKING") {
                    var response = await _repo.GetBankingInfo(id);
                    var data = response.First();
                    return Ok( new {
                    code = 200,
                    message = "Success",
                    data
                });
                } else if(field == "DOCUMENTS") {
                    var response = await _repo.GetDocuments(id);
                    var data = response.First();
                    return Ok( new {
                    code = 200,
                    message = "Success",
                    data
                });
                } else {
                    return BadRequest( new {
                    code = 400,
                    message = "Unnable to process your request",
                });
                }

            } catch(Exception e) {
                Console.WriteLine(e.Message);

                using StreamWriter outputFile = new ("logs.txt", append: true);

                await outputFile.WriteAsync(e.Message);

                var response = new {
                    code = 500,
                    message = "Unnable to process your request",
                };

                return StatusCode(500, e.Message);
            }
        }
    }
}