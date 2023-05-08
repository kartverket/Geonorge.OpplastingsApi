using Geonorge.OpplastingsApi.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Geonorge.OpplastingsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatasetController : ControllerBase
    {
        private readonly IDatasetService _datasetService;
        private readonly ILogger<DatasetController> _logger;

        public DatasetController(IDatasetService datasetService, ILogger<DatasetController> logger)
        {
            _datasetService = datasetService;
            _logger = logger;
        }

        [HttpGet(Name = "GetDatasets")]
        public async Task<List<Dataset>> Get()
        {
           return await _datasetService.GetDatasets();
        }
    }
}