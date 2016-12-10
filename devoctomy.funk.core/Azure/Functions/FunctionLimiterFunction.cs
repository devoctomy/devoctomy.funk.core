using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Azure.Functions
{

    public class FunctionLimiterFunction
    {

        #region private objects

        private String cStrName = String.Empty;
        private List<FunctionLimiterLimit> cLisLimits;

        #endregion

        #region public properties

        public String Name
        {
            get { return (cStrName); }
        }

        public IReadOnlyList<FunctionLimiterLimit> Limits
        {
            get { return (cLisLimits); }
        }

        #endregion

        #region constructor / destructor

        public FunctionLimiterFunction()
        {
            cLisLimits = new List<FunctionLimiterLimit>();
        }

        #endregion

        #region public methods

        public static FunctionLimiterFunction FromJSON(JObject iJSON)
        {
            FunctionLimiterFunction pFLFFunction = new FunctionLimiterFunction();
            pFLFFunction.cStrName = iJSON["Name"].Value<String>();
            JArray pJAyLimts = iJSON["Limits"].Value<JArray>();
            foreach (JObject curLimit in pJAyLimts)
            {
                FunctionLimiterLimit pFLLCurLimit = FunctionLimiterLimit.FromJSON(curLimit);
                pFLFFunction.cLisLimits.Add(pFLLCurLimit);
            }
            return (pFLFFunction);
        }

        public static FunctionLimiterFunction FromJSON(String iJSON)
        {
            JObject pJOtFunction = JObject.Parse(iJSON);
            return (FromJSON(pJOtFunction));
        }

        #endregion

    }

}
