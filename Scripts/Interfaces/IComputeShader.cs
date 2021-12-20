using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist {
    interface IComputeShaderUser  {
        void InitBuffers();
        void InitKernels();
    }
}