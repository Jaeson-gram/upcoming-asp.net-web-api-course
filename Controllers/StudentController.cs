﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebAPI2.Data;
using WebAPI2.Data.Repository;
using WebAPI2.Models;

namespace WebAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [EnableCors(PolicyName = "LocalHost")]
    [Authorize(AuthenticationSchemes = "LoginForLocal", Roles = "SuperAdmin,Admin")]
    // the auth scheme ensures that only user logged in as 'Local' audience will have access to these endpoints..
    // ie, if a user logs in as 'Hire' or whatever, they will not have access to these endpoints...

    public class StudentController : ControllerBase
    {
     
        private readonly ILogger<StudentController> _logger;
        // private readonly SchoolDbContext _dbContext;
        private readonly IMapper _mapper;
        // private readonly IStudentRepository _studentRepo;
        private readonly ISchoolRepository<Student> _studentRepo;


        public StudentController(ILogger<StudentController> logger, IMapper mapper, ISchoolRepository<Student> studentRepo)
        {
            _logger = logger;
            // _dbContext = dbContext;
            _mapper = mapper;
            _studentRepo = studentRepo;
        }
        
        
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
        {
            _logger.LogInformation("GetAllStudents() called!");

            //in ef core we can easily fetch the whole table like so...
            var students = await _studentRepo.GetAllAsync();
            
            var studentDtoData = _mapper.Map<IEnumerable<StudentDTO>>(students);
            
            return Ok(studentDtoData);
        }

        [HttpGet("{id:int}", Name = "GetStudentByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> GetStudentByIdAsync(int id)
        {
            _logger.LogInformation("GetStudentsByIdAsync() called!");

            if (id < 0)
            {
                return BadRequest(); //400 Bad Request
            }
            
            var student = await _studentRepo.GetAsync(student => student.Id == id, false);

            if (student == null)
            {
                return NotFound(); //404 Not Found
            }

            var studentDtoData = _mapper.Map<StudentDTO>(student);

            return Ok(studentDtoData);
            // return SchoolRepo.Students.Where(s => s.Id == id).FirstOrDefault();

            //same thing!
        }

        [HttpGet("{name:alpha}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

            var student = await _studentRepo.GetAsync(student => student.Name.ToLower().Contains(name.ToLower()), false);

            if (student == null)
            {
                return NotFound($"no student with the name {name} was found"); //404 Not Found
            }
            
            var studentDtoData = _mapper.Map<StudentDTO>(student);
            
            return Ok(studentDtoData);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
            
            var student = await _studentRepo.GetAsync(stu => stu.Id == id, false);

            if (student == null)
            {
                return NotFound($"no student with the id {id} was found"); //404 Not Found
            }
            
            _studentRepo.Delete(student);
            
            return Ok(true);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> RegisterStudentAsync([FromBody] StudentDTO studentModel)
        {
            _logger.LogInformation("RegisterStudentAsync() called!");
            
            var student = _mapper.Map<Student>(studentModel);
            
            await _studentRepo.CreateAsync(student);

            studentModel.Id = student.Id;

            return CreatedAtRoute("GetStudentByIdAsync", new { id = student.Id }, student);
        }

        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> UpdateStudentAsync([FromBody] StudentDTO student, int studentId)
        {
            if (studentId < 0)
            {
                return BadRequest();
            }

            var studentToPatch = await _studentRepo.GetAsync(stu => stu.Id == studentId, true);
            
            // var studentToPatch2 = _dbContext.Students.AsNoTracking().FirstOrDefault(s => s.Id == studentId);
            
            
            if (studentToPatch == null)
            {
                return NotFound($"No student with the id {studentId} was found");
            }
            
            studentToPatch = _mapper.Map<Student>(student);

            await _studentRepo.Update(studentToPatch);

            // _dbContext.SaveChanges(); we use the patch method
            return Ok(studentToPatch);

        }
        
        [HttpPatch("{id:int}/updatePartial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
            var studentToPatch = await _studentRepo.GetAsync(stu => stu.Id == id, true);
        
            if (studentToPatch == null)
            {
                return NotFound($"No student with the id {id} was found");
            }
            
            //map student data
            var studentDto = _mapper.Map<StudentDTO>(studentToPatch); 
            

            //apply the patch we receive from the json patch to the model
            patchDocument.ApplyTo(studentDto, ModelState);

            //validate the model
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }
            
            // return (ModelState.IsValid ? Ok(studentToPatch) : BadRequest(ModelState));
            
            
            studentToPatch = _mapper.Map<Student>(studentDto);
            
            await _studentRepo.Update(studentToPatch); //otherwise it'll not update it.
            
            return Ok(studentToPatch);

        }
    }
}

