using mj.gist;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mj.gist {
    public class ComputeKernel<T> where T : Enum {

        public GPUThreads Threads => threads;

        public int GetKernelIndex(T type) {
            return kernelMap[type];
        }

        private Dictionary<T, int> kernelMap = new Dictionary<T, int>();
        private GPUThreads threads;

        public ComputeKernel(ComputeShader cs) {
            kernelMap = Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(t => t, t => cs.FindKernel(t.ToString()));
            threads = ComputeShaderUtil.GetThreadGroupSize(cs, kernelMap.FirstOrDefault().Value);
        }
    }

    public class Kernel {
        public int Index { get { return index; } }
        public uint ThreadX { get { return threadX; } }
        public uint ThreadY { get { return threadY; } }
        public uint ThreadZ { get { return threadZ; } }
        int index;
        uint threadX, threadY, threadZ;

        public Kernel(ComputeShader shader, string key) {
            index = shader.FindKernel(key);
            if (index < 0) {
                Debug.LogWarning("Can't find kernel: " + key);
                return;
            }
            shader.GetKernelThreadGroupSizes(index, out threadX, out threadY, out threadZ);
        }
    }

    public struct GPUThreads {
        public int x;
        public int y;
        public int z;

        public GPUThreads(uint x, uint y, uint z) {
            this.x = (int)x;
            this.y = (int)y;
            this.z = (int)z;
        }
    }

    public static class DirectCompute5_0 {
        //Use DirectCompute 5.0 on DirectX11 hardware.
        public const int MAX_THREAD = 1024;
        public const int MAX_X = 1024;
        public const int MAX_Y = 1024;
        public const int MAX_Z = 64;
        public const int MAX_DISPATCH = 65535;
        public const int MAX_PROCESS = MAX_DISPATCH * MAX_THREAD;
    }
}