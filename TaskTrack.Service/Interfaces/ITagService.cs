using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface ITagService
{
    Task<List<TagDto>> GetAllAsync();
    Task<ServiceResult<TagDto>> CreateAsync(CreateTagDto dto);
    Task<ServiceResult<TagDto>> UpdateAsync(int id, UpdateTagDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
