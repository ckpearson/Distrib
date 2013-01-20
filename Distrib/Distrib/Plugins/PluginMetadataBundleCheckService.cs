/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
*/
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginMetadataBundleCheckService : IPluginMetadataBundleCheckService
    {
        public Res<List<Tuple<IPluginMetadataBundle, PluginMetadataBundleExistenceCheckResult, string>>> 
            CheckMetadataBundlesFulfilConstraints(IEnumerable<IPluginMetadataBundle> metadataBundles)
        {
            var resList = new List<Tuple<IPluginMetadataBundle, PluginMetadataBundleExistenceCheckResult, string>>();
            bool success = true;

            Action<IEnumerable<IPluginMetadataBundle>, PluginMetadataBundleExistenceCheckResult, string>
                addRange = (bundles, res, msg) =>
                    {
                        lock (resList)
                        {
                            resList.AddRange(bundles.Select(b =>
                                new Tuple<IPluginMetadataBundle, PluginMetadataBundleExistenceCheckResult, string>(
                                    b, res, msg)));
                        }
                    };

            if (metadataBundles == null)
            {
                throw new ArgumentNullException("Metadata bundles must be supplied");
            }

            try
            {
                // Go through each bundle group using the identities
                foreach (var group in metadataBundles.GroupBy(b => b.MetadataBundleIdentity))
                {
                    // The whole group (share an identity) should agree on their existence policy
                    if (group.GroupBy(b => b.MetadataInstanceExistencePolicy).Count() != 1)
                    {
                        addRange(group, PluginMetadataBundleExistenceCheckResult.ExistencePolicyMismatch,
                            "Existence policy must be agreed upon by all bundles in an identity group");

                        success &= false;
                    }
                    else
                    {
                        // Perform the policy checks (use the first in the group as they all agree on policy)
                        switch (group.First().MetadataInstanceExistencePolicy)
                        {
                                // The group should only contain a single instance
                            case PluginMetadataBundleExistencePolicy.SingleInstance:

                                if (group.Count() == 1)
                                {
                                    addRange(group, PluginMetadataBundleExistenceCheckResult.Success,
                                        "Succeeded as policy called for single instance");
                                    success &= true;
                                }
                                else
                                {
                                    addRange(group, PluginMetadataBundleExistenceCheckResult.FailedExistencePolicy,
                                        "Failed as policy called for single instance");
                                    success &= false;
                                }

                                break;

                                // The group should contain more than one instance
                            case PluginMetadataBundleExistencePolicy.MultipleInstances:

                                if (group.Count() > 1)
                                {
                                    addRange(group, PluginMetadataBundleExistenceCheckResult.Success,
                                        "Succeeded as policy called for multiple instances");
                                    success &= true;
                                }
                                else
                                {
                                    addRange(group, PluginMetadataBundleExistenceCheckResult.FailedExistencePolicy,
                                        "Failed as policy called for multiple instances");
                                    success &= false;
                                }

                                break;

                                // The policy simply isn't important, treat all instances as OK
                            default:
                            case PluginMetadataBundleExistencePolicy.NotImportant:

                                addRange(group, PluginMetadataBundleExistenceCheckResult.Success,
                                    "Succeeded as policy not important");
                                success &= true;
                                break;
                        }
                    }
                }

                return new Res<List<Tuple<IPluginMetadataBundle, PluginMetadataBundleExistenceCheckResult, string>>>
                    (success &= true, resList);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to check metadata bundle constraints", ex);
            }
        }
    }
}
