using Geonorge.OpplastingsApi.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using File = Geonorge.OpplastingsApi.Models.Api.File;

namespace Geonorge.OpplastingsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatasetController : BaseController
    {
        private readonly IDatasetService _datasetService;
        private readonly ILogger<DatasetController> _logger;

        public DatasetController(IDatasetService datasetService, ILogger<DatasetController> logger) : base(logger)
        {
            _datasetService = datasetService;
            _logger = logger;
        }

        [HttpGet(Name = "GetDatasets")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Dataset>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Get()
        {
           try 
           { 
              return Ok(await _datasetService.GetDatasets());
           }
           catch(Exception ex) 
           {
            var result = HandleException(ex);

            if (result != null)
                return result;

            throw;
           }
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

        [HttpGet("file/{id:int}")]
        public async Task<File> GetFile(int id)
        {
            return await _datasetService.GetFile(id);
        }

        [HttpPost("file", Name = "PostFile")]
        public async Task<File> AddFile(File fileInfo)
        {
            return await _datasetService.AddFile(fileInfo, null);
        }
    }
}