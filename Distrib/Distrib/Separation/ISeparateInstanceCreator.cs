using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    public interface ISeparateInstanceCreator
    {
        object CreateInstance(Type type, object[] args);
    }
}
