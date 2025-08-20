using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assignment6.Models.DTOs;
using Assignment6.Services;

namespace Assignment6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatients([FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _patientService.GetAllAsync(searchTerm, pageNumber, pageSize);
            return Ok(new ApiResponseDto<PagedResultDto<PatientDto>>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Patient not found"
                });
            }

            return Ok(new ApiResponseDto<PatientDto>
            {
                Success = true,
                Data = patient
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto createPatientDto)
        {
            var patientId = await _patientService.CreateAsync(createPatientDto);
            return CreatedAtAction(nameof(GetPatient), new { id = patientId }, new ApiResponseDto<object>
            {
                Success = true,
                Data = new { PatientId = patientId },
                Message = "Patient created successfully"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDto updatePatientDto)
        {
            var result = await _patientService.UpdateAsync(id, updatePatientDto);
            if (!result)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Patient not found"
                });
            }

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Patient updated successfully"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var result = await _patientService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Patient not found"
                });
            }

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Patient deleted successfully"
            });
        }
    }
}
