using API.Application.DTO;
using API.Application.Interfaces;
using API.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly IAppLogger<EmployeeService> _logger;
        public EmployeeService(IUnitOfWork unit, IMapper mapper, IAppLogger<EmployeeService> logger)
        {
            _mapper = mapper;
            _unit = unit;
            _logger = logger;
        }

        //PasswordHash
        private string PasswordHash(EmployeeDTO dto)
        {
            var PasswordHash = new PasswordHasher<string>();
            var hash = PasswordHash.HashPassword(dto.Email, dto.PasswordHash);
            return hash;
        }

        public async Task<IEnumerable<EmpResponseDTO>> GetAllEmployee(CancellationToken cancellationToken = default)
        {
            var employees = await _unit.Employees.GetAllAsync(cancellationToken);

            _logger.LogInformation($"Retrieved {employees.Count()} employees from the database");

            return _mapper.Map<IEnumerable<EmpResponseDTO>>(employees);
        }

        public async Task<EmpResponseDTO> GetEmployeeById(int id, CancellationToken cancellationToken = default)
        {
            if(id <= 0)
                throw new ArgumentException("Invalid employee ID");
            
            var employee = await _unit.Employees.GetByIdAsync(id, cancellationToken);

            if (employee == null)
            {
                _logger.LogWarning($"Employee with ID {id} not found in the database");
                throw new KeyNotFoundException($"Employee with ID {id} not found");
            }
                

            _logger.LogInformation($"Retrieved employee with ID {id} from the database");

            return _mapper.Map<EmpResponseDTO>(employee);

        }

        public async Task<EmpResponseDTO> GetEmployeeByEmail(string email, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }
            var employee = await _unit.Employees.GetByAsync(e => e.Email == email, cancellationToken : cancellationToken);

            if(employee == null)
            {
                throw new KeyNotFoundException($"Employee with Email {email} not found");
            }

            _logger.LogInformation($"Retrieved employee with Email {email} from the database");
            return _mapper.Map<EmpResponseDTO>(employee);
        }

        public async Task<EmpResponseDTO> CreateEmployee(EmployeeDTO dto, CancellationToken cancellationToken = default)
        {
            if(dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Employee data cannot be null");
            }
             
            var employee = _mapper.Map<Employee>(dto);
            employee.PasswordHash = PasswordHash(dto);

            await _unit.Employees.AddAsync(employee, cancellationToken);

            await _unit.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Created new employee with ID {employee.Id} in the database");

            return _mapper.Map<EmpResponseDTO>(employee);
        }

        public async Task<EmpResponseDTO> UpdateEmployee(int id, EmployeeDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Employee data cannot be null");

            var employee = await _unit.Employees.GetByIdAsync(id, cancellationToken);
            if (employee == null)
            {
                _logger.LogWarning($"Employee with ID {id} not found in the database");
                throw new KeyNotFoundException($"Employee with ID {id} not found");
            }

            // Storing the original password hash 
            var currentHash = employee.PasswordHash;

            _mapper.Map(dto, employee);

            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                employee.PasswordHash = PasswordHash(dto);
            }
            else
            {
                employee.PasswordHash = currentHash;
            }

            _unit.Employees.Update(employee);
            await _unit.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Updated employee with ID {employee.Id} in the database");

            return _mapper.Map<EmpResponseDTO>(employee);
        }

        public async Task DeleteEmployee(int id, CancellationToken cancellationToken = default)
        {
            var employee = await _unit.Employees.GetByIdAsync(id, cancellationToken);

            if (employee == null)
            {
                 _logger.LogWarning($"Employee with ID {id} not found in the database");
                 throw new KeyNotFoundException($"Employee with ID {id} not found");
            }

            _unit.Employees.Delete(employee);

            _logger.LogInformation($"Deleted employee with ID {employee.Id} from the database");

            await _unit.SaveChangesAsync(cancellationToken);

        }
    }
}

