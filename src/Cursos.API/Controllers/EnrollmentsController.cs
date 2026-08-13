using Microsoft.AspNetCore.Mvc;

namespace Cursos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    // TODO: Implement enrollment endpoints using MediatR
    // - GET /api/enrollments
    // - GET /api/enrollments/student/{studentId}
    // - GET /api/enrollments/course/{courseId}
    // - POST /api/enrollments
    // - PUT /api/enrollments/{id}/complete
    // - PUT /api/enrollments/{id}/cancel
    
    [HttpGet]
    public ActionResult GetAll()
    {
        return Ok(new { message = "Enrollments endpoint - TODO" });
    }
}
