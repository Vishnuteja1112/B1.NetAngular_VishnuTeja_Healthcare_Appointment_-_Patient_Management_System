using AppointmentService.DTOs;
using AppointmentService.Models;
using AppointmentService.Repository;
using AppointmentService.Clients;

namespace AppointmentService.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly DoctorClient _doctorClient;

        public AppointmentService(IAppointmentRepository repo, DoctorClient doctorClient)
        {
            _repo = repo;
            _doctorClient = doctorClient;
        }
        public Task<string> BookAsync(AppointmentDto dto)
        {
            throw new System.NotImplementedException("BookAsync(AppointmentDto) is not implemented. Use BookAsync(dto, patientId) or implement retrieval of patientId.");
        }

        public async Task<string> BookAsync(AppointmentDto dto, int patientId)
        {
            var doctorExists = await _doctorClient.DoctorExistsAsync(dto.DoctorId);

            if (!doctorExists)
                return "Invalid DoctorId. Doctor not found.";

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                Status = AppointmentStatus.Booked
            };

            await _repo.AddAsync(appointment);

            return "Appointment Booked Successfully";
        }

        public async Task<List<Appointment>> GetForPatientAsync(int patientId)
        {
            return await _repo.GetByPatientAsync(patientId);
        }

        public async Task<List<Appointment>> GetForDoctorAsync(int doctorId)
        {
            return await _repo.GetByDoctorAsync(doctorId);
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }
        public async Task<string> CancelByPatientAsync(int id, int patientId)
        {
            var appt = await _repo.GetByIdAsync(id);
            if (appt == null)
                return "Not Found";

            if (appt.PatientId != patientId)
                return "Unauthorized";

            if (appt.Status != AppointmentStatus.Booked)
                return "Only booked appointments can be cancelled";

            appt.Status = AppointmentStatus.Cancelled;
            await _repo.UpdateAsync(appt);

            return "Cancelled by Patient";
        }
        public async Task<string> CancelByDoctorAsync(int id, int doctorId)
        {
            var appt = await _repo.GetByIdAsync(id);
            if (appt == null)
                return "Not Found";

            if (appt.DoctorId != doctorId)
                return "Unauthorized";

            if (appt.Status != AppointmentStatus.Booked)
                return "Only booked appointments can be cancelled";

            appt.Status = AppointmentStatus.Cancelled;
            await _repo.UpdateAsync(appt);

            return "Cancelled by Doctor";
        }

        public async Task<string> CompleteAsync(int id, int doctorId)
        {
            var appt = await _repo.GetByIdAsync(id);
            if (appt == null)
                return "Not Found";

            if (appt.DoctorId != doctorId)
                return "Unauthorized";

            if (appt.Status != AppointmentStatus.Booked)
                return "Only booked appointments can be completed";

            appt.Status = AppointmentStatus.Completed;
            await _repo.UpdateAsync(appt);

            return "Completed";
        }

        public async Task<string> CancelByAdminAsync(int id)
        {
            var appt = await _repo.GetByIdAsync(id);

            if (appt == null)
                return "Appointment not found";

            if (appt.Status != AppointmentStatus.Booked)
                return "Only booked appointments can be cancelled";

            appt.Status = AppointmentStatus.Cancelled;

            await _repo.UpdateAsync(appt);

            return "Appointment cancelled by Admin";
        }
    }
}