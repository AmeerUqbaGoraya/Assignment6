// Author: Your Name
// Date: August 19, 2025

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assignment6.Models.DTOs;
using Assignment6.Services;

namespace Assignment6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(new ApiResponseDto<IEnumerable<DoctorDto>>
            {
                Success = true,
                Data = doctors
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Doctor not found"
                });
            }

            return Ok(new ApiResponseDto<DoctorDto>
            {
                Success = true,
                Data = doctor
            });
        }

        [HttpGet("{id}/schedule")]
        public async Task<IActionResult> GetDoctorSchedule(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var schedule = await _doctorService.GetScheduleAsync(id, startDate, endDate);
            return Ok(new ApiResponseDto<IEnumerable<VisitDto>>
            {
                Success = true,
                Data = schedule
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto createDoctorDto)
        {
            var doctorId = await _doctorService.CreateAsync(createDoctorDto);
            return CreatedAtAction(nameof(GetDoctor), new { id = doctorId }, new ApiResponseDto<object>
            {
                Success = true,
                Data = new { DoctorId = doctorId },
                Message = "Doctor created successfully"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDoctorDto)
        {
            var result = await _doctorService.UpdateAsync(id, updateDoctorDto);
            if (!result)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Doctor not found"
                });
            }

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Doctor updated successfully"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var result = await _doctorService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Doctor not found"
                });
            }

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Doctor deleted successfully"
            });
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VisitsController : ControllerBase
    {
        private readonly IVisitService _visitService;

        public VisitsController(IVisitService visitService)
        {
            _visitService = visitService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVisits()
        {
            var visits = await _visitService.GetAllAsync();
            return Ok(new ApiResponseDto<IEnumerable<VisitDto>>
            {
                Success = true,
                Data = visits
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisit(int id)
        {
            var visit = await _visitService.GetByIdAsync(id);
            if (visit == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Visit not found"
                });
            }

            return Ok(new ApiResponseDto<VisitDto>
            {
                Success = true,
                Data = visit
            });
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetVisitsByPatient(int patientId)
        {
            var visits = await _visitService.GetByPatientIdAsync(patientId);
            return Ok(new ApiResponseDto<IEnumerable<VisitDto>>
            {
                Success = true,
                Data = visits
            });
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetVisitsByDoctor(int doctorId)
        {
            var visits = await _visitService.GetByDoctorIdAsync(doctorId);
            return Ok(new ApiResponseDto<IEnumerable<VisitDto>>
            {
                Success = true,
                Data = visits
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<IActionResult> CreateVisit([FromBody] CreateVisitDto createVisitDto)
        {
            var visitId = await _visitService.CreateAsync(createVisitDto);
            return CreatedAtAction(nameof(GetVisit), new { id = visitId }, new ApiResponseDto<object>
            {
                Success = true,
                Data = new { VisitId = visitId },
                Message = "Visit scheduled successfully"
            });
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdateVisitStatus(int id, [FromBody] UpdateVisitStatusDto updateStatusDto)
        {
            var result = await _visitService.UpdateStatusAsync(id, updateStatusDto);
            if (!result)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Visit not found"
                });
            }

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Visit status updated successfully"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelVisit(int id)
        {
            var result = await _visitService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Visit not found"
                });
            }

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Visit cancelled successfully"
            });
        }
    }
}
