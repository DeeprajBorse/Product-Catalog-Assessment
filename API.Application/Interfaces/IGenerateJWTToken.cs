using API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace API.Application.Interfaces
{
    public interface IGenerateJWTToken
    {
        string GenerateAccessToken(Employee emp);

        (string Token, DateTime ExpiresOn) GenerateRefreshToken();
    }
}
