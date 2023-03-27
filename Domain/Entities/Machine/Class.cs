using System.Collections.Generic;
using System.Linq;
using AccountManager.Domain.Entities.Public;

namespace AccountManager.Domain.Entities.Machine
{
    public class Class
    {
        public static string[] ProductionClasses =
            { "Internal Production", "Customer Production", "Customer Staging", "On Premises" };

        private ICollection<MmaInstance> _mmaInstances;

        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<MmaInstance> MmaInstances
        {
            get => _mmaInstances ?? (_mmaInstances = new List<MmaInstance>());
            set => _mmaInstances = value;
        }

        public MmaInstance MmaInstance => MmaInstances?.FirstOrDefault();

        public bool IsProduction
        {
            get
            {
                if (ProductionClasses.Contains(Name)) return true;

                return false;
            }
        }
    }
}