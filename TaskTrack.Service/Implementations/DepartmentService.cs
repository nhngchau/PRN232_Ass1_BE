using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;
using TaskTrack.Service.Mapping;

namespace TaskTrack.Service.Implementations;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<List<DepartmentDto>> GetActiveAsync() =>
        (await _departmentRepository.GetActiveAsync()).Select(d => d.ToDto()).ToList();

    public async Task<ServiceResult<DepartmentDetailDto>> GetDetailAsync(int id)
    {
        var department = await _departmentRepository.GetDetailAsync(id);
        return department is null
            ? ServiceResult<DepartmentDetailDto>.Missing("Department was not found.")
            : ServiceResult<DepartmentDetailDto>.Success(department.ToDetailDto());
    }

    public async Task<List<DepartmentDto>> SearchAsync(string? name) =>
        (await _departmentRepository.SearchAsync(name)).Select(d => d.ToDto()).ToList();

    public async Task<ServiceResult<DepartmentDto>> CreateAsync(CreateDepartmentDto dto)
    {
        var department = new Department
        {
            DepartmentName = dto.DepartmentName.Trim(),
            DepartmentDescription = dto.DepartmentDescription.Trim(),
            IsActive = true
        };

        await _departmentRepository.AddAsync(department);
        await _departmentRepository.SaveChangesAsync();
        return ServiceResult<DepartmentDto>.Success(department.ToDto());
    }

    public async Task<ServiceResult<DepartmentDto>> UpdateAsync(int id, UpdateDepartmentDto dto)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department is null || !department.IsActive)
        {
            return ServiceResult<DepartmentDto>.Missing("Department was not found.");
        }

        department.DepartmentName = dto.DepartmentName.Trim();
        department.DepartmentDescription = dto.DepartmentDescription.Trim();
        _departmentRepository.Update(department);
        await _departmentRepository.SaveChangesAsync();
        return ServiceResult<DepartmentDto>.Success(department.ToDto());
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department is null)
        {
            return ServiceResult<bool>.Missing("Department was not found.");
        }

        if (await _departmentRepository.HasProjectsAsync(id))
        {
            return ServiceResult<bool>.Failure("Department cannot be deleted because projects are linked to it.");
        }

        _departmentRepository.Delete(department);
        await _departmentRepository.SaveChangesAsync();
        return ServiceResult<bool>.Success(true);
    }
}
