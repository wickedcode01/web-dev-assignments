using System.Threading.Tasks;

namespace ClinicAppointment.Services
{
    public interface IWaitTimeService
    {
        Task<int> GetCurrentWaitTimeAsync();
    }
} 