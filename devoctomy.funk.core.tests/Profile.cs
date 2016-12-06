using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace devoctomy.funk.core.tests
{
    [TestClass]
    public class Profile
    {

        #region private objects

        private String cStrDefaultProfile = String.Empty;

        #endregion

        #region public methods

        [TestInitialize()]
        public void Init()
        {
            //Load default profile into cStrDefaultProfile here
            cStrDefaultProfile = File.ReadAllText(@"Assets\ProfileDefaults.json");
        }

        [TestMethod]
        public void InitDefaultProfile()
        {
            Membership.Profile pProProfile = new Membership.Profile(cStrDefaultProfile);

            String pop = pProProfile.ToJSON(Newtonsoft.Json.Formatting.Indented);

        }

        #endregion


    }
}
