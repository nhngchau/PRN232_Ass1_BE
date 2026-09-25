using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetActiveAsync();
    Task<ServiceResult<DepartmentDetailDto>> GetDetailAsync(int id);
    Task<List<DepartmentDto>> SearchAsync(string? name);
    Task<ServiceResult<DepartmentDto>> CreateAsync(CreateDepartmentDto dto);
    Task<ServiceResult<DepartmentDto>> UpdateAsync(int id, UpdateDepartmentDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
