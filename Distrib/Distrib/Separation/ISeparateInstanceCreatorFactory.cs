using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    /// <summary>
    /// Factory for <see cref="ISeparateInstanceCreator"/>
    /// </summary>
    public interface ISeparateInstanceCreatorFactory
    {
        /// <summary>
        /// Create a new separate instance creator
        /// </summary>
        /// <returns>The <see cref="ISeparateInstanceCreator"/></returns>
        ISeparateInstanceCreator CreateCreator();
    }
}
