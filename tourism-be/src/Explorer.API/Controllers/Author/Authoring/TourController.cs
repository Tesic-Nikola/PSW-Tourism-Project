using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author.Authoring;

[Authorize(Policy = "authorPolicy")]
[Route("api/authoring/tours")]
public class TourController : BaseApiController
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpGet]
    public ActionResult<List<TourDto>> GetByAuthor()
    {
        var result = _tourService.GetByAuthor(User.PersonId());
        return CreateResponse(result);
    }

    [HttpGet("status/{status}")]
    public ActionResult<List<TourDto>> GetByAuthorAndStatus(int status)
    {
        var result = _tourService.GetByAuthorAndStatus(User.PersonId(), status);
        return CreateResponse(result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<TourDto> Get(long id)
    {
        var result = _tourService.Get(id);
        return CreateResponse(result);
    }

    [HttpPost]
    public ActionResult<TourDto> Create([FromBody] TourDto tour)
    {
        tour.AuthorId = User.PersonId();
        var result = _tourService.Create(tour);
        return CreateResponse(result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourDto> Update([FromBody] TourDto tour)
    {
        var result = _tourService.Update(tour);
        return CreateResponse(result);
    }

    [HttpPatch("{id:long}/publish")]
    public ActionResult<TourDto> Publish(long id)
    {
        var result = _tourService.Publish(id, User.PersonId());
        return CreateResponse(result);
    }

    [HttpPatch("{id:long}/cancel")]
    public ActionResult<TourDto> Cancel(long id)
    {
        var result = _tourService.Cancel(id, User.PersonId());
        return CreateResponse(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var result = _tourService.Delete(id);
        return CreateResponse(result);
    }
}