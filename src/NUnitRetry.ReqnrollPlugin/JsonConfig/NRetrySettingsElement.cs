using System.ComponentModel;
using System.Runtime.Serialization;

namespace NUnitRetry.ReqnrollPlugin.JsonConfig
{
    // Class containing definitions and its default values for config from specflow.json
    public class NRetrySettingsElement
    {
        [DefaultValue(3)]
        [DataMember(Name = "maxRetries")]
        public int MaxRetries { get; set; }

        [DefaultValue(true)]
        [DataMember(Name = "applyGlobally")]
        public bool ApplyGlobally { get; set; }
    }
}
