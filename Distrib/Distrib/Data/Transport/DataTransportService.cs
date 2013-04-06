/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distrib.IOC;

namespace Distrib.Data.Transport
{
    public sealed class DataTransportService :
        IDataTransportService
    {
        private readonly IDataTransportPointFactory _pointFactory;

        public DataTransportService(
            [IOC(true)] IDataTransportPointFactory pointFactory)
        {
            _pointFactory = pointFactory;
        }

        public TRight MapLTR<TLeft, TRight>(TLeft left, Func<TRight> rightCreator)
            where TLeft : class
            where TRight : class
        {
            if (left == null) throw Ex.ArgNull(() => left);
            if (rightCreator == null) throw Ex.ArgNull(() => rightCreator);

            TRight right = null;

            try
            {
                try
                {
                    right = rightCreator();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to use creation function for right value", ex);
                }

                var leftDataPoints = _pointFactory.GetDataPointsFromPropertiesOnInstance(left).ToArray();
                var rightDataPoints = _pointFactory.GetDataPointsFromPropertiesOnInstance(right).ToArray();



                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to map left to right", ex);
            }
        }

        public TRight MapLTR<TLeft, TRight>(TLeft left, TRight right)
            where TLeft : class
            where TRight : class
        {
            return MapLTR<TLeft, TRight>(left, () => right);
        }
    }
}
