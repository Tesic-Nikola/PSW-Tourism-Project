using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Authoring;

[Authorize(Policy = "authorPolicy")]
[Route("api/authoring/keypoints")]
public class KeyPointController : BaseApiController
{
    private readonly IKeyPointService _keyPointService;

    public KeyPointController(IKeyPointService keyPointService)
    {
        _keyPointService = keyPointService;
    }

    [HttpGet("tour/{tourId:long}")]
    public ActionResult<List<KeyPointDto>> GetByTour(long tourId)
    {
        var result = _keyPointService.GetByTour(tourId);
        return CreateResponse(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<KeyPointDto> Get(long id)
    {
        var result = _keyPointService.Get(id);
        return CreateResponse(result);
    }

    [HttpPost]
    public ActionResult<KeyPointDto> Create([FromBody] KeyPointDto keyPoint)
    {
        var result = _keyPointService.Create(keyPoint);
        return CreateResponse(result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<KeyPointDto> Update([FromBody] KeyPointDto keyPoint)
    {
        var result = _keyPointService.Update(keyPoint);
        return CreateResponse(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var result = _keyPointService.Delete(id);
        return CreateResponse(result);
    }
}