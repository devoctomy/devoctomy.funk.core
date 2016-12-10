using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Azure.Functions
{

    public class FunctionLimiterLimit
    {

        #region public enums

        public enum SourcePoint
        {
            none = 0,
            requesterip = 1,
            sessiontoken = 2
        }

        public enum LimitFrequency
        {
            none = 0,
            seconds = 1,
            minutes = 2,
            hours = 3,
            days = 4,
            weeks = 5,
            months = 6,
            years = 7
        }

        #endregion

        #region private objects

        private SourcePoint cSPtSource = SourcePoint.none;
        private LimitFrequency cLFyFrequency = LimitFrequency.none;
        private Int32 cIntFrequencyCount = 1;
        private Int32 cIntLimit = 1;

        #endregion

        #region public properties

        public SourcePoint Source
        {
            get { return (cSPtSource); }
        }

        public LimitFrequency Frequency
        {
            get { return (cLFyFrequency); }
        }

        public Int32 FrequencyCount
        {
            get { return (cIntFrequencyCount); }
        }

        public Int32 Limit
        {
            get { return (cIntLimit); }
        }

        #endregion

        #region constructor / destructor

        private FunctionLimiterLimit()
        { }

        #endregion

        #region public methods

        public static FunctionLimiterLimit FromJSON(JObject iJSON)
        {
            FunctionLimiterLimit pFLLLimit = new FunctionLimiterLimit();
            pFLLLimit.cSPtSource = (SourcePoint)Enum.Parse(typeof(SourcePoint), iJSON["Source"].Value<String>());
            pFLLLimit.cLFyFrequency = (LimitFrequency)Enum.Parse(typeof(LimitFrequency), iJSON["Frequency"].Value<String>());
            pFLLLimit.cIntFrequencyCount = iJSON["Count"].Value<Int32>();
            pFLLLimit.cIntLimit = iJSON["Limit"].Value<Int32>();
            return (pFLLLimit);
        }

        public static FunctionLimiterLimit FromJSON(String iJSON)
        { 
            JObject pJOtLimit = JObject.Parse(iJSON);
            return (FromJSON(pJOtLimit));
        }

        #endregion

    }

}
