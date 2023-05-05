using Geonorge.OpplastingsApi.Models.Api.User;
using Geonorge.OpplastingsApi.Models.Entity;
using Microsoft.EntityFrameworkCore;
using api = Geonorge.OpplastingsApi.Models.Api;

public class DatasetService : IDatasetService
{

    private readonly ApplicationContext _context;

    public DatasetService(ApplicationContext context) 
    {
            _context = context;
    }

    public async Task<List<api.Dataset>> GetDatasets()
    {
        User user = GetUser(); //Todo add service

        if (user.IsAdmin) 
        { 
            return await _context.Datasets.Select(
                d => new api.Dataset 
                    {
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
                    Title = d.Title,
                    Files = d.Files.Select(f => new api.File { FileName = f.FileName }).ToList()
                }
                ).ToListAsync();
        }

        return null;

    }

    public Task<api.Dataset> GetDataset(int id)
    {
        throw new NotImplementedException();
    }

    public Task<api.Dataset> AddDataset(api.Dataset dataset)
    {
        throw new NotImplementedException();
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

    private User GetUser()
    {
        //test data
        return new User { OrganizationName = "Kartverket2", Roles = new List<string>() { Role.Editor, "nd.datast1" } };
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