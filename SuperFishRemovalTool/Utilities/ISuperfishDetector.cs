namespace SuperFishRemovalTool.Utilities
{
    public class FixResult
    {
        public FixResult(string name)
        {
            this.NameOfItem = name;
        }
        /// <summary> Whether the detector detected any data to remove.</summary>
        public bool DidExist { get; set; }
        /// <summary>Whether removal of the data was successful.</summary>
        public bool WasRemoved { get; set; }
        /// <summary>The name of the Utility that prduced this item.</summary>
        public string NameOfItem { get; set; }
        /// <summary>Whether either the probing for data, or the atual removal produced an exception.</summary>
        public bool DidFail { get; set; }
    }

    public interface ISuperfishDetector
    {
        /// <summary>Whether the utility has detected data that can be removed.</summary>
        bool DoesExist();
        /// <summary> Performs a removal of the detected Superfish data.</summary>
        /// <returns>Whether the operation was a success.</returns>
        bool Remove();
        /// <summary>The name of the Utility.</summary>
        string UtilityName { get; }
    }
}
