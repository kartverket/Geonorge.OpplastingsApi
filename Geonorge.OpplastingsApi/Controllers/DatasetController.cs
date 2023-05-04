using Geonorge.OpplastingsApi.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace Geonorge.OpplastingsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatasetController : ControllerBase
    {

        private readonly IDatasetService _datasetService;

        public DatasetController(IDatasetService datasetService)
        {
            _datasetService = datasetService;
        }

        [HttpGet(Name = "GetDatasets")]
        public async Task<List<Dataset>> Get()
        {
           return await _datasetService.GetDatasets();
        }
    }
}