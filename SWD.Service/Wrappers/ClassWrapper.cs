using SWD.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.Wrappers
{
    public class ClassWrapper
    {
        private readonly CECMSContext _db = CECMSContext.Instance;

        /// <summary>Read(in classId, out class)</summary>
        public Class? Read(string classId)
            => _db.Classes.FirstOrDefault(c => c.ClassId == classId);

        public List<Class> ReadAll() => _db.Classes;
    }
}
