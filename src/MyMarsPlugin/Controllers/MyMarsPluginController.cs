using System.Net.Mime;
using Mars.Host.Shared.ExceptionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMarsPlugin.Services;

namespace MyMarsPlugin.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[UserActionResultExceptionFilter]
[NotFoundExceptionFilter]
[FluentValidationExceptionFilter]
[AllExceptionCatchToUserActionResultFilter]
public class MyMarsPluginController : ControllerBase
{
    private readonly MyPluginService _service;

    public MyMarsPluginController(MyPluginService service)
    {
        _service = service;
    }

    [HttpGet("GetValue")]
    public string GetValue(string value = "123")
    {
        return _service.GetValue(value);
    }
}
