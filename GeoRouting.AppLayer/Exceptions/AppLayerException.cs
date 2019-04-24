using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Exceptions
{
    public class AppLayerException : Exception
    {
        public int Code { get; set; }
        public AppLayerException(int code, string message) : base(message) { Code = code; }
    }
}
