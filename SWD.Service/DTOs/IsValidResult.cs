using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD.Service.DTOs
{
    public record IsValidResult(bool Valid, string Message, StudentClass? Data = null);
}
