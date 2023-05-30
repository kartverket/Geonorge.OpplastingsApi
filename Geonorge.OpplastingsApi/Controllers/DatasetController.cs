using Geonorge.OpplastingsApi.Middleware;
using Geonorge.OpplastingsApi.Models.Api;
using Geonorge.OpplastingsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Utilities;
using Serilog;
using System;
using System.IO;
using File = Geonorge.OpplastingsApi.Models.Api.File;

namespace Geonorge.OpplastingsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatasetController : BaseController
    {
        private readonly IDatasetService _datasetService;
        private readonly ILogger<DatasetController> _logger;
        private readonly IMultipartRequestService _multipartRequestService;

        public DatasetController(IDatasetService datasetService, ILogger<DatasetController> logger, IMultipartRequestService multipartRequestService) : base(logger)
        {
            _datasetService = datasetService;
            _logger = logger;
            _multipartRequestService = multipartRequestService;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dataset))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteDataset(int id)
        {
            try
            {
                var datasetDeleted = await _datasetService.RemoveDataset(id);

                return Ok(datasetDeleted);
            }
            catch (Exception ex)
            {
                var result = HandleException(ex);

                if (result != null)
                    return result;

                throw;
            }
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
        //todo return filestream
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FileNew))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestFormLimits(MultipartBodyLengthLimit = 1_048_576_000)]
        [RequestSizeLimit(1_048_576_000)]
        //[DisableFormValueModelBinding]
        public async Task<IActionResult> AddFile()
        {
            var fileInfo = new FileNew();

            if (!ModelState.IsValid)
            {
                LogValidationErrors();
                return BadRequest(ModelState);
            }

            var inputData = await _multipartRequestService.GetFormDataAndFilesFromMultipart();

            if (inputData == null || !inputData.Files.Any())
                return BadRequest();

            try
            {
                var formData = inputData.Values.GetResults();
                if (formData.ContainsKey("datasetId")) 
                {
                    formData.TryGetValue("datasetId", out StringValues datasetID);
                    if(!string.IsNullOrEmpty(datasetID))
                        fileInfo.datasetId = int.Parse(datasetID);
                }
                var fileAdded = await _datasetService.AddFile(fileInfo, inputData.Files[0]);

                return Created("/Dataset/" + fileAdded.Id, fileAdded);
            }
            catch (Exception ex)
            {
                var result = HandleException(ex);

                if (result != null)
                    return result;

                throw;
            }

        }

        [HttpPut("file/{id:int}", Name = "PutFile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileUpdate))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFile(int id, FileUpdate file)
        {
            if (!ModelState.IsValid)
            {
                LogValidationErrors();
                return BadRequest(ModelState);
            }
            try
            {
                var fileUpdated = await _datasetService.UpdateFile(id, file, null);

                return Ok(fileUpdated);
            }
            catch (Exception ex)
            {
                var result = HandleException(ex);

                if (result != null)
                    return result;

                throw;
            }
        }

        [HttpDelete("file/{id:int}", Name = "DeleteFile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(File))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteFile(int id)
        {
            try
            {
                var fileDeleted = await _datasetService.RemoveFile(id);

                return Ok(fileDeleted);
            }
            catch (Exception ex)
            {
                var result = HandleException(ex);

                if (result != null)
                    return result;

                throw;
            }

        }

        [HttpPut("file-status-change/{id:int}", Name = "PutFileStatusChange")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(File))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFileStatusChange(int id, string status)
        {
            try
            {
                var fileUpdated= await _datasetService.FileStatusChange(id, status);

                return Ok(fileUpdated);
            }
            catch (Exception ex)
            {
                var result = HandleException(ex);

                if (result != null)
                    return result;

                throw;
            }
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