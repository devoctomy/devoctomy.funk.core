using System;

namespace devoctomy.funk.core.Environment
{

    /// <summary>
    /// Helper functions for the current environment
    /// </summary>
    public class EnvironmentHelpers
    {

        #region public methods

        /// <summary>
        /// Get an environment variable string by its name
        /// </summary>
        /// <param name="iName">Name of the environment variable to get from the system</param>
        /// <param name="iTarget">Target of the environment variable, i.e. Processs / User / Machine</param>
        /// <returns>The value of the required environment variable</returns>
        public static String GetEnvironmentVariable(String iName, 
            EnvironmentVariableTarget iTarget = EnvironmentVariableTarget.Process)
        {
            return (System.Environment.GetEnvironmentVariable(iName, iTarget));
        }

        /// <summary>
        /// Set an emnvironment variable by its name
        /// </summary>
        /// <param name="iName">Name of the environment variable to set</param>
        /// <param name="iValue">Value to set the specified environment variable to</param>
        /// <param name="iTarget">Target of the environment variable, i.e. Processs / User / Machine</param>
        public static void SetEnvironmentVariable(String iName,
            String iValue,
            EnvironmentVariableTarget iTarget = EnvironmentVariableTarget.Process)
        {
            System.Environment.SetEnvironmentVariable(iName, iValue, iTarget);
        }

        #endregion

    }

}
