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

        public FunctionLimiterFunction this[String iKey]
        {
            get
            {
                return (null);
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
            }
            return (pFLrLimiter);
        }

        #endregion

    }

}
