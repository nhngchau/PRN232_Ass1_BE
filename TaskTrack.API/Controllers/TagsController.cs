using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[Route("api/tags")]
public class TagsController : ApiControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _tagService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateTagDto dto)
    {
        var result = await _tagService.CreateAsync(dto);
        if (!result.Succeeded)
        {
            return FromResult(result);
        }

        return CreatedAtAction(nameof(GetAll), new { id = result.Data!.TagId }, result.Data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTagDto dto) => FromResult(await _tagService.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => NoContentFromResult(await _tagService.DeleteAsync(id));
}
