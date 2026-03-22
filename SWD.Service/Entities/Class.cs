using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Entities
{
    public class Class
    {
        public string ClassId { get; set; } = Guid.NewGuid().ToString();
        public string ClassName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string ScheduleId { get; set; } = string.Empty;
        public double ConditionScore { get; set; }   // prerequisite min score
    }
}
