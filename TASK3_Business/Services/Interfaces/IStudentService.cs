using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASK3_Business.Dtos;
using TASK3_Business.Dtos.GroupDtos;
using TASK3_Business.Dtos.StudentDtos;

namespace TASK3_Business.Services.Interfaces {
  public interface IStudentService {
    public Task<int> Create(StudentCreateOneDto createDto);
    public Task<PaginatedList<StudentGetAllDto>> GetAll(int pageNumber = 1, int pageSize = 1);
    public Task<List<StudentGetAllDto>> GetWhole();
    public Task<StudentGetOneDto> GetById(int id);
    public Task Update(int id, StudentUpdateOneDto updateDto);
    public Task Delete(int id);
  }
}
