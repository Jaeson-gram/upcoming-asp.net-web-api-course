using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI2.Data;
using WebAPI2.Models;

namespace WebAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        //dependency injection testing
        // private readonly ILog _log;
        //
        // public StudentController(ILog log)
        // {
        //     _log = log;
        // }
        
        private readonly ILogger<StudentController> _logger;
        private readonly SchoolDbContext _dbContext;
        private readonly IMapper _mapper;

        public StudentController(ILogger<StudentController> logger, SchoolDbContext dbContext, IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
        {
            _logger.LogInformation("GetAllStudents() called!");

            //dependency injection testing
            // _log.Log("all students");
            _logger.LogInformation("retrieving all students...");

            //in ef core we can easily fetch the whole table like so...
            var students = await _dbContext.Students.ToListAsync();
            _logger.LogInformation("All students retrieved!");

            // use automapper instead
            // and only use dtos when we want to return a modified version of our students...
            // perhaps one where date of birth is not necessary or something is edited, evaluated or added
            
            // var students = _dbContext.Students.Select(s => new StudentDTO();
            // {
            //     Id = s.Id,
            //     Name = s.Name,
            //     Email = s.Email,
            //     Address = s.Address,
            //     DateOfBirth = s.DateOfBirth
            // }); //or add a .ToList();
            
            _logger.LogInformation("mapping students' data to data transfer object...");

            var studentDtoData = _mapper.Map<IEnumerable<StudentDTO>>(students);
            
            _logger.LogInformation("completed!");

            return Ok(studentDtoData);
        }

        [HttpGet("{id:int}", Name = "GetStudentByIdAsync")]
        //without the constraints above we will have an exception about our req hitting multiple endpoints because the GetStudentByName is also the same endpoint and the webapi needs to know which one to hit
        //besides, they also define the path visibly on uri
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> GetStudentByIdAsync(int id)
        {
            _logger.LogInformation("GetStudentsByIdAsync() called!");

            if (id < 0)
            {
                // _logger.LogWarning("bad request");
                return BadRequest(); //400 Bad Request
            }
            
            _logger.LogInformation("matching ID");

            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                // _logger.LogError("Student not found!");
                // _logger.LogDebug("bla bla bla"); --> what's the difference btw this and using only 
                return NotFound(); //404 Not Found
            }
            
            _logger.LogInformation("ID matched!");

            // use automapper instead
            // var studentDTO = new StudentDTO
            // {
            //     Id = student.Id,
            //     Name = student.Name,
            //     Email = student.Email,
            //     Address = student.Address,
            //     DateOfBirth = student.DateOfBirth
            //     //then any other property in the dto, like a party ticket number or anything not in the Student obj we want to add - the reason for dtos
            // };
            
            _logger.LogInformation("mapping student data to data transfer object...");

            var studentDtoData = _mapper.Map<StudentDTO>(student);
            _logger.LogInformation("student data mapping completed!");

            return Ok(studentDtoData);
            // return SchoolRepo.Students.Where(s => s.Id == id).FirstOrDefault();

            //same thing!
        }

        [HttpGet("{name:alpha}")]
        //without the constraints we will have an exception about our req hitting multiple endpoints because the GetStudentById is also the same endpoint and the webapi needs to know which one to hit
        //making the GetStudentById accept string clears this error, but also allows this action to search with even ids. and we must separate concerns
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> GetStudentByNameAsync(string name)
        {
            _logger.LogInformation("GetStudentsByNameAsync() called!");

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("name cannot be empty"); //400 Bad Request
            }

            _logger.LogInformation("matching ID");
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Name == name);

            if (student == null)
            {
                return NotFound($"no student with the name {name} was found"); //404 Not Found
            }
            
            _logger.LogInformation("ID found!");

            // use automapper instead
            
            // var studentDTO = new StudentDTO
            // {
            //     Id = student.Id,
            //     Name = student.Name,
            //     Email = student.Email,
            //     Address = student.Address,
            //     DateOfBirth = student.DateOfBirth
            //     //then any other property in the dto, like a party ticket number or anything not in the Student obj we want to add - the reason for dtos
            // };
            
            _logger.LogInformation("mapping student data to data transfer object...");
            var studentDtoData = _mapper.Map<StudentDTO>(student);
            _logger.LogInformation("student data mapping completed!");

            
            return Ok(studentDtoData);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteStudentByIdAsync(int id)
        {
            _logger.LogInformation("DeleteStudentByIdAsync() called!");

            if (id < 0)
            {
                return BadRequest("wrong id"); //400 Bad Request
            }

            _logger.LogInformation("matching ID");

            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound($"no student with the id {id} was found"); //404 Not Found
            }
            _logger.LogInformation("ID found!");

            _dbContext.Students.Remove(student);
            
            await _dbContext.SaveChangesAsync();

            return Ok(true);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> RegisterStudentAsync([FromBody] StudentDTO studentModel)
        {
            _logger.LogInformation("RegisterStudentAsync() called!");

            // the [APIController] automatically checks this out manages this option for us
            // if (!ModelState.IsValid)
            // {
            //     return BadRequest("invalid student model");
            // }

            // var newId = _dbContext.Students.Count() + 1;

            //using model validations so this is unnecessary.

            // if (string.IsNullOrEmpty(studentModel.Name) || string.IsNullOrEmpty(studentModel.Email) || string.IsNullOrEmpty(studentModel.Address))
            // {
            //     return BadRequest("All parameters 'name', 'email', and 'address' must be present");
            // }

            // CUSTOM VALIDATIONS
            // 1. directly adding model error to Model State
            // if (studentModel.AdmissionDate < DateTime.Now)
            // {
            //     ModelState.AddModelError("Admission Error", "We only accept students admitted after yesterday");
            // }

            //2. using custom attribute in models/validators
            
            
            // use automapper instead
            // var student = new Student()
            // {
            //     Id = newId,
            //     Address = studentModel.Address,
            //     Email = studentModel.Email,
            //     Name = studentModel.Name,
            //     DateOfBirth = studentModel.DateOfBirth
            //     //can we add a new property to Student from here? if we can't, should they? is it necessary? 
            // };
            
            _logger.LogInformation("mapping data transfer object to student data...");

            var student = _mapper.Map<Student>(studentModel);
            
            _logger.LogInformation("student data transfer object mapping completed!");


            _logger.LogInformation("adding and saving student to database ...");

            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();

            studentModel.Id = student.Id;
            
            _logger.LogInformation("student added and saved to database!");
            
            _logger.LogInformation("creating student route...");
            _logger.LogInformation("student route created!");

            return CreatedAtRoute("GetStudentByIdAsync", new { id = student.Id }, student);
        }

        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> UpdateStudentAsync([FromBody] StudentDTO student, int studentId)
        {
            if (studentId < 0)
            {
                return BadRequest();
            }

            var studentToPatch = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            
            // var studentToPatch2 = _dbContext.Students.AsNoTracking().FirstOrDefault(s => s.Id == studentId);
            
            
            if (studentToPatch == null)
            {
                return NotFound($"No student with the id {studentId} was found");
            }
            
            // {
            // we can create a new student instance to update our column with... unnecessary, but to explain that-->
            // because Id is PK it will throw an exception,unless we use the AsNoTracking() property while fetching the student in the commented code above

            // var newRecord = new Student()
            // {
            //     Id = studentToPatch.Id, => this line will cause the error without the AsNoTracking()
            //     Name = student.Name,
            //     Address = student.Address,
            //     Email = student.Email,
            //     DateOfBirth = student.DateOfBirth
            // };
            //
            // _dbContext.Students.Update(newRecord);
            
            //just another way to update.
            // }
            
            
            // use automapper
            // studentToPatch.Name = student.Name;
            // studentToPatch.Email = student.Email;
            // studentToPatch.Address = student.Address;
            // studentToPatch.DateOfBirth = student.DateOfBirth;
            
            studentToPatch = _mapper.Map<Student>(student);

            // _dbContext.SaveChanges(); we use the patch method
            return Ok(studentToPatch);

        }
        
        [HttpPatch("{id:int}/updatePartial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> UpdateStudentPartialAsync([FromBody] JsonPatchDocument<StudentDTO>? patchDocument, int id)
        {
            _logger.LogInformation("UpdateStudentPartialAsync() called!");

            if (patchDocument is null  ||  id < 0)
            {
                return BadRequest();
            }
            
            //fetch the student to patch
            
            _logger.LogInformation("matching ID...");
            var studentToPatch = await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        
            if (studentToPatch == null)
            {
                return NotFound($"No student with the id {id} was found");
            }

            // use automapper to make the dto model
            // var studentDto = new StudentDTO
            // {
            //     Id = studentToPatch.Id,
            //     Name = studentToPatch.Name,
            //     Email = studentToPatch.Email,
            //     Address = studentToPatch.Address,
            //     DateOfBirth = studentToPatch.DateOfBirth
            // };
            _logger.LogInformation("ID found!");
            
            _logger.LogInformation("mapping student data to data transfer object...");

            var studentDto = _mapper.Map<StudentDTO>(studentToPatch); 
            
            _logger.LogInformation("applying patch to student model...");

            //apply the patch we receive from the json patch to the model
            patchDocument.ApplyTo(studentDto, ModelState);

            //validate the model
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }
            
            // return (ModelState.IsValid ? Ok(studentToPatch) : BadRequest(ModelState));
            
            
            // use automapper to update the student with the data from the model
            //
            // studentToPatch.Name = studentDto.Name;
            // studentToPatch.Email = studentDto.Email;
            // studentToPatch.Address = studentDto.Address;
            // studentToPatch.DateOfBirth = studentDto.DateOfBirth;
            
            _logger.LogInformation("mapping data transfer object back to student data...");

            studentToPatch = _mapper.Map<Student>(studentDto);
            
            
            _logger.LogInformation("updating database with new student data");

            _dbContext.Students.Update(studentToPatch); //otherwise it'll not update it.
            
            _logger.LogInformation("saving changes...");

            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation("completed");

            //return ok.
            return Ok(studentToPatch);

        }

    }
}

