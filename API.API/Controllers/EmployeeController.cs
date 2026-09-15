using API.Application.DTO;
using API.Application.DTOs;
using API.Application.Interfaces;
using API.Domain.Enum;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/employees")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Retrieves all employees (Admin & FrontDesk only).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.FrontDesk)}")]
        [ProducesResponseType(typeof(IEnumerable<EmpResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var employees = await _employeeService.GetAllEmployee(cancellationToken);
            return Ok(employees);
        }

        /// <summary>
        /// Retrieves an employee by ID (Admin & FrontDesk only).
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.FrontDesk)}")]
        [ProducesResponseType(typeof(EmpResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var employee = await _employeeService.GetEmployeeById(id, cancellationToken);
            return Ok(employee);
        }

        /// <summary>
        /// Retrieves an employee by Email (Admin & FrontDesk only).
        /// </summary>
        [HttpGet("by-email/{email}")]
        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.FrontDesk)}")]
        [ProducesResponseType(typeof(EmpResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken = default)
        {
            var employee = await _employeeService.GetEmployeeByEmail(email, cancellationToken);
            return Ok(employee);
        }

        /// <summary>
        /// Creates a new employee (Admin only).
        /// </summary>
        [HttpPost]        
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(EmpResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] EmployeeDTO dto, CancellationToken cancellationToken = default)
        {
            var created = await _employeeService.CreateEmployee(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "1.0" }, created);
        }

        /// <summary>
        /// Updates an existing employee (Admin only).
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(typeof(EmpResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeDTO dto, CancellationToken cancellationToken = default)
        {
            var updated = await _employeeService.UpdateEmployee(id, dto, cancellationToken);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes an employee (Admin only).
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            await _employeeService.DeleteEmployee(id, cancellationToken);
            return NoContent();
        }
    }
}