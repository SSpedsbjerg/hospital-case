using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using static Hospital.Application.DepartmentRepository;


namespace Hospital.Application;

public class AppointmentService
{
    AppointmentRepository appointmentRepository;
    DepartmentRepository departmentRepository;
    public AppointmentService(AppointmentRepository appointmentRepository) {
        this.appointmentRepository = appointmentRepository;
        this.departmentRepository = new();
        departmentRepository.Add("General Practice", [IsAssignedToGP], "[ERROR] Patients can only book appointments with their assigned GP.");
        departmentRepository.Add("Physiotherapy", [HasValidReferral, HasValidFinancialApproval], "[ERROR] Physiotherapy requires a doctor's referral." ); //ToDo: add support for multiple error messages based on functions
        departmentRepository.Add("Surgery", [HasValidReferral], "[ERROR] Surgery requires a specialist referral." ); //ToDo: add support for multiple error messages based on functions
        departmentRepository.Add("Radiology", [HasValidReferral, HasValidFinancialApproval], "[ERROR] Radiology requires a doctor's referral." ); //ToDo: add support for multiple error messages based on functions
    }

    private bool EvaluateAppointmentRules(Appointment appointment) {
        Department departmentRules = departmentRepository.Get(appointment.Department);
        foreach(Delegate departmentRule in departmentRules.method) {
            Dictionary<string, object> methodParameterValues = appointment.GetType()
                .GetProperties()
                .ToDictionary(parameter => parameter.Name.ToLower(), parameter => parameter.GetValue(appointment));
            object[] arguments = departmentRule.Method.GetParameters()
                .Select(parameter => methodParameterValues
                .TryGetValue(parameter.Name.ToLower(), out var value) ? value : null)
                .ToArray();
            bool success = (bool)departmentRule.DynamicInvoke(arguments);
            if(!success) {
                Console.WriteLine(departmentRules.onFailureMessage);
                return false;
            }
        }
        return true;
    }   
    public async Task<bool> ScheduleAppointment(
        string cpr, string patientName, DateTime appointmentDate,
        string department, string doctorName)
    {
        // Basic validation
        if (string.IsNullOrEmpty(cpr) || string.IsNullOrEmpty(department) ||
            string.IsNullOrEmpty(doctorName) || appointmentDate < DateTime.Now)
        {
            Console.WriteLine("[ERROR] Invalid appointment request.");
            return false;
        }

        // Validate CPR before scheduling
        if (!await new NationalRegistryService().ValidateCpr(cpr))
        {
            Console.WriteLine("[ERROR] Invalid CPR number. Cannot schedule appointment.");
            return false;
        }

        Console.WriteLine($"[LOG] Scheduling appointment for {patientName} (CPR: {cpr}) in {department} with {doctorName} on {appointmentDate}");
        Appointment appointment = new() {
            Cpr = cpr,
            PatientName = patientName,
            AppointmentDate = appointmentDate,
            Department = department,
            DoctorName = doctorName
        };

        bool success = EvaluateAppointmentRules(appointment);
        if(success) {
            await appointmentRepository.AddAsync(appointment);
        }
        return true;
    }

    private bool IsAssignedToGP(string cpr, string doctorName)
    {
        Console.WriteLine($"[LOG] Checking GP assignment for {cpr}");
        return true; // Dummy check (To be implemented later)
    }

    private bool CheckDoubleBooking(string cpr, DateTime appointmentDate)
    {
        Console.WriteLine($"[LOG] Checking for double booking for CPR {cpr}");
        return false; // Dummy check (To be replaced later)
    }

    private bool RequiresReferral(string department)
    {
        return department == "Surgery" || department == "Radiology" || department == "Physiotherapy";
    }

    private bool RequiresInsuranceApproval(string department)
    {
        return department == "Physiotherapy";
    }

    private bool RequiresFinancialApproval(string department)
    {
        return department == "Radiology";
    }

    private bool HasValidReferral(string cpr, string department)
    {
        Console.WriteLine($"[LOG] Checking if referral exists for CPR {cpr} in {department}");
        return RequiresReferral(department); // Dummy check (To be replaced later)
    }

    private bool HasValidInsuranceApproval(string cpr, string department)
    {
        Console.WriteLine($"[LOG] Checking if insurance approval exists for CPR {cpr} in {department}");
        return RequiresInsuranceApproval(department); // Dummy check (To be replaced later)
    }

    private bool HasValidFinancialApproval(string cpr, string department)
    {
        Console.WriteLine($"[LOG] Checking if financial approval exists for CPR {cpr} in {department}");
        return RequiresInsuranceApproval(department) || RequiresFinancialApproval(department); // Dummy check (To be replaced later)
    }
}