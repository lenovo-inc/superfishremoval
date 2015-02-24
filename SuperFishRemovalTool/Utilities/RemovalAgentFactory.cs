using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool.Utilities
{
    internal static class RemovalAgentFactory
    {
        /// <summary>
        /// Returns a collection of removal agent instances
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ISuperfishDetector> GetRemovalAgents()
        {
            // Wrap the instantiation in try catch statements to prevent spoilage
            Func<Func<ISuperfishDetector>, ISuperfishDetector> TryInstantiateRemovalAgent = (instantiationFunc) =>
            {
                ISuperfishDetector agent = null;
                try
                {
                    agent = instantiationFunc != null ? instantiationFunc() : null; ;
                }
                catch (Exception ex)
                {
                    Logging.Logger.Log(ex, "Unable to instantiate individual removal agent");
                }
                return agent;
            };

            return new List<Utilities.ISuperfishDetector>()
            {
                TryInstantiateRemovalAgent( () => new Utilities.ApplicationUtility()),
                TryInstantiateRemovalAgent( () => new Utilities.CertificateUtility()),
                TryInstantiateRemovalAgent( () => new Utilities.RegistryUtility()),
                TryInstantiateRemovalAgent( () => new Utilities.FilesDetector()),
                TryInstantiateRemovalAgent( () => new Utilities.MozillaCertificateUtility()),
            };
        }
    }
}
