using devoctomy.funk.core.Membership;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Azure.Functions
{

    public class FunctionLimiter
    {

        #region private objects

        private List<FunctionLimiterFunction> cLisFunctions;
        private Dictionary<String, FunctionLimiterFunction> cDicFunctions;

        #endregion

        #region public properties

        public FunctionLimiterFunction this[String iFunctionName]
        {
            get
            {
                return (cDicFunctions[iFunctionName]);
            }
        }

        public IReadOnlyList<FunctionLimiterFunction> Functions
        {
            get { return (cLisFunctions); }
        }

        #endregion

        #region constructor / destructor

        private FunctionLimiter()
        {
            cLisFunctions = new List<FunctionLimiterFunction>();
            cDicFunctions = new Dictionary<String, FunctionLimiterFunction>();
        }

        #endregion

        #region public methods

        public FunctionLimiter FromJSON(String iJSON)
        {
            JObject pJOtLimiter = JObject.Parse(iJSON);
            FunctionLimiter pFLrLimiter = new FunctionLimiter();
            JArray pJAyFunctions = pJOtLimiter["Functions"].Value<JArray>();
            foreach(JObject curFunction in pJAyFunctions)
            {
                FunctionLimiterFunction pFLFCurFunction = FunctionLimiterFunction.FromJSON(curFunction);
                pFLrLimiter.cLisFunctions.Add(pFLFCurFunction);
                cDicFunctions.Add(pFLFCurFunction.Name, pFLFCurFunction);
            }
            return (pFLrLimiter);
        }

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

        public Boolean IsLimited(String iFunctionName)
        {
            return (cDicFunctions.ContainsKey(iFunctionName));
        }

        #endregion

    }

}
