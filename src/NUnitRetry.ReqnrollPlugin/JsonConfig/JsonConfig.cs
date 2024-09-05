using System.Runtime.Serialization;

namespace NUnitRetry.ReqnrollPlugin.JsonConfig
{
    public class JsonConfig
    {
        [DataMember(Name = "NRetrySettings")]
        public NRetrySettingsElement NRetrySettings { get; set; }
    }
}
