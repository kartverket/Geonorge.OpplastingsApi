using Geonorge.OpplastingsApi.Models;
using Geonorge.OpplastingsApi.Models.Api;
using Geonorge.OpplastingsApi.Models.Api.User;
using Geonorge.OpplastingsApi.Models.Entity;
using Geonorge.OpplastingsApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using api = Geonorge.OpplastingsApi.Models.Api;

public class DatasetService : IDatasetService
{

    private readonly ApplicationContext _context;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;
    private readonly FileConfiguration _config;

    public DatasetService(ApplicationContext context, IAuthService authService, INotificationService notificationService, IOptions<FileConfiguration> options) 
    {
        _context = context;
        _authService = authService;
        _notificationService = notificationService;
        _config = options.Value;
    }

    public async Task<List<api.Dataset>> GetDatasets()
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        if (user.IsAdmin) 
        { 
            return await _context.Datasets.Select(
                d => new api.Dataset 
                    {
                        Id = d.Id,
                        Title = d.Title,
                        ContactEmail = d.ContactEmail, ContactEmailExtra = d.ContactEmailExtra, ContactName = d.ContactName, MetadataUuid = d.MetadataUuid, OwnerOrganization = d.OwnerOrganization, RequiredRole = d.RequiredRole, RequireValidFile = d.RequireValidFile,
                        Files = d.Files.Select(f => new api.File {Id = f.Id, FileName = f.FileName, Status = f.Status, Date = f.Date }).ToList(),
                    AllowedFileFormats = d.AllowedFileFormats.Select(f => new api.FileFormat { Extension = f.Extension, Name = f.Name }).ToList()
                }
                ).OrderBy(o => o.Title).ToListAsync();
        }
        else if (user.HasRole(Role.Editor)) 
        {
            var datasetsOwner = await _context.Datasets.Where(d => d.OwnerOrganization == user.OrganizationName).Select(
                d => new api.Dataset
                {
                    Id = d.Id,
                    Title = d.Title,
                    ContactEmail = d.ContactEmail, ContactEmailExtra = d.ContactEmailExtra, ContactName = d.ContactName, MetadataUuid = d.MetadataUuid, OwnerOrganization = d.OwnerOrganization, RequiredRole = d.RequiredRole, RequireValidFile=d.RequireValidFile,
                    Files = d.Files.Select(f => new api.File { Id = f.Id, FileName = f.FileName, Status = f.Status, Date = f.Date }).ToList(),
                    AllowedFileFormats = d.AllowedFileFormats.Select(f => new api.FileFormat { Extension = f.Extension, Name = f.Name }).ToList()
                }
                ).OrderBy(o => o.Title).ToListAsync();

            if (datasetsOwner.Any()) 
                return datasetsOwner;

            var datasetsUploader = await _context.Datasets.Where(d => user.Roles.Contains(d.RequiredRole)).Select(
                d => new api.Dataset
                {
                    Id = d.Id,
                    Title = d.Title,
                    RequireValidFile = d.RequireValidFile,
                    Files = d.Files.Where(u => u.UploaderUsername == user.Username).Select(f => new api.File { Id = f.Id, FileName = f.FileName, Status = f.Status, Date = f.Date }).ToList(),
                    AllowedFileFormats = d.AllowedFileFormats.Select(f => new api.FileFormat { Extension = f.Extension, Name = f.Name }).ToList()
                }
                ).OrderBy(o => o.Title).ToListAsync();

            if (datasetsUploader.Any())
                return datasetsUploader;

        }

        throw new AuthorizationException("Manglende tilgang til ressursen");

    }

    public async Task<api.Dataset> GetDataset(int id)
    {

        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var dataset = await _context.Datasets.Where(d => d.Id == id && ((d.OwnerOrganization == user.OrganizationName || user.Roles.Contains(d.RequiredRole)) || user.IsAdmin)).Select(
            d => new api.Dataset
            {
                Id=d.Id,
                Title = d.Title,
                ContactEmail = d.ContactEmail,ContactEmailExtra=d.ContactEmailExtra, ContactName = d.ContactName, MetadataUuid = d.MetadataUuid, OwnerOrganization = d.OwnerOrganization, RequiredRole = d.RequiredRole,
                RequireValidFile = d.RequireValidFile,
                Files = d.Files.Select(f => new api.File { Id = f.Id, FileName = f.FileName, Status = f.Status,Date = f.Date }).OrderByDescending(o => o.Date).ToList(),
                AllowedFileFormats = d.AllowedFileFormats.Select(f => new api.FileFormat { Extension = f.Extension, Name = f.Name }).ToList()
            }
            ).FirstOrDefaultAsync();

        if (dataset == null)
            throw new Exception("Ingen datasett funnet");

            return dataset;
    }

    public async Task<api.Dataset> AddDataset(api.DatasetNew datasetNew)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        if (!user.IsAdmin)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var dataset = new Geonorge.OpplastingsApi.Models.Entity.Dataset
        {
            Title = datasetNew.Title,
            MetadataUuid = datasetNew.MetadataUuid,
            ContactEmail = datasetNew.ContactEmail,
            ContactName = datasetNew.ContactName,
            ContactEmailExtra = datasetNew.ContactEmailExtra,
            OwnerOrganization = datasetNew.OwnerOrganization,
            RequiredRole = datasetNew.RequiredRole,
            RequireValidFile = datasetNew.RequireValidFile,
            AllowedFileFormats = new List<Geonorge.OpplastingsApi.Models.Entity.FileFormat>()
        };
        foreach (var format in datasetNew.AllowedFileFormats)
        {
            var formatExtension = _context.FileFormats.Where(f => f.Extension == format).FirstOrDefault();
            if(formatExtension != null)
                dataset.AllowedFileFormats.Add(formatExtension);
        }
        await _context.Datasets.AddAsync(dataset);
        await _context.SaveChangesAsync();

        return new api.Dataset { Id = dataset.Id, Title = dataset.Title, ContactEmail = dataset.ContactEmail, ContactEmailExtra = dataset.ContactEmailExtra,
            ContactName = dataset.ContactName, MetadataUuid = dataset.MetadataUuid, OwnerOrganization = dataset.OwnerOrganization, RequiredRole = dataset.RequiredRole, RequireValidFile = dataset.RequireValidFile,
             AllowedFileFormats = dataset.AllowedFileFormats.Select(f => new api.FileFormat { Extension = f.Extension, Name = f.Name }).ToList()
        };

    }

    public async Task<api.Dataset> UpdateDataset(int id, api.DatasetUpdate datasetUpdated)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        if (!user.IsAdmin)
            throw new AuthorizationException("Brukeren har ikke tilgang");

        var dataset = await _context.Datasets.Where(d => d.Id == id).Include(f => f.AllowedFileFormats).FirstOrDefaultAsync();

        if (dataset == null)
            throw new Exception("Datasettet finnes ikke");

        if(!string.IsNullOrEmpty(datasetUpdated.ContactName))
            dataset.ContactName = datasetUpdated.ContactName;

        if (!string.IsNullOrEmpty(datasetUpdated.ContactEmail))
            dataset.ContactEmail = datasetUpdated.ContactEmail;

        if (!string.IsNullOrEmpty(datasetUpdated.ContactEmailExtra))
            dataset.ContactEmailExtra = datasetUpdated.ContactEmailExtra;

        if (!string.IsNullOrEmpty(datasetUpdated.OwnerOrganization))
            dataset.OwnerOrganization = datasetUpdated.OwnerOrganization;

        if (!string.IsNullOrEmpty(datasetUpdated.Title))
            dataset.Title = datasetUpdated.Title;

        if (!string.IsNullOrEmpty(datasetUpdated.MetadataUuid))
            dataset.MetadataUuid = datasetUpdated.MetadataUuid;

        if (!string.IsNullOrEmpty(datasetUpdated.RequiredRole))
            dataset.RequiredRole = datasetUpdated.RequiredRole;

        dataset.RequireValidFile = datasetUpdated.RequireValidFile;

        if (dataset.AllowedFileFormats != null) 
        { 
            foreach (var item in dataset.AllowedFileFormats.ToList()) 
            {
                dataset.AllowedFileFormats.Remove(item);
            }
        }

        dataset.AllowedFileFormats = new List<Geonorge.OpplastingsApi.Models.Entity.FileFormat>();

        foreach (var format in datasetUpdated.AllowedFileFormats)
        {
            var formatExtension = _context.FileFormats.Where(f => f.Extension == format).FirstOrDefault();
            if (formatExtension != null)
                dataset.AllowedFileFormats.Add(formatExtension);
        }

        _context.SaveChanges();

        return new api.Dataset 
        { 
            Id = dataset.Id, Title = dataset.Title,
            ContactEmail = dataset.ContactEmail, ContactName = dataset.ContactName, ContactEmailExtra = dataset.ContactEmailExtra,
            MetadataUuid = dataset.MetadataUuid, RequiredRole = dataset.RequiredRole, RequireValidFile = dataset.RequireValidFile,  
            OwnerOrganization=dataset.OwnerOrganization,
            AllowedFileFormats = dataset.AllowedFileFormats.Select(f => new api.FileFormat { Extension = f.Extension, Name = f.Name }).ToList()
        };
    }

    public async Task<api.Dataset> RemoveDataset(int id)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        if (!user.IsAdmin)
            throw new AuthorizationException("Brukeren har ikke tilgang");

        var dataset = _context.Datasets.FirstOrDefault(d => d.Id == id);
        if (dataset != null) 
        {
            _context.Datasets.Remove(dataset);
            await _context.SaveChangesAsync();

            return new api.Dataset { Id = dataset.Id, Title = dataset.Title };
        }

        return null;
    }

    public async Task<api.File> GetFile(int id)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var file = await _context.Files.Where(f => f.Id == id).Include(d => d.Dataset).FirstOrDefaultAsync();

        if (file == null)
            throw new Exception("File not found");

        if (user.IsAdmin) 
        {
            return new api.File { Id = file.Id, FileName = file.FileName, Status = file.Status };
        }

        if (user.HasRole(Role.Editor))
        {
            if(file.Dataset.OwnerOrganization == user.OrganizationName)
                return new api.File { Id = file.Id, FileName = file.FileName, Status = file.Status };
            else if(file.UploaderUsername == user.Username)
                return new api.File { Id = file.Id, FileName = file.FileName, Status = file.Status };
        }

        throw new AuthorizationException("Manglende tilgang til ressursen");
    }
    public async Task<api.File> AddFile(api.FileNew fileInfo, IFormFile file, User user)
    {
        if(user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var dataset = _context.Datasets.Where((d) => d.Id == fileInfo.datasetId && ((d.OwnerOrganization == user.OrganizationName || user.Roles.Contains(d.RequiredRole)) || user.IsAdmin)).FirstOrDefault();

        if (dataset == null)
            throw new Exception("Datasett ikke tilgjengelig");

        if (dataset.Files == null)
            dataset.Files = new List<Geonorge.OpplastingsApi.Models.Entity.File>();

        var fileNew = new Geonorge.OpplastingsApi.Models.Entity.File
        {
            FileName = file.FileName,
            Date = DateTime.Now,
            Status = Const.Status.Submitted,
            UploaderOrganization = user.OrganizationName,
            UploaderPerson = user.Name,
            UploaderEmail = user.Email,
            UploaderUsername = user.Username
        };


        dataset.Files.Add(fileNew);
        await _context.SaveChangesAsync();
        api.File fileAdded = new api.File();
        fileAdded.Id = fileNew.Id;
        fileAdded.FileName = fileNew.FileName;
        fileAdded.Status = fileNew.Status;
        fileAdded.Dataset = new api.Dataset { Id = dataset.Id, Title = dataset.Title, ContactEmail = dataset.ContactEmail, ContactName = dataset.ContactName, ContactEmailExtra = dataset.ContactEmailExtra, MetadataUuid = dataset.MetadataUuid };

        string uploads = Path.Combine(_config.Path, dataset.MetadataUuid);
        if (file.Length > 0)
        {
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            string filePath = Path.Combine(uploads, file.FileName);
            if (System.IO.File.Exists(filePath)) 
            {
                var newFileName = file.FileName.Replace(".", "-" + fileAdded.Id + ".");
                filePath = Path.Combine(uploads, newFileName);
                fileNew.FileName = newFileName;
                _context.Files.Update(fileNew);
                _context.SaveChangesAsync();
                fileAdded.FileName = fileNew.FileName;

            }
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        _notificationService.SendEmailUploadedFileToContact(fileNew);

        return fileAdded;
    }
    public async Task<api.File> UpdateFile(int id, api.FileUpdate fileUpdated)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var fileData = await _context.Files.Where(f => f.Id == id).Include(d => d.Dataset).FirstOrDefaultAsync();

        if (!user.IsAdmin)
            if (!(user.HasRole(Role.Editor) && fileData.Dataset.OwnerOrganization == user.OrganizationName)
             || !(user.HasRole(Role.Editor) && fileData.UploaderUsername == user.Username)
             )
            {
            throw new AuthorizationException("Brukeren har ikke tilgang");
        }

        var currentStatus = fileData.Status;

        if (fileData != null)
        {
            if (!string.IsNullOrEmpty(fileUpdated.Status))
                fileData.Status = fileUpdated.Status;

            _context.Files.Update(fileData);
            _context.SaveChangesAsync();

            if(currentStatus != fileUpdated.Status)
                _notificationService.SendEmailStatusChangedToUploader(fileData);
        }

        return new api.File { Id = fileData.Id, FileName = fileData.FileName, Status = fileData.Status };
    }

    public async Task<api.File> RemoveFile(int id)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var file = await _context.Files.Where(f => f.Id == id).Include(d => d.Dataset).FirstOrDefaultAsync();

        if (!user.IsAdmin)
            if( !(user.HasRole(Role.Editor) && file.Dataset.OwnerOrganization == user.OrganizationName)
             || !(user.HasRole(Role.Editor) && file.UploaderUsername == user.Username)
             )
        {
            throw new AuthorizationException("Brukeren har ikke tilgang");
        }


        if (file != null) { 
            _context.Files.Remove(file);
            _context.SaveChangesAsync();
        }

        string path = Path.Combine(_config.Path, file.Dataset.MetadataUuid);
        string filePath = Path.Combine(path, file.FileName);
        if (System.IO.File.Exists(filePath)) 
        {
            System.IO.File.Delete(filePath);
        }

        return new api.File {  Id = file.Id, FileName = file.FileName};
    }

    public async Task<string> GetFilePath(int id)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var file = await _context.Files.Where(f => f.Id == id).Include(d => d.Dataset).FirstOrDefaultAsync();

        if (file == null)
            throw new Exception("Filen ble ikke funnnet");

        if (!user.IsAdmin)
            if (!(user.HasRole(Role.Editor) && file.Dataset.OwnerOrganization == user.OrganizationName)
             || !(user.HasRole(Role.Editor) && file.UploaderUsername == user.Username)
             )
        {
            throw new AuthorizationException("Brukeren har ikke tilgang");
        }

        string path = Path.Combine(_config.Path, file.Dataset.MetadataUuid);
        string filePath = Path.Combine(path, file.FileName);
        return filePath;
    }

    public async Task<List<api.FileFormat>> GetFileFormats()
    {
        return await _context.FileFormats.Select(f => new api.FileFormat { Extension = f.Extension, Name = f.Name }).ToListAsync();
    }
}

public interface IDatasetService
{
    Task<List<api.Dataset>> GetDatasets();
    Task<api.Dataset> GetDataset(int id);
    Task<api.Dataset> AddDataset(api.DatasetNew dataset);
    Task<api.Dataset> UpdateDataset(int id, api.DatasetUpdate dataset);
    Task<api.Dataset> RemoveDataset(int id);

    Task<api.File> GetFile(int id);
    Task<string> GetFilePath(int id);
    Task<api.File> AddFile(api.FileNew fileInfo, IFormFile file, User user);
    Task<api.File> UpdateFile(int id, api.FileUpdate fileInfo);
    Task<api.File> RemoveFile(int id);
    Task<List<api.FileFormat>> GetFileFormats();
}