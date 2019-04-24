using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Exceptions
{
    public class BadInputException : AppLayerException
    {
        public BadInputException(int code, string message) : base(code, message) { }
    }
}
