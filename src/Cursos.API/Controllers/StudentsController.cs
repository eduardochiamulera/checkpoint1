using Microsoft.AspNetCore.Mvc;

namespace Cursos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    // TODO: Implement student endpoints using MediatR
    // - GET /api/students
    // - GET /api/students/{id}
    // - POST /api/students
    // - PUT /api/students/{id}
    // - DELETE /api/students/{id}
    
    [HttpGet]
    public ActionResult GetAll()
    {
        return Ok(new { message = "Students endpoint - TODO" });
    }
}
