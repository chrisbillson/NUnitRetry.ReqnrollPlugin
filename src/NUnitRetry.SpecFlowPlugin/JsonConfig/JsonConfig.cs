using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NUnitRetry.SpecFlowPlugin.JsonConfig
{
    public class JsonConfig
    {
        [DataMember(Name = "NRetrySettings")]
        public NRetrySettingsElement NRetrySettings { get; set; }
    }
}
