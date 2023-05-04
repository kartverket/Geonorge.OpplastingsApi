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
        return await _context.Datasets.Select(
            d => new api.Dataset 
                {
                    Title = d.Title,
                    Files = d.Files.Select(f => new api.File { FileName = f.FileName }).ToList()
                }
            ).ToListAsync();
    }
}

public interface IDatasetService
{
    Task<List<api.Dataset>> GetDatasets();
}