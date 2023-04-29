using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface IExpertService
    {
        Task<IEnumerable<ExpertHeader>> GetAll();
    }
}
