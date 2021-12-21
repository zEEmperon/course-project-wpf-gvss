using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gvss_project
{
    class InvalidArgument : Exception
    {
        public InvalidArgument() : base("Invalid argument!") { }
    }

    class InvalidSensor : Exception {
        public InvalidSensor() : base("Invalid sensor") { }
    }

    class UnexpectedError : Exception {
        public UnexpectedError() : base("UnexpectedError") { }
    }
    class NotFoundException: Exception
    {
        public NotFoundException() : base("Not found") { }
    }
    class SensorRepeatsException: Exception {
        public SensorRepeatsException() : base("The sensor repeats") { }
    }
}
