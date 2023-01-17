using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Osc
{
    public interface IOscUser
    {
        OscPort Server { get; set; }
    }
}