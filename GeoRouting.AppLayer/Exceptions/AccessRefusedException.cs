using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Exceptions
{
    public class AccessRefusedException : AppLayerException
    {
        public AccessRefusedException(int code, string message) : base(code, message) { }
    }
}
