using System.Runtime.Serialization;

namespace NUnitRetry.Tests.JsonConfig
{
    public class JsonConfig
    {
        [DataMember(Name = "NRetrySettings")]
        public NRetrySettingsElement NRetrySettings { get; set; }
    }
}
