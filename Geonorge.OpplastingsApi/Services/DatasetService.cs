using Geonorge.OpplastingsApi.Models.Api.User;
using Geonorge.OpplastingsApi.Models.Entity;
using Geonorge.OpplastingsApi.Services;
using Microsoft.EntityFrameworkCore;
using api = Geonorge.OpplastingsApi.Models.Api;

public class DatasetService : IDatasetService
{

    private readonly ApplicationContext _context;
    private readonly IAuthService _authService;

    public DatasetService(ApplicationContext context, IAuthService authService) 
    {
        _context = context;
        _authService = authService;
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
                        Files = d.Files.Select(f => new api.File { FileName = f.FileName }).ToList()
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
                    Files = d.Files.Select(f => new api.File { FileName = f.FileName }).ToList()
                }
                ).ToListAsync();
        }

        throw new UnauthorizedAccessException();

    }

    public async Task<api.Dataset> GetDataset(int id)
    {

        User user = await _authService.GetUser();

        var dataset = await _context.Datasets.Where(d => d.Id == id && (d.OwnerOrganization == user.OrganizationName || user.Roles.Contains(d.RequiredRole)) || user.IsAdmin).Select(
            d => new api.Dataset
            {
                Id=d.Id,
                Title = d.Title,
                Files = d.Files.Select(f => new api.File { FileName = f.FileName }).ToList()
            }
            ).FirstOrDefaultAsync();

        return dataset;
    }

    public async Task<api.Dataset> AddDataset(api.Dataset datasetNew)
    {
        var dataset = new Dataset
        {
            Title = datasetNew.Title,
            ContactEmail = "utvikling@arkitektum.no",
            Status = "Sendt inn",
            ContactName = "Dag",
            MetadataUuid = "xxxxxxxxxxxxxxxxxxxxxxxx",
            OwnerOrganization = "Kartverket",
            RequiredRole = "nd.datasett1"
        };
        await _context.Datasets.AddAsync(dataset);
        await _context.SaveChangesAsync();

        return new api.Dataset { Id = dataset.Id, Title = dataset.Title };

    }

    public Task<api.Dataset> UpdateDataset(api.Dataset dataset)
    {
        throw new NotImplementedException();
    }

    public Task<api.Dataset> RemoveDataset(api.Dataset dataset)
    {
        throw new NotImplementedException();
    }

    public Task<api.File> GetFile(int id)
    {
        throw new NotImplementedException();
    }
    public Task<api.File> AddFile(api.File fileInfo, IFormFile file)
    {
        throw new NotImplementedException();
    }
    public Task<api.File> UpdateFile(api.File fileInfo, IFormFile file)
    {
        throw new NotImplementedException();
    }

    public Task<api.File> RemoveFile(int id)
    {
        throw new NotImplementedException();
    }
}

public interface IDatasetService
{
    Task<List<api.Dataset>> GetDatasets();
    Task<api.Dataset> GetDataset(int id);
    Task<api.Dataset> AddDataset(api.Dataset dataset);
    Task<api.Dataset> UpdateDataset(api.Dataset dataset);
    Task<api.Dataset> RemoveDataset(api.Dataset dataset);

    Task<api.File> GetFile(int id);
    Task<api.File> AddFile(api.File fileInfo, IFormFile file);
    Task<api.File> UpdateFile(api.File fileInfo, IFormFile file);
    Task<api.File> RemoveFile(int id);
}