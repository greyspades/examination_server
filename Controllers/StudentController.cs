using Microsoft.AspNetCore.Mvc;
using Student.Interface;
using Student.Models;
using BC = BCrypt.Net.BCrypt;
using Helpers.General;
using Microsoft.AspNetCore.Cors;
using Aspose.Pdf;
using PDFHelpers;
using Credentials.Models;
using Newtonsoft.Json;
using HTML;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Student.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {

        private readonly ILogger<StudentController> _logger;
        private IStudentRepository _repo;
        private IWebHostEnvironment _environment;
        Guid guid = Guid.NewGuid();

        public StudentController(ILogger<StudentController> logger, IStudentRepository repo, IWebHostEnvironment environment)
        {
            _logger = logger;
            _repo = repo;
            _environment = environment;
        }
        [HttpPost("test")]
        public async Task<ActionResult> Test() {
            try {
                
                return Ok("testes");
                
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

        [HttpPost("register")]
                public async Task<ActionResult> Register([FromBody] StudentAuth payload) {
            try {
                var duplicate = await _repo.CheckRegistration(payload);
                var deadline = await _repo.GetSettings();
                Console.WriteLine(deadline.Deadline);
                if(deadline.Deadline < DateTime.Now) {
                    return Ok( new {
                        code = 400,
                        message = "Application deadline has been exceeded",
                    });
                }
                if(duplicate.Any()) {
                    return Ok( new {
                        code = 400,
                        message = "You have already registered on the portal",
                    });
                } else {
                    payload.Password = BC.HashPassword(payload.Password.Trim());

                    await _repo.RegisterUser(payload);

                    var claims = new[]
            {
                new Claim(ClaimTypes.Sid, payload.Id),
                // Add additional claims as needed
            };
            // Create a JWT token
            var token = new JwtSecurityToken(
                issuer: "your_issuer",
                audience: "your_audience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expiration time
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_ludex")), SecurityAlgorithms.HmacSha256)
            );

               return Ok(new {
                    code = 200,
                    message = "Registered successfully",
                    token = new JwtSecurityTokenHandler().WriteToken(token)
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
                var deadline = await _repo.GetSettings();
    //   Console.WriteLine("user id is");
    //             Console.WriteLine(user.First().Id);
    //             Console.WriteLine("password is");
    //             Console.WriteLine(password);

    //             return Ok(new {
    //                 code = 200,
    //                 message = "did thing"
    //             });
                if(deadline.Deadline < DateTime.Now) {
                    return Ok( new {
                        code = 400,
                        message = "Application deadline has been exceeded",
                    });
                }
                if(!user.Any()) {
                    return Ok( new {
                        code = 400,
                        message = "This user does not exist on the portal",
                    });
                } else if(user.Any()
                 && BC.Verify(payload.Password, password) == true
                 ) {
                    var claims = new[]
            {
                new Claim(ClaimTypes.Sid, payload.Id),
                // Add additional claims as needed
            };
            // Create a JWT token
            var token = new JwtSecurityToken(
                issuer: "your_issuer",
                audience: "your_audience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expiration time
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_ludex")), SecurityAlgorithms.HmacSha256)
            );

               return Ok(new {
                    code = 200,
                    message = "Logged in successfully",
                    data = user.First(),
                    token = new JwtSecurityTokenHandler().WriteToken(token)
               });
                } else {
                    return Ok(new {
                        code = 401,
                        message = "Wrong Email/Password"
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
        // [Authorize]
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
                await _repo.SaveExcelFile(file);

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
        // [Authorize]
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
        // [Authorize]
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
        // [Authorize]
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
        // [Authorize]
        public async Task<ActionResult> AddBankingInfo(BankingInfo payload) {
            try {
                var data = await _repo.GetBankingInfo(payload.Id);
                CredentialsObj cred = await _repo.GetCredentials();
                var details = await _repo.GetAccountInfo(payload.MpkAccNo, cred);
                if(data.Any()) {
                    await _repo.UpdateBankingInfo(payload);

                    return Ok(new {
                        code = 200,
                        message = "Successfully updated personal info"
                    });
                } else if(!data.Any()) {
                    await _repo.SaveBankingInfo(payload);

                    return Ok(new {
                        code = 200,
                        message = "Successfully saved personal info"
                    });
                } 
                else if(details == null) {
                    return Ok(new {
                        code = 404,
                        message = "Invalid Account number provided"
                    });
                }
                 else {
                    return Ok(new {
                        code = 404,
                        message = "Couldnt process your request"
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
                Console.WriteLine(field);
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
                    var data = await _repo.GetDocuments(id);
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

        [HttpGet("search/applicants/{field}/{value}")]
        public async Task<ActionResult> SearchApplicants(string field, string value) {
            try {
                var data = await _repo.GetPendingVerifications();

                var results = new List<StudentData>();

                if(field == "FIRSTNAME") {
                    results = data.Where((item) => item.FirstName.ToLower().Contains(value.ToLower())).ToList<StudentData>();
                } else if(field == "LASTNAME") {
                    results = data.Where((item) => item.LastName.ToLower().Contains(value.ToLower())).ToList<StudentData>();
                } else if(field == "BRANCH") {
                    results = data.Where((item) => item.Branch.ToLower().Contains(value.ToLower())).ToList<StudentData>();
                } else if(field == "ZONE") {
                    results = data.Where((item) => item.Zone.ToLower().Contains(value.ToLower())).ToList<StudentData>();
                }

                return Ok( new {
                    code = 200,
                    message = "Successfull",
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
        [HttpGet("info/{id}")]
        public async Task<ActionResult> GetAllInfo(string id) {
            try {
                var personal = await _repo.GetPersonalInfo(id);
                var parents = await _repo.GetParentsInfo(id);
                var banking = await _repo.GetBankingInfo(id);
                var education = await _repo.GetEducationInfo(id);
                var documents = await _repo.GetDocuments(id);

                return Ok(new {
                    code = 200,
                    message = "Successfull",
                    data = new {
                        personal = personal.First(),
                        parents = parents.First(),
                        banking = banking.First(),
                        education,
                        documents = documents
                    }
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

    [HttpGet("doc/{path}/{extension}")]
    // [EnableCors("XPolicy")]
    public ActionResult<dynamic> DownloadFile(string path, string extension)
    {
        // Check if the file exists
        var filePath = @$"documents/students/{path}{extension}";
        if (System.IO.File.Exists(filePath))
        {
            if(extension == ".pdf") {
                var data = PdfToHtmlConverter.ConvertPdfToHtml(filePath);
                return Ok(data);
            }
            else {
                var content = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(content, "application/octet-stream");
            }
        }
        else {
            Console.WriteLine("not found");
            return NotFound();
        }
    }
     [HttpPost("Approve")]
        public async Task<ActionResult> ApproveClient(IdDTO payload) {
            try {
                await _repo.ApproveClient(payload.Id);
                return Ok(new {
                    code = 200,
                    message = "Successfully Approved client",
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

        [HttpPost("Reject")]
        public async Task<ActionResult> DeclineClient(DeclineDto payload) {
            try {
                await _repo.RejectClient(payload.Id, payload.Reason);
                var data = await _repo.GetPersonalInfo(payload.Id);
                CredentialsObj cred = await _repo.GetCredentials();
                var mailObj = new EmailDto
                    {
                        emailaddress = data.First().Email,
                        subject = "Decline of scholarship application",
                        hasFile = "No",
                        body = HTMLHelper.Reject(data.First().FirstName, payload.Reason),
                    };
                // await _repo.SendMail(mailObj, cred);
                await _repo.UpdateStatus(payload.Id, "DECLINED");

                return Ok(new {
                    code = 200,
                    message = "Successfully Declined client",
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

        [HttpPost("pushBack")]
        public async Task<ActionResult> PushBack(DeclineDto payload) {
            try {
                var data = await _repo.GetPersonalInfo(payload.Id);
                CredentialsObj cred = await _repo.GetCredentials();
                var mailObj = new EmailDto
                    {
                        emailaddress = data.First().Email,
                        subject = "Notice on your scholarship application",
                        hasFile = "No",
                        body = HTMLHelper.PushBack(data.First().FirstName, payload.Reason),
                    };
                await _repo.UpdateStatus(payload.Id, "PUSHED_BACK");
                // await _repo.SendMail(mailObj, cred);
                return Ok(new {
                    code = 200,
                    message = "Successfully Pushed back client",
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

        [HttpGet("candidates")]
        public async Task<ActionResult> GetCandidates() {
            try {
               var data = await _repo.GetCandidates();

                return Ok(new {
                    code = 200,
                    message = "Successfull",
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

        [HttpGet("application/{id}")]
        public async Task<ActionResult> GetApplicationInfo(string id) {
            try {
               var data = await _repo.GetApplicationInfo(id);

                return Ok(new {
                    code = 200,
                    message = "Successfull",
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

        [HttpGet("/candidate/search/{field}/{value}")]
        public async Task<ActionResult> SearchCandidates(string field, string value) {
            try {
                var data = await _repo.GetCandidates();

                var results = new List<dynamic>();

                if(field == "firstname") {
                    results = data.Where((item) => item.firstName.ToLower().Contains(value.ToLower())).ToList<dynamic>();
                } 
                else if(field == "lastname") {
                    results = data.Where((item) => item.lastName.ToLower().Contains(value.ToLower())).ToList<dynamic>();
                } else if(field == "branch") {
                     results = data.Where((item) => item.branch.ToLower().Contains(value.ToLower())).ToList<dynamic>();
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

        [HttpPost("exam/status")]
        // [Authorize]
        public async Task<ActionResult<IEnumerable<ExamDetails>>> GetExamDetails(IdDTO payload) {
            try {
                var data = await _repo.GetExamDetails(payload.Id);
                if(data.Any()) {
                    return Ok( new {
                    code = 200,
                    message = "Approved",
                    data = data.First()
                });
                }
                else {
                    return Ok( new {
                    code = 400,
                    message = "Not Approved",
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

        [HttpGet("excell/students")]
        public async Task<ActionResult<IEnumerable<ExamDetails>>> GetExcellFiles() {
            try {
                string folderPath = @"documents/excel";
                string[] files = Directory.GetFiles(folderPath);

                return Ok( new {
                    code = 200,
                    data = files,
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

        [HttpGet("scores/{id}")]
        public async Task<ActionResult<IEnumerable<ExamDetails>>> GetScores(string id) {
            try {
                var data = await _repo.GetScoresById(id);

                return Ok( new {
                    code = 200,
                    data,
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

        [HttpGet("metrics")]
        public async Task<ActionResult<IEnumerable<ExamDetails>>> GetMetrics() {
            try {
                var data = await _repo.GetMetrics();

                return Ok( new {
                    code = 200,
                    data,
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

        [HttpGet("scores/all/{scope}")]
        public async Task<ActionResult<IEnumerable<ExamDetails>>> GetAllScores(string scope) {
            try {
                var data = await _repo.GetAllScores(scope);

                return Ok( new {
                    code = 200,
                    data,
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

        [HttpGet("dashboard/{id}")]
        // [Authorize]
        public async Task<ActionResult<IEnumerable<ExamDetails>>> GetDashboardDetails(string id) {
            try {
                var data = await _repo.GetDashboardDetails(id);
                if(data.Any()) {
                    // Console.WriteLine(data.First());
                    var response = new {
                        firstName = data.First().firstname,
                        lastName = data.First().lastname,
                        school = data.First().school,
                        scores = new List<dynamic>() {}
                    };
                    foreach(dynamic item in data) {
                        var items = new {
                            id = item.subject,
                            name = item.name,
                            score = item.score,
                            attempted = item.questionsAttempted
                        };
                        response.scores.Add(items);
                    }
                    return Ok( new {
                    code = 200,
                    message = "Successfull",
                    data = response
                });
                }
                else {
                    return Ok( new {
                    code = 400,
                    message = "Not Found",
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

        [HttpPost("validate/mpk")]
        // [Authorize]
        public async Task<ActionResult<IEnumerable<ExamDetails>>> ValidateUserDetails(IdDTO payload) {
            try {
                CredentialsObj cred = await _repo.GetCredentials();
                var data = await _repo.GetAccountInfo(payload.Id, cred);
                if(data != null) {
                    return Ok( new {
                        code = 200,
                        message = "Valid"
                    });
                } else {
                    return Ok( new {
                        code = 404,
                        message = "InValid"
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

        [HttpPost("email")]
        public async Task<ActionResult<IEnumerable<ExamDetails>>> SendEmail(IdDTO payload) {
            try {
                CredentialsObj cred = await _repo.GetCredentials();
                var studentData = await _repo.GetStudentById(payload.Id);
                var student = studentData.First();
                var mailObj = new EmailDto
                    {
                        emailaddress = student.Email,
                        subject = "Please confirm your email address",
                        hasFile = "No",
                        body = HTMLHelper.VerifyEmail(student.FirstName, payload.Id),
                    };

                    // var mail = await _repo.SendMail(mailObj, cred);

                    return Ok( new {
                        code = 200,
                        message = "Mail delivered"
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
    }
}