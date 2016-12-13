using devoctomy.funk.core.Membership;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Azure.Functions
{

    /// <summary>
    /// 
    /// </summary>
    public class FunctionLimiter
    {

        #region private objects

        private List<FunctionLimiterFunction> cLisFunctions;
        private Dictionary<String, FunctionLimiterFunction> cDicFunctions;

        #endregion

        #region public properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iFunctionName"></param>
        /// <returns></returns>
        public FunctionLimiterFunction this[String iFunctionName]
        {
            get
            {
                return (cDicFunctions[iFunctionName]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<FunctionLimiterFunction> Functions
        {
            get { return (cLisFunctions); }
        }

        #endregion

        #region constructor / destructor

        /// <summary>
        /// 
        /// </summary>
        private FunctionLimiter()
        {
            cLisFunctions = new List<FunctionLimiterFunction>();
            cDicFunctions = new Dictionary<String, FunctionLimiterFunction>();
        }

        #endregion

        #region public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iJSON"></param>
        /// <returns></returns>
        public static FunctionLimiter FromJSON(String iJSON)
        {
            JObject pJOtLimiter = JObject.Parse(iJSON);
            FunctionLimiter pFLrLimiter = new FunctionLimiter();
            JArray pJAyFunctions = pJOtLimiter["Functions"].Value<JArray>();
            foreach(JObject curFunction in pJAyFunctions)
            {
                FunctionLimiterFunction pFLFCurFunction = FunctionLimiterFunction.FromJSON(curFunction);
                pFLrLimiter.cLisFunctions.Add(pFLFCurFunction);
                pFLrLimiter.cDicFunctions.Add(pFLFCurFunction.Name, pFLFCurFunction);
            }
            return (pFLrLimiter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iStorage"></param>
        /// <param name="iFunctionName"></param>
        /// <param name="iSource"></param>
        /// <returns></returns>
        public Boolean Allowed(Storage iStorage,
            String iFunctionName,
            String iSource)
        {
            if(cDicFunctions.ContainsKey(iFunctionName))
            {
                List<FunctionHit> pLisHits = iStorage.GetHits(iFunctionName,
                    iSource,
                    DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0)));
                FunctionLimiterFunction pFLFFunction = cDicFunctions[iFunctionName];
                foreach(FunctionLimiterLimit curLimit in pFLFFunction.Limits)
                {
                    if(curLimit.Exceeded(pLisHits))
                    {
                        return (false);
                    }
                }
            }
            return (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iFunctionName"></param>
        /// <returns></returns>
        public Boolean IsLimited(String iFunctionName)
        {
            return (cDicFunctions.ContainsKey(iFunctionName));
        }

        #endregion

    }

}
