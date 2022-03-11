using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NUnitRetry.Tests.JsonConfig
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
