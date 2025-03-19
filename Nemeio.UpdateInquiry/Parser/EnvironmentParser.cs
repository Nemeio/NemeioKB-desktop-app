using System;
using Nemeio.UpdateInquiry.Builders;

namespace Nemeio.UpdateInquiry.Parser
{
    public class EnvironmentParser : IParser<UpdateEnvironment>
    {
        const string PublicContainer = "public";
        const string PrivateContainer = "private";
        const string DevelopEnvironment = "develop";
        const string TestingEnvironment = "testing";
        const string MasterEnvironment = "master";
        const string UpdateTestingEnvironment = "update-testing";

        public UpdateEnvironment Parse(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            switch (data)
            {
                case DevelopEnvironment:
                    return UpdateEnvironment.Develop;
                case TestingEnvironment:
                    return UpdateEnvironment.Testing;
                case UpdateTestingEnvironment:
                    return UpdateEnvironment.UpdateTesting;
                case MasterEnvironment:
                    return UpdateEnvironment.Master;
                default:
                    throw new InvalidOperationException("Not supported environment");
            }
        }

        public string Parse(UpdateEnvironment environment)
        {
            switch (environment)
            {
                case UpdateEnvironment.Develop:
                    return PrivateContainer;
                case UpdateEnvironment.Testing:
                    return PrivateContainer;
                case UpdateEnvironment.UpdateTesting:
                    return PrivateContainer;
                case UpdateEnvironment.Master:
                    return PublicContainer;
                default:
                    throw new InvalidOperationException($"Environment <{environment}> is unknown");
            }
        }
    }
}
