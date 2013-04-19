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

namespace Distrib.Storage
{
    /// <summary>
    /// Helper functions for persistence
    /// </summary>
    public static class PersistHelper
    {
        /// <summary>
        /// Generates <see cref="PersistRecords"/> and gets <paramref name="persistable"/> to persist data to it
        /// </summary>
        /// <param name="persistable">The <see cref="IPersist"/>-aware instance</param>
        /// <returns>The <see cref="PersistRecords"/></returns>
        public static PersistRecords GetRecordsFromPersistable(IPersist persistable)
        {
            var pr = new PersistRecords();
            persistable.PersistRecords(pr);
            return pr;
        }
    }
}
