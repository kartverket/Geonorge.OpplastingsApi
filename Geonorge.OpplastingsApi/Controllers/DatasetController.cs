using Geonorge.OpplastingsApi.HttpClients;
using Geonorge.OpplastingsApi.Models;
using Geonorge.OpplastingsApi.Models.Api;
using Geonorge.OpplastingsApi.Models.Api.User;
using Geonorge.OpplastingsApi.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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
        private readonly IValidatorHttpClient _validatorHttpClient;
        private readonly IMessageService _messageService;
        private readonly IAuthService _authService;

        public DatasetController(
            IDatasetService datasetService, 
            IMultipartRequestService multipartRequestService, 
            IAuthService authService,
            IValidatorHttpClient validatorHttpClient,
            IMessageService messageService,
            ILogger<DatasetController> logger) : base(logger)
        {
            _datasetService = datasetService;
            _multipartRequestService = multipartRequestService;
            _authService = authService;
            _validatorHttpClient = validatorHttpClient;
            _messageService = messageService;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDownloadFile(int id)
        {
            try
            {
                var filePath = await _datasetService.GetFilePath(id);
                return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", System.IO.Path.GetFileName(filePath));

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
        public async Task<IActionResult> AddFile()
        {   
            if (!ModelState.IsValid)
            {
                LogValidationErrors();
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _authService.GetUser() ?? throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

                var inputData = await _multipartRequestService.GetInputDataFromMultipartAsync();

                if (inputData?.File == null)
                    return BadRequest();

                await CheckFileExtensionValidity(inputData);

                if (inputData.File.FileName.EndsWith("gml"))
                {
                    await _messageService.SendAsync("Validerer...");

                    var validationReport = await _validatorHttpClient.ValidateAsync(inputData.File);

                    if (validationReport.Errors > 0)
                    {
                        if(inputData.RequireValidFile)
                        return UnprocessableEntity($"Filen inneholder {validationReport.Errors} feil");
                    }
                }

                var fileAdded = await _datasetService.AddFile(inputData.FileInfo, inputData.File, user);

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
        public async Task<IActionResult> UpdateFile(int id, FileUpdate fileInfo)
        {
            if (!ModelState.IsValid)
            {
                LogValidationErrors();
                return BadRequest(ModelState);
            }
            try
            {
                var fileUpdated = await _datasetService.UpdateFile(id, fileInfo);

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


        [HttpGet("file/statuses", Name = "GetStatuses")]
        public async Task<IActionResult> GetStatuses()
        {
            Dictionary<string, string> statuses = new Dictionary<string, string>();
            statuses.Add("Submitted", Const.Status.Submitted);
            statuses.Add("InProcess", Const.Status.InProcess);
            statuses.Add("Valid", Const.Status.Valid);
            statuses.Add("Invalid", Const.Status.Invalid);
            return Ok(statuses);

        }

        [HttpGet("fileformats", Name = "GetFileFormats")]
        public async Task<IActionResult> GetFileFormats()
        {
            return Ok(await _datasetService.GetFileFormats());

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

        private async Task CheckFileExtensionValidity(InputData inputData)
        {
            var dataset = await _datasetService.GetDataset(inputData.FileInfo.datasetId);

            var supportedTypes = dataset.AllowedFileFormats
                .Select(fileFormat => fileFormat.Extension)
                .ToList();

            var fileExt = Path.GetExtension(inputData.File.FileName)[1..];

            if (!supportedTypes.Contains(fileExt))
                throw new Exception($"Ugyldig filendelse. Gyldig filendelser er: {string.Join(", ", supportedTypes)}");
        }

    }
}