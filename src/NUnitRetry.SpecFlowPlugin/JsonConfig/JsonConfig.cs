using System.Runtime.Serialization;

namespace NUnitRetry.SpecFlowPlugin.JsonConfig
{
    public class JsonConfig
    {
        [DataMember(Name = "NRetrySettings")]
        public NRetrySettingsElement NRetrySettings { get; set; }
    }
}
