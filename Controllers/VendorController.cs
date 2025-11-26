using Microsoft.AspNetCore.Mvc;
using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Services;

namespace VendorRiskScoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorProfileService _vendorProfileService;

        public VendorController(IVendorProfileService vendorProfileService)
        {
            _vendorProfileService = vendorProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVendorProfiles()
        {
            List<VendorProfile> vendorProfiles = await _vendorProfileService.GetVendorProfilesAsync();

            return Ok(vendorProfiles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorProfileById(int id)
        {
            VendorProfile vendorProfile = await _vendorProfileService.GetVendorProfileByIdAsync(id);
            return Ok(vendorProfile);
        }

        [HttpGet("{vendorId}/risk")]
        public async Task<IActionResult> GetVendorProfileRiskResult(int vendorId)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddVendorProfile([FromBody] VendorProfile vendorProfile)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VendorProfile dbvendorProfile = await _vendorProfileService.AddVendorProfileAsync(vendorProfile);
            return CreatedAtAction(nameof(GetVendorProfileById), new { id = dbvendorProfile.Id }, dbvendorProfile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorProfile(int id, [FromBody] VendorProfile vendorProfile)
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
