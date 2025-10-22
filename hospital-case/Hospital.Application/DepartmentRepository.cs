using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application {
    public class DepartmentRepository {
        public void Add(string departmentName, ICollection<Delegate> methods, string errorMessage) {
            if(!departments.ContainsKey(departmentName)) {
                departments.Add(departmentName, new Department(methods, errorMessage));
            }
        }

        public Department Get(string key) {
            return departments[key];
        }

        private Dictionary<string, Department> departments = new();
        public record Department(ICollection<Delegate> method, string onFailureMessage);
    }
}