using System;

namespace SuperFishRemovalTool.Utilities
{
    public static class SuperfishDetectorExtension
    {
        /// <summary>
        /// Detects if a removal of Superfish needs to be made, then attempts to 
        /// perfom a removal using the specified utility.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="superfishDetector">The utility to use</param>
        /// <returns>The result of the removal</returns>
        public static FixResult RemoveItem(this ISuperfishDetector superfishDetector, bool DetectExistenceOnly = false)
        {
            if (superfishDetector == null)
                throw new ArgumentNullException("superfishDetector");

            string name = null;
            try
            {
                name = superfishDetector.UtilityName;
            }
            catch
            {
                //TODO: Log
            }
            finally
            {
                if (name == null)
                {
                    name = "";
                }
            }

            var result = new FixResult(name);
            try
            {
                result.DidExist = superfishDetector.DoesExist();

                //TODO: This functionality should be discussed, 
                //Performing a removal if nothing was found strikes me as odd.
                if (DetectExistenceOnly)
                {
                    result.WasRemoved = false;
                }
                else
                {
                    result.WasRemoved = superfishDetector.Remove();
                }
            }
            catch (Exception)
            {
                result.DidFail = true;
            }
            return result;
        }

    }
}
