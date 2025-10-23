using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application {
    public class DepartmentRepository {
        public void Add(string departmentName, IList<Delegate> methods, IList<string> errorMessages) {
            if(!departments.ContainsKey(departmentName)) {
                departments.Add(departmentName, new Department(methods, errorMessages));
            }
        }

        public Department Get(string key) {
            return departments[key];
        }

        private Dictionary<string, Department> departments = new();
        public record Department(IList<Delegate> method, IList<string> onFailureMessages);
    }
}