using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.Exceptions
{
    // Thông báo khi tỉnh đang được dự báo
        public class ForecastInProgressException : Exception
    {
        public ForecastInProgressException(string message)
            : base(message)
        {
        }        
    }
}