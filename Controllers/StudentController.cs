using AutoMapper;
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
    [EnableCors(PolicyName = "LocalHost")]
    // enabling cors so only origins allowed in the named policy 'LocalHost' will have access here. this will override the middleware policy
    // [EnableCors(PolicyName = "LocalHost")] -- you can also configure policy for individual endpoints, which overrides the controller policy
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // [ DisableCors] -- disabling cors for a particular endpoint revokes the controller cors policy

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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // [EnableCors(PolicyName = "LocalHost")] -- using custom cors policy for a particular endpoint overrides the controller policy for that endpoint
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

