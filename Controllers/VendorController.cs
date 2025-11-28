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

        public VendorController(IVendorProfileService vendorProfileService, IRiskAssessmentService riskAssessmentService)
        {
            _vendorProfileService = vendorProfileService;
            _riskAssessmentService = riskAssessmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVendorProfiles()
        {
            List<VendorProfile> vendorProfiles = await _vendorProfileService.GetVendorProfilesAsync();
            List<VendorProfileDto> vendorProfileDtos = vendorProfiles.Select(_vendorProfileService.CreateVendorProfileDto).ToList();
            return Ok(vendorProfileDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorProfileById(int id)
        {
            VendorProfile vendorProfile = await _vendorProfileService.GetVendorProfileByIdAsync(id);
            VendorProfileDto vendorProfileDto = _vendorProfileService.CreateVendorProfileDto(vendorProfile);
            return Ok(vendorProfileDto);
        }

        [HttpGet("{vendorId}/risk")]
        public async Task<IActionResult> GetVendorProfileRiskResult(int vendorId)
        {
            VendorProfile vendorProfile = await _vendorProfileService.GetVendorProfileByIdAsync(vendorId);
            RiskAssessmentResponseDto responseDto = await _riskAssessmentService.CreateRiskAssessmentResult(vendorProfile);
            return Ok(responseDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddVendorProfile([FromBody] VendorProfileDto vendorProfile)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VendorProfile dbvendorProfile = await _vendorProfileService.AddVendorProfileAsync(vendorProfile);
            VendorProfileRiskScoreResponseDto vendorProfileRiskScoreResponseDto = _vendorProfileService.CreateVendorProfileRiskScoreResponseDto(dbvendorProfile);
            return CreatedAtAction(nameof(GetVendorProfileById), new { id = dbvendorProfile.Id }, vendorProfileRiskScoreResponseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorProfile(int id, [FromBody] VendorProfileDto vendorProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _vendorProfileService.UpdateVendorProfileAsync(id, vendorProfile);
            return Ok("VendorProfile updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorProfile(int id)
        {
            await _vendorProfileService.DeleteVendorProfile(id);
            return NoContent();
        }
    }
}
