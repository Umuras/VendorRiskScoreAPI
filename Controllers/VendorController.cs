using Microsoft.AspNetCore.Mvc;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Dtos;
using VendorRiskScoreAPI.Services;

namespace VendorRiskScoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorProfileService _vendorProfileService;
        private readonly IRiskAssessmentService _riskAssessmentService;
        private readonly ILogger<VendorController> _logger;

        public VendorController(IVendorProfileService vendorProfileService, IRiskAssessmentService riskAssessmentService, ILogger<VendorController> logger)
        {
            _vendorProfileService = vendorProfileService;
            _riskAssessmentService = riskAssessmentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVendorProfiles()
        {
            _logger.LogInformation("Getting all vendor profiles...");
            List<VendorProfile> vendorProfiles = await _vendorProfileService.GetVendorProfilesAsync();
            List<VendorProfileDto> vendorProfileDtos = vendorProfiles.Select(_vendorProfileService.CreateVendorProfileDto).ToList();
            _logger.LogInformation("All Vendor Profiles got successfully.");
            return Ok(vendorProfileDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorProfileById(int id)
        {
            _logger.LogInformation($"Getting vendor with id: {id}");
            VendorProfile vendorProfile = await _vendorProfileService.GetVendorProfileByIdAsync(id);
            VendorProfileDto vendorProfileDto = _vendorProfileService.CreateVendorProfileDto(vendorProfile);
            _logger.LogInformation($"Vendor {id} retrieved successfully");
            return Ok(vendorProfileDto);
        }

        [HttpGet("{vendorId}/risk")]
        public async Task<IActionResult> GetVendorProfileRiskResult(int vendorId)
        {
            _logger.LogInformation($"Creating VendorProfile Risk Result for this id:{vendorId}");
            VendorProfile vendorProfile = await _vendorProfileService.GetVendorProfileByIdAsync(vendorId);
            RiskAssessmentResponseDto responseDto = await _riskAssessmentService.CreateRiskAssessmentResult(vendorProfile);
            _logger.LogInformation($"Created VendorProfile Risk Result for this id:{vendorId}");
            return Ok(responseDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddVendorProfile([FromBody] VendorProfileDto vendorProfile)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Adding VendorProfile operation is starting...");
            VendorProfile dbvendorProfile = await _vendorProfileService.AddVendorProfileAsync(vendorProfile);
            VendorProfileRiskScoreResponseDto vendorProfileRiskScoreResponseDto = _vendorProfileService.CreateVendorProfileRiskScoreResponseDto(dbvendorProfile);
            _logger.LogInformation("Added VendorProfile successfully");
            return CreatedAtAction(nameof(GetVendorProfileById), new { id = dbvendorProfile.Id }, vendorProfileRiskScoreResponseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorProfile(int id, [FromBody] VendorProfileDto vendorProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Updating VendorProfile operation is starting...");
            await _vendorProfileService.UpdateVendorProfileAsync(id, vendorProfile);
            _logger.LogInformation("Updated VendorProfile successfully");
            return Ok("VendorProfile updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorProfile(int id)
        {
            _logger.LogInformation("Deleting VendorProfile operation is starting...");
            await _vendorProfileService.DeleteVendorProfile(id);
            _logger.LogInformation("VendorProfile deleted successfully.");
            return NoContent();
        }
    }
}
