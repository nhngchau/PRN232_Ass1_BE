using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;
using TaskTrack.Service.Mapping;

namespace TaskTrack.Service.Implementations;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<List<TagDto>> GetAllAsync() =>
        (await _tagRepository.GetAllAsync()).Select(t => t.ToDto()).ToList();

    public async Task<ServiceResult<TagDto>> CreateAsync(CreateTagDto dto)
    {
        if (await _tagRepository.NameExistsAsync(dto.TagName))
        {
            return ServiceResult<TagDto>.Invalid(new Dictionary<string, string[]>
            {
                [nameof(dto.TagName)] = ["Tag name already exists."]
            });
        }

        var tag = new Tag
        {
            TagName = dto.TagName.Trim(),
            Color = NormalizeColor(dto.Color)
        };

        await _tagRepository.AddAsync(tag);
        await _tagRepository.SaveChangesAsync();
        return ServiceResult<TagDto>.Success(tag.ToDto());
    }

    public async Task<ServiceResult<TagDto>> UpdateAsync(int id, UpdateTagDto dto)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag is null)
        {
            return ServiceResult<TagDto>.Missing("Tag was not found.");
        }

        if (await _tagRepository.NameExistsAsync(dto.TagName, id))
        {
            return ServiceResult<TagDto>.Invalid(new Dictionary<string, string[]>
            {
                [nameof(dto.TagName)] = ["Tag name already exists."]
            });
        }

        tag.TagName = dto.TagName.Trim();
        tag.Color = NormalizeColor(dto.Color);
        _tagRepository.Update(tag);
        await _tagRepository.SaveChangesAsync();
        return ServiceResult<TagDto>.Success(tag.ToDto());
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag is null)
        {
            return ServiceResult<bool>.Missing("Tag was not found.");
        }

        if (await _tagRepository.IsUsedAsync(id))
        {
            return ServiceResult<bool>.Failure("Tag cannot be deleted because it is assigned to one or more tasks.");
        }

        _tagRepository.Delete(tag);
        await _tagRepository.SaveChangesAsync();
        return ServiceResult<bool>.Success(true);
    }

    private static string? NormalizeColor(string? color) =>
        string.IsNullOrWhiteSpace(color) ? null : color.Trim();
}
