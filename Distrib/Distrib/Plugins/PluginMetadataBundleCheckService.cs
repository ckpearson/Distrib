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
