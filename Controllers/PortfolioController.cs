using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using ParParWebsite.Api.DTOs;
using ParParWebsite.Api.Helper;
using ParParWebsite.Api.Middleware;
using ParParWebsite.Api.Models;
using ParParWebsite.Api.Models.Enums;
using ParParWebsite.Api.Repositories.Interfaces;
using ParParWebsite.Api.Services.Interfaces;

namespace ParParWebsite.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController(IPortfolioService PortfolioService) : ControllerBase
    {
        [HttpPost]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(long.MaxValue)]
        [Route("Create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePortfolio(CancellationToken cancellationToken)
        {
            if (Request.ContentType is null)
                return BadRequest("Unsupported content type.");

            if (!Request.ContentType.Contains("multipart/form-data"))
                return BadRequest("Unsupported content type.");

            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;

            await PortfolioService.Create(Request.Body, boundary, cancellationToken);

            return Ok();
        }

       

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> GetPortfolios(CancellationToken cancellationToken)
        {
            var res = await PortfolioService.Get(cancellationToken);

            return Ok(res);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetPortfolioById(int PortfolioId, CancellationToken cancellationToken)
        {
            var res = await PortfolioService.GetById(PortfolioId, cancellationToken);

            return Ok(res);
        }

        [HttpPut]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(long.MaxValue)]
        [Route("Update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePortfolio(CancellationToken cancellationToken)
        {
            if (Request.ContentType is null)
                return BadRequest("Unsupported content type.");

            if (!Request.ContentType.Contains("multipart/form-data"))
                return BadRequest("Unsupported content type.");

            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;

            await PortfolioService.Update(Request.Body, boundary, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeletePortfolio(int PortfolioId, CancellationToken cancellationToken)
        {
            await PortfolioService.Delete(PortfolioId, cancellationToken);

            return Ok();
        }
    }
}
