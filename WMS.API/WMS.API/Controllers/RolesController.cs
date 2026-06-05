using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using AutoMapper;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RolesController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostRole([FromBody] Role role)
        {
            await _roleService.CreateAsync(role);
            return CreatedAtAction(nameof(GetRoles), new { id = role.RoleId }, role);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutRole(int id, [FromBody] Role role)
        {
            var success = await _roleService.UpdateAsync(id, role);
            if (!success) return BadRequest(new { message = "ID mismatch." });
            return Ok(new { message = "Role updated successfully!" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var success = await _roleService.DeleteAsync(id);
            if (!success) return NotFound(new { message = "Role not found." });
            return Ok(new { message = "Role deleted successfully!" });
        }
    }
}
