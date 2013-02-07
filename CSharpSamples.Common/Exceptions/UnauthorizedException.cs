using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpSamples.Common.Exceptions
{
    public class UnauthorizedException: Exception
    {
        public UnauthorizedException(string message)
            : base(message)
        {
        }
    }
}
