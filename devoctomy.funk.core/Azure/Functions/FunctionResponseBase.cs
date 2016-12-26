using devoctomy.funk.core.Environment;
using devoctomy.funk.core.JSON;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.funk.core.Azure.Functions
{

    public class FunctionResponseBase
    {

        #region private objects

        private DateTime cDTeStarted = DateTime.UtcNow;
        private DateTime cDTeFinished;

        #endregion

        #region public properties

        public DateTime StartedAt
        {
            get { return (cDTeStarted); }
        }

        public DateTime FinishedAt
        {
            get { return (cDTeFinished); }
        }

        public TimeSpan Ellapsed
        {
            get { return (FinishedAt - StartedAt); }
        }

        #endregion

        #region public methods

        public void Finish()
        {
            cDTeFinished = DateTime.UtcNow;
        }

        public String ToJSON(Boolean iFinish,
            HttpStatusCode iStatusCode,
            IJSONSerialisable iReturnValue,
            Newtonsoft.Json.Formatting iFormatting)
        {
            if (iFinish) Finish();
            JObject pJOtJSON = new JObject();
            JObject pJOtStatistics = new JObject();
            pJOtStatistics.Add("StartedAt", new JValue(StartedAt.ToString(EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"))));
            pJOtStatistics.Add("FinishedAt", new JValue(StartedAt.ToString(EnvironmentHelpers.GetEnvironmentVariable("DateTimeFormat"))));
            pJOtStatistics.Add("Ellapsed", new JValue(Ellapsed.ToString()));
            pJOtJSON.Add("Statistics", pJOtStatistics);
            JObject pJOtResponse = new JObject();
            pJOtResponse.Add("StatusCode", new JValue((Int32)iStatusCode));
            pJOtResponse.Add("ReturnValue", iReturnValue.ToJObject());
            pJOtJSON.Add("Response", pJOtResponse);
            return (pJOtJSON.ToString(iFormatting));
        }

        #endregion

    }

}
