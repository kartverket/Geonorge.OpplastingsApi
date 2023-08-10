using Geonorge.OpplastingsApi.Models.Api;

namespace Geonorge.OpplastingsApi.HttpClients
{
    public interface IValidatorHttpClient
    {
        Task<ValidationReport?> ValidateAsync(IFormFile file);
    }
}
