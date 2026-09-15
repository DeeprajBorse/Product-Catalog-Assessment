using API.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmpResponseDTO>> GetAllEmployee(CancellationToken cancellationToken = default);
        Task<EmpResponseDTO> GetEmployeeById(int id, CancellationToken cancellationToken = default);
        Task<EmpResponseDTO> GetEmployeeByEmail(string email, CancellationToken cancellationToken = default);
        Task<EmpResponseDTO> CreateEmployee(EmployeeDTO dto, CancellationToken cancellationToken = default);
        Task<EmpResponseDTO> UpdateEmployee(int id, EmployeeDTO dto, CancellationToken cancellationToken = default);
        Task DeleteEmployee(int id, CancellationToken cancellationToken = default);
    }
}
