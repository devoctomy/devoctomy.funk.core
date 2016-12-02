using System;

namespace devoctomy.funk.core.Environment
{

    public class EnvironmentHelpers
    {

        #region public methods

        /// <summary>
        /// Get an environment variable string by it's name.
        /// </summary>
        /// <param name="iName">Name of the environment variable to get from the system.</param>
        /// <param name="iTarget">Target of the environment variable, i.e. Processs / User / Machine.</param>
        /// <returns></returns>
        public static String GetEnvironmentVariable(String iName, 
            EnvironmentVariableTarget iTarget = EnvironmentVariableTarget.Process)
        {
            return (System.Environment.GetEnvironmentVariable(iName, iTarget));
        }

        #endregion

    }

}
