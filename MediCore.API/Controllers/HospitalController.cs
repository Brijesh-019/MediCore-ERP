using MediCore.Application.DTOs;
using MediCore.Application.Interfaces;
using MediCore.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MediCore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HospitalController : ControllerBase
{
    private readonly IHospitalRepository _hospitalRepository;

    public HospitalController(IHospitalRepository hospitalRepository)
    {
        _hospitalRepository = hospitalRepository;
    }

    #region Insert For Hospital Master
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHospitalRequest request)
    {
        var createdIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        long? createdBy = null;

        var result = await _hospitalRepository.CreateAsync(request, createdBy, createdIp);

        if (!result.Success)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Message, 400, new { id = result.Id }));
        }

        return Ok(ApiResponse<object>.Ok(new { id = result.Id }, result.Message));
    }
    #endregion
}