using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.API.Helpers;
using WMS.Application.DTOs;
using WMS.Domain.Interfaces;
using WMS.Domain.Models;
using AutoMapper;

namespace WMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public ClientsController(IClientService clientService, ICurrentUserService currentUser, IMapper mapper)
        {
            _clientService = clientService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients(
            [FromQuery] string? search, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _clientService.GetAllAsync(search, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<ClientDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<ClientDto>>.Ok(dtos, pagination));
        }

        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedClients(
            [FromQuery] string? search, [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _clientService.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<ClientDto>>(result.Items);
            var pagination = new PaginationInfo { Page = result.Page, PageSize = result.PageSize, TotalCount = result.TotalCount };
            return Ok(ApiResponse<IEnumerable<ClientDto>>.Ok(dtos, pagination));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostClient([FromBody] CreateClientDto dto)
        {
            var client = _mapper.Map<Client>(dto);
            await _clientService.CreateAsync(client, _currentUser.Username);
            return CreatedAtAction(nameof(GetClients), new { id = client.ClientId }, ApiResponse<ClientDto>.Ok(_mapper.Map<ClientDto>(client)));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutClient(int id, [FromBody] UpdateClientDto dto)
        {
            var client = _mapper.Map<Client>(dto);
            client.ClientId = id;
            await _clientService.UpdateAsync(id, client, _currentUser.Username);
            return Ok(ApiResponse<object>.Ok(null!, "Client updated successfully!"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var success = await _clientService.SoftDeleteAsync(id, _currentUser.Username);
            if (!success) return NotFound(ApiResponse<object>.Fail("Client not found."));
            return Ok(ApiResponse<object>.Ok(null!, "Client deleted successfully!"));
        }

        [HttpPost("restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreClient(int id)
        {
            var success = await _clientService.RestoreAsync(id);
            if (!success) return NotFound(ApiResponse<object>.Fail("Deleted client not found."));
            return Ok(ApiResponse<object>.Ok(null!, "Client restored successfully!"));
        }
    }
}
