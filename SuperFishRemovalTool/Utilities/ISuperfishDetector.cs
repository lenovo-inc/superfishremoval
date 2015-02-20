using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool.Utilities
{
    public class FixResult
    {
        public FixResult(string name)
        {
            this.NameOfItem = name;
        }
        public bool DidExist { get; set; }
        public bool WasRemoved { get; set; }
        public string NameOfItem { get; set; }
        public bool DidFail { get; set; }
    }

    public interface ISuperfishDetector
    {
        bool DoesExist();
        bool Remove();
        FixResult RemoveItem();
    }
}
