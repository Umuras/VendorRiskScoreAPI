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
        private readonly IVendorProfileRiskScoreService _vendorProfileRiskScoreService;
        private readonly ILogger<VendorController> _logger;

        public VendorController(IVendorProfileService vendorProfileService, IRiskAssessmentService riskAssessmentService, 
           IVendorProfileRiskScoreService vendorProfileRiskScoreService, ILogger<VendorController> logger)
        {
            _vendorProfileService = vendorProfileService;
            _riskAssessmentService = riskAssessmentService;
            _vendorProfileRiskScoreService = vendorProfileRiskScoreService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVendorProfiles()
        {
            _logger.LogInformation("Getting all vendor profiles...");
            List<VendorProfile> vendorProfiles = await _vendorProfileService.GetVendorProfilesAsync();
            List<VendorProfileResponseDto> vendorProfileDtos = vendorProfiles.Select(_vendorProfileService.CreateVendorProfileDto).ToList();
            _logger.LogInformation("All Vendor Profiles got successfully.");
            return Ok(vendorProfileDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorProfileById(int id)
        {
            _logger.LogInformation($"Getting vendor with id: {id}");
            VendorProfile vendorProfile = await _vendorProfileService.GetVendorProfileByIdAsync(id);
            VendorProfileResponseDto vendorProfileDto = _vendorProfileService.CreateVendorProfileDto(vendorProfile);
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
        public async Task<IActionResult> AddVendorProfile([FromBody] VendorProfileRequestDto vendorProfile)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Adding VendorProfile operation is starting...");
            VendorProfile dbvendorProfile = await _vendorProfileService.AddVendorProfileAsync(vendorProfile);
            RiskAssessmentResponseDto responseDto = await _riskAssessmentService.CreateRiskAssessmentResult(dbvendorProfile);
            _logger.LogInformation("Added VendorProfile successfully");
            return CreatedAtAction(nameof(GetVendorProfileById), new { id = dbvendorProfile.Id }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorProfile(int id, [FromBody] VendorProfileRequestDto vendorProfile)
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

        [HttpGet("{vendorId}/riskscores")]
        public async Task<IActionResult> GetVendorProfileRiskScoresResult(int vendorId)
        {
            _logger.LogInformation($"Creating VendorProfile Risk Scores Result for this id:{vendorId}");
            VendorProfile vendorProfile = await _vendorProfileService.GetVendorProfileByIdAsync(vendorId);
            VendorProfileRiskScoreResponseDto vendorProfileRiskScoreResponse = await _vendorProfileRiskScoreService.CreateVendorProfileRiskScoreResponseDto(vendorProfile);

            return Ok(vendorProfileRiskScoreResponse);
        }
    }
}
