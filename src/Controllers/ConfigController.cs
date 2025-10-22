using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ParParWebsite.Api.DTOs;
using ParParWebsite.Api.Middleware;
using ParParWebsite.Api.Services;
using ParParWebsite.Api.Services.Interfaces;

namespace ParParWebsite.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController(IConfigService configService) : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateConfig([FromBody]ConfigDTO configDTO, CancellationToken cancellationToken)
        {
            await configService.Create(configDTO, cancellationToken);

            return Ok();
        }

       

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> GetConfigs(CancellationToken cancellationToken)
        {
            var res = await configService.Get(cancellationToken);

            return Ok(res);
        }



        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> DeleteConfig(int configId, CancellationToken cancellationToken)
        {
            await configService.Delete(configId, cancellationToken);

            return Ok();
        }
    }
}
