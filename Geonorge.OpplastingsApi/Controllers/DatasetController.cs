using Geonorge.OpplastingsApi.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using File = Geonorge.OpplastingsApi.Models.Api.File;

namespace Geonorge.OpplastingsApi.Controllers
{
    //todo handle response exception for all methods
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dataset))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDataset(int id)
        {
            try
            {
                return Ok(await _datasetService.GetDataset(id));
            }
            catch (Exception ex)
            {
                var result = HandleException(ex);

                if (result != null)
                    return result;

                throw;
            }
        }

        [HttpPost(Name = "PostDataset")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DatasetNew))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddDataset(DatasetNew dataset)
        {
            if (!ModelState.IsValid)
            {
                LogValidationErrors();
                return BadRequest(ModelState);
            }
            try 
            { 
            var datasetAdded = await _datasetService.AddDataset(dataset);

            return Created("/Dataset/" + datasetAdded.Id, datasetAdded);
            }
            catch(Exception ex) 
            {
                var result = HandleException(ex);

                if (result != null)
                    return result;

                throw;
            }
        }

        [HttpPut("{id:int}", Name = "PutDataset")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DatasetUpdate))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDataset(int id, DatasetUpdate dataset)
        {
            if (!ModelState.IsValid)
            {
                LogValidationErrors();
                return BadRequest(ModelState);
            }
            try
            {
                var datasetUpdated = await _datasetService.UpdateDataset(id, dataset);

                return Ok(datasetUpdated);
            }
            catch (Exception ex)
            {
                var result = HandleException(ex);

                if (result != null)
                    return result;

                throw;
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteDataset")]
        public async Task<Dataset> DeleteDataset(int id)
        {
            return await _datasetService.RemoveDataset(id);
        }

        [HttpGet("file/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(File))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetFile(int id)
        {
            try
            {
                return Ok(await _datasetService.GetFile(id));
            }
            catch (Exception ex)
            {
                var result = HandleException(ex);

                if (result != null)
                    return result;

                throw;
            }
        }

        [HttpGet("download-file/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDownloadFile(int id)
        {
            try
            {
                return Ok(await _datasetService.DownloadFile(id));
            }
            catch (Exception ex)
            {
                var result = HandleException(ex);

                if (result != null)
                    return result;

                throw;
            }
        }

        [HttpPost("file", Name = "PostFile")]
        public async Task<File> AddFile(File fileInfo)
        {
            return await _datasetService.AddFile(fileInfo, null);
        }

        [HttpPut("file/{id:int}", Name = "PutFile")]
        public async Task<File> UpdateFile(int id, File file)
        {
            return await _datasetService.UpdateFile(id, file, null);
        }

        [HttpDelete("file/{id:int}", Name = "DeleteFile")]
        public async Task<File> UpdateFile(int id)
        {
            return await _datasetService.RemoveFile(id);
        }

        [HttpPut("fileStatusChange/{id:int}", Name = "PutFileStatusChange")]
        public async Task<File> UpdateFileStatusChange(int id, string status)
        {
            return await _datasetService.FileStatusChange(id, status);
        }

        private void LogValidationErrors()
        {
            _logger.LogError("Invalid model state: {@errors}", ModelState.Select(x =>
                new
                {
                    Key = x.Key,
                    Errors = x.Value?.Errors.Select(x => new
                    {
                        x.ErrorMessage,
                        x.Exception
                    })
                }));
        }

    }
}