using Geonorge.OpplastingsApi.Models.Api.User;
using Geonorge.OpplastingsApi.Models.Entity;
using Geonorge.OpplastingsApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
                        ContactEmail = d.ContactEmail, ContactName = d.ContactName, MetadataUuid = d.MetadataUuid, OwnerOrganization = d.OwnerOrganization, RequiredRole = d.RequiredRole,
                        Files = d.Files.Select(f => new api.File {Id = f.Id, FileName = f.FileName }).ToList()
                    }
                ).ToListAsync();
        }
        else if (user.HasRole(Role.Editor)) 
        {
            var datasetsOwner = await _context.Datasets.Where(d => d.OwnerOrganization == user.OrganizationName && user.Roles.Contains(d.RequiredRole)).Select(
                d => new api.Dataset
                {
                    Id = d.Id,
                    Title = d.Title,
                    ContactEmail = d.ContactEmail, ContactName = d.ContactName, MetadataUuid = d.MetadataUuid, OwnerOrganization = d.OwnerOrganization, RequiredRole = d.RequiredRole,
                    Files = d.Files.Select(f => new api.File { Id = f.Id, FileName = f.FileName }).ToList()
                }
                ).ToListAsync();

            if (datasetsOwner.Any()) 
                return datasetsOwner;

            var datasetsUploader = await _context.Datasets.Where(d => user.Roles.Contains(d.RequiredRole)).Select(
                d => new api.Dataset
                {
                    Id = d.Id,
                    Title = d.Title,
                    Files = d.Files.Where(u => u.UploaderUsername == user.Username).Select(f => new api.File { Id = f.Id, FileName = f.FileName }).ToList()
                }
                ).ToListAsync();

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

        var dataset = await _context.Datasets.Where(d => d.Id == id && ((d.OwnerOrganization == user.OrganizationName && user.Roles.Contains(d.RequiredRole)) || user.IsAdmin)).Select(
            d => new api.Dataset
            {
                Id=d.Id,
                Title = d.Title,
                ContactEmail = d.ContactEmail, ContactName = d.ContactName, MetadataUuid = d.MetadataUuid, OwnerOrganization = d.OwnerOrganization, RequiredRole = d.RequiredRole,
                Files = d.Files.Select(f => new api.File { Id = f.Id, FileName = f.FileName }).ToList()
            }
            ).FirstOrDefaultAsync();

        if(dataset == null) 
        {
            dataset = await _context.Datasets.Where(d => d.Id == id && (user.Roles.Contains(d.RequiredRole))).Select(
            d => new api.Dataset
            {
                Id = d.Id,
                Title = d.Title,
                ContactEmail = d.ContactEmail,
                ContactName = d.ContactName,
                MetadataUuid = d.MetadataUuid,
                OwnerOrganization = d.OwnerOrganization,
                RequiredRole = d.RequiredRole,
                Files = d.Files.Where(u => u.UploaderUsername == user.Username).Select(f => new api.File { Id = f.Id, FileName = f.FileName }).ToList()
            }
            ).FirstOrDefaultAsync();
        }
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

        var dataset = new Dataset
        {
            Title = datasetNew.Title,
            MetadataUuid = datasetNew.MetadataUuid,
            ContactEmail = datasetNew.ContactEmail,
            ContactName = datasetNew.ContactName,
            OwnerOrganization = datasetNew.OwnerOrganization,
            RequiredRole = datasetNew.RequiredRole
        };
        await _context.Datasets.AddAsync(dataset);
        await _context.SaveChangesAsync();

        return new api.Dataset { Id = dataset.Id, Title = dataset.Title, ContactEmail = dataset.ContactEmail,
            ContactName = dataset.ContactName, MetadataUuid=dataset.MetadataUuid, OwnerOrganization = dataset.OwnerOrganization, RequiredRole = dataset.RequiredRole };

    }

    public async Task<api.Dataset> UpdateDataset(int id, api.DatasetUpdate datasetUpdated)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        if (!user.IsAdmin)
            throw new AuthorizationException("Brukeren har ikke tilgang");

        var dataset = await _context.Datasets.Where(d => d.Id == id).FirstOrDefaultAsync();

        if (dataset == null)
            throw new Exception("Datasettet finnes ikke");

        if(!string.IsNullOrEmpty(datasetUpdated.ContactName))
            dataset.ContactName = datasetUpdated.ContactName;

        if (!string.IsNullOrEmpty(datasetUpdated.ContactEmail))
            dataset.ContactEmail = datasetUpdated.ContactEmail;

        if (!string.IsNullOrEmpty(datasetUpdated.OwnerOrganization))
            dataset.OwnerOrganization = datasetUpdated.OwnerOrganization;

        if (!string.IsNullOrEmpty(datasetUpdated.Title))
            dataset.Title = datasetUpdated.Title;

        if (!string.IsNullOrEmpty(datasetUpdated.MetadataUuid))
            dataset.MetadataUuid = datasetUpdated.MetadataUuid;

        if (!string.IsNullOrEmpty(datasetUpdated.RequiredRole))
            dataset.RequiredRole = datasetUpdated.RequiredRole;

        _context.SaveChanges();

        return new api.Dataset 
        { 
            Id = dataset.Id, Title = dataset.Title,
            ContactEmail = dataset.ContactEmail, ContactName = dataset.ContactName,
            MetadataUuid = dataset.MetadataUuid, RequiredRole = dataset.RequiredRole,
            OwnerOrganization=dataset.OwnerOrganization
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
            return new api.File { Id = file.Id, FileName = file.FileName };
        }

        if (user.HasRole(Role.Editor))
        {
            if(file.Dataset.OwnerOrganization == user.OrganizationName)
                return new api.File { Id = file.Id, FileName = file.FileName };
            else if(file.UploaderUsername == user.Username)
                return new api.File { Id = file.Id, FileName = file.FileName };
        }

        throw new AuthorizationException("Manglende tilgang til ressursen");
    }
    public async Task<api.File> AddFile(api.FileNew fileInfo, IFormFile file)
    {
        User user = await _authService.GetUser();

        if(user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var dataset = _context.Datasets.Where((d) => d.Id == fileInfo.datasetId && ((user.Roles.Contains(d.RequiredRole)) || user.IsAdmin)).FirstOrDefault();

        if (dataset == null)
            throw new Exception("Datasett ikke tilgjengelig");

        var fileNew = new Geonorge.OpplastingsApi.Models.Entity.File
        {
            FileName = file.FileName,
            Date = DateTime.Now,
            Status = "Lastet opp",
            UploaderOrganization = user.OrganizationName,
            UploaderPerson = user.Name,
            UploaderEmail = user.Email,
            UploaderUsername = user.Username
        };


        dataset.Files.Add(fileNew);
        //todo check error save
        await _context.SaveChangesAsync();
        api.File fileAdded = new api.File();
        fileAdded.Id = fileNew.Id;
        fileAdded.Dataset = new api.Dataset { Id = dataset.Id, Title = dataset.Title };

        string uploads = Path.Combine(_config.Path, dataset.MetadataUuid);
        if (file.Length > 0)
        {
            string filePath = Path.Combine(uploads, file.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        _notificationService.SendEmailUploadedFileToContact(fileNew);

        return fileAdded;
    }
    public async Task<api.File> UpdateFile(int id, api.FileUpdate fileUpdated, IFormFile file)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var fileData = await _context.Files.Where(f => f.Id == fileUpdated.datasetId).Include(d => d.Dataset).FirstOrDefaultAsync();

        if (!user.IsAdmin 
            || !(user.HasRole(Role.Editor ) && fileData.Dataset.OwnerOrganization == user.OrganizationName)
            || !(user.HasRole(Role.Editor) && fileData.UploaderUsername == user.Username)
            )
        {
            throw new AuthorizationException("Brukeren har ikke tilgang");
        }

        var currentStatus = fileData.Status;

        if (fileData != null)
        {
            if(!string.IsNullOrEmpty(fileUpdated.FileName))
                fileData.FileName = fileUpdated.FileName;
            if (!string.IsNullOrEmpty(fileUpdated.Status))
                fileData.Status = fileUpdated.Status;

            _context.Files.Update(fileData);
            _context.SaveChangesAsync();

            if(currentStatus != fileUpdated.Status)
                _notificationService.SendEmailStatusChangedToUploader(fileData);
        }

        //todo update file in folder

        return new api.File { Id = fileData.Id, FileName = fileData.FileName };
    }

    public async Task<api.File> RemoveFile(int id)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var file = await _context.Files.Where(f => f.Id == id).Include(d => d.Dataset).FirstOrDefaultAsync();

        if (!user.IsAdmin
             || !(user.HasRole(Role.Editor) && file.Dataset.OwnerOrganization == user.OrganizationName)
             || !(user.HasRole(Role.Editor) && file.UploaderUsername == user.Username)
             )
        {
            throw new AuthorizationException("Brukeren har ikke tilgang");
        }


        if (file != null) { 
            _context.Files.Remove(file);
            _context.SaveChangesAsync();
        }

        //todo remove file from folder

        return new api.File {  Id = file.Id, FileName = file.FileName};
    }

    public async Task<api.File> FileStatusChange(int fileId, string status)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var file = await _context.Files.Where(f => f.Id == fileId).Include(d => d.Dataset).FirstOrDefaultAsync();

        if (!user.IsAdmin
             || !(user.HasRole(Role.Editor) && file.Dataset.OwnerOrganization == user.OrganizationName)
             || !(user.HasRole(Role.Editor) && file.UploaderUsername == user.Username)
             )
        {
            throw new AuthorizationException("Brukeren har ikke tilgang");
        }

        if (file != null)
        {
            file.Status = status;

            _context.Files.Update(file);
            _context.SaveChangesAsync();
        }

        _notificationService.SendEmailStatusChangedToUploader(file);

        return new api.File { Id = file.Id, FileName = file.FileName, Status = file.Status };
    }

    public async Task<string> DownloadFile(int id)
    {
        User user = await _authService.GetUser();

        if (user == null)
            throw new UnauthorizedAccessException("Brukeren har ikke tilgang");

        var file = await _context.Files.Where(f => f.Id == id).Include(d => d.Dataset).FirstOrDefaultAsync();

        if (file == null)
            throw new Exception("Filen ble ikke funnnet");

        if (!user.IsAdmin
             || !(user.HasRole(Role.Editor) && file.Dataset.OwnerOrganization == user.OrganizationName)
             || !(user.HasRole(Role.Editor) && file.UploaderUsername == user.Username)
             )
        {
            throw new AuthorizationException("Brukeren har ikke tilgang");
        }

        if (user.HasRole(Role.Editor) && file.Dataset.OwnerOrganization == user.OrganizationName || user.IsAdmin)
        {
            var currentStatus = file.Status;

            if (currentStatus == "Sendt inn")
            {
                file.Status = "I progress";
                _context.Files.Update(file);
                _context.SaveChangesAsync();

                _notificationService.SendEmailStatusChangedToUploader(file);
            }
        }

        return file.FileName; // todo stream file
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
    Task<string> DownloadFile(int id);
    Task<api.File> AddFile(api.FileNew fileInfo, IFormFile file);
    Task<api.File> UpdateFile(int id, api.FileUpdate fileInfo, IFormFile file);
    Task<api.File> RemoveFile(int id);

    Task<api.File> FileStatusChange(int fileId, string status);
}