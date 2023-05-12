using Geonorge.OpplastingsApi.Models.Api.User;
using Geonorge.OpplastingsApi.Models.Entity;
using Geonorge.OpplastingsApi.Services;
using Microsoft.EntityFrameworkCore;
using api = Geonorge.OpplastingsApi.Models.Api;

public class DatasetService : IDatasetService
{

    private readonly ApplicationContext _context;
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;

    public DatasetService(ApplicationContext context, IAuthService authService, INotificationService notificationService) 
    {
        _context = context;
        _authService = authService;
        _notificationService = notificationService;
    }

    public async Task<List<api.Dataset>> GetDatasets()
    {
        User user = await _authService.GetUser();

        if (user.IsAdmin) 
        { 
            return await _context.Datasets.Select(
                d => new api.Dataset 
                    {
                        Id = d.Id,
                        Title = d.Title,
                        Files = d.Files.Select(f => new api.File {Id = f.Id, FileName = f.FileName }).ToList()
                    }
                ).ToListAsync();
        }
        else if (user.HasRole(Role.Editor)) 
        {
            return await _context.Datasets.Where(d => d.OwnerOrganization == user.OrganizationName || user.Roles.Contains(d.RequiredRole)).Select(
                d => new api.Dataset
                {
                    Id = d.Id,
                    Title = d.Title,
                    Files = d.Files.Select(f => new api.File { Id = f.Id, FileName = f.FileName }).ToList()
                }
                ).ToListAsync();
        }

        throw new AuthorizationException("Manglende tilgang til ressursen");

    }

    public async Task<api.Dataset> GetDataset(int id)
    {

        User user = await _authService.GetUser();

        var dataset = await _context.Datasets.Where(d => d.Id == id && ((d.OwnerOrganization == user.OrganizationName || user.Roles.Contains(d.RequiredRole)) || user.IsAdmin)).Select(
            d => new api.Dataset
            {
                Id=d.Id,
                Title = d.Title,
                Files = d.Files.Select(f => new api.File { Id = f.Id, FileName = f.FileName }).ToList()
            }
            ).FirstOrDefaultAsync();

        return dataset;
    }

    public async Task<api.Dataset> AddDataset(api.Dataset datasetNew)
    {
        User user = await _authService.GetUser();

        if (!user.IsAdmin)
            throw new AuthorizationException("Brukeren har ikke tilgang");

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

        return new api.Dataset { Id = dataset.Id, Title = dataset.Title };

    }

    public async Task<api.Dataset> UpdateDataset(int id, api.Dataset datasetUpdated)
    {
        User user = await _authService.GetUser();

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

        var files = await _context.Datasets.Where(d => ((d.OwnerOrganization == user.OrganizationName || user.Roles.Contains(d.RequiredRole)) || user.IsAdmin)).Select(d => d.Files).Where(dd => dd.Contains(new Geonorge.OpplastingsApi.Models.Entity.File { Id = id })).FirstOrDefaultAsync();
        var file = files?.FirstOrDefault();

        api.File fileData = null;

        if (file != null) 
        {
            fileData = new api.File {Id = file.Id, FileName = file.FileName };
        }

        return fileData;
    }
    public async Task<api.File> AddFile(api.File fileInfo, IFormFile file)
    {
        User user = await _authService.GetUser();

        if(user == null)
            throw new AuthorizationException("Brukeren har ikke tilgang");

        //todo check user rights

        var dataset = _context.Datasets.Where((d) => d.Id == fileInfo.Dataset.Id && ((d.OwnerOrganization == user.OrganizationName || user.Roles.Contains(d.RequiredRole)) || user.IsAdmin)).Include(ff => ff.Files).FirstOrDefault();

        var fileNew = new Geonorge.OpplastingsApi.Models.Entity.File
        {
            FileName = fileInfo.FileName,
            Date = DateTime.Now,
            Status = "Lastet opp", // todo endre status når eier laster ned => I progress
            UploaderOrganization = user.OrganizationName,
            UploaderPerson = user.Name,
            UploaderEmail = user.Email,
            UploaderUsername = user.Username
        };


        dataset.Files.Add(fileNew);
        //todo check error save
        await _context.SaveChangesAsync();

        fileInfo.Id = fileNew.Id;
        fileInfo.Dataset = new api.Dataset { Id = dataset.Id, Title = dataset.Title };
        fileInfo.Dataset.Files = new List<api.File>();
        foreach (var fileData in dataset.Files)
        {
            fileInfo.Dataset.Files.Add( new api.File { Id = fileData.Id,  FileName = fileData.FileName });
        }

        _notificationService.SendEmailUploadedFileToContact(fileNew);

        return fileInfo;
    }
    public Task<api.File> UpdateFile(int id, api.File fileInfo, IFormFile file)
    {
        //todo check access
        throw new NotImplementedException();
    }

    public Task<api.File> RemoveFile(int id)
    {
        //todo check access
        //todo have Files in _context=
        //var file = _context.Datasets.Where(x => x.Files.Contains(id)).FirstOrDefault();

        throw new NotImplementedException();
    }

    public async Task<api.File> FileStatusChange(int fileId, string status)
    {
        //todo update status
        _notificationService.SendEmailStatusChangedToUploader(new Geonorge.OpplastingsApi.Models.Entity.File());
        return new api.File();
    }
}

public interface IDatasetService
{
    Task<List<api.Dataset>> GetDatasets();
    Task<api.Dataset> GetDataset(int id);
    Task<api.Dataset> AddDataset(api.Dataset dataset);
    Task<api.Dataset> UpdateDataset(int id, api.Dataset dataset);
    Task<api.Dataset> RemoveDataset(int id);

    Task<api.File> GetFile(int id);
    Task<api.File> AddFile(api.File fileInfo, IFormFile file);
    Task<api.File> UpdateFile(int id, api.File fileInfo, IFormFile file);
    Task<api.File> RemoveFile(int id);

    Task<api.File> FileStatusChange(int fileId, string status);
}