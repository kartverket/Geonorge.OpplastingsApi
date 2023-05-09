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

        [HttpGet("{id:int}")]
        public async Task<Dataset> GetDataset(int id)
        {
            return await _datasetService.GetDataset(id);
        }

        [HttpPost(Name = "PostDataset")]
        public async Task<Dataset> AddDataset(Dataset dataset)
        {
            return await _datasetService.AddDataset(dataset);
        }

        [HttpDelete("{id:int}", Name = "DeleteDataset")]
        public async Task<Dataset> DeleteDataset(int id)
        {
            return await _datasetService.RemoveDataset(id);
        }
    }
}