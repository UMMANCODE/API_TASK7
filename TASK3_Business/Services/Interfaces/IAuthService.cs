using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASK3_Business.Dtos.AuthDtos;

namespace TASK3_Business.Services.Interfaces {
  public interface IAuthService {
    Task<string> Login(LoginDto loginDto);
  }
}
