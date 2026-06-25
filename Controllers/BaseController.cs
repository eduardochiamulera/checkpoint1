using System.Security.Claims;
using Cursos.Models;
using Cursos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cursos.Controllers;

public abstract class BaseController : ControllerBase
{

    protected string GetUserid()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
