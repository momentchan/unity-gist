using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace mj.gist {
    static class ComputeShaderExtensions {
        public static GPUThreads GetThreadGroupSize(this ComputeShader compute, int kernel) {
            uint threadX, threadY, threadZ;
            compute.GetKernelThreadGroupSizes(kernel, out threadX, out threadY, out threadZ);
            return new GPUThreads(threadX, threadY, threadZ);
        }
        public static void DispatchThreads
          (this ComputeShader compute, int kernel, int x, int y, int z) {
            uint xc, yc, zc;
            compute.GetKernelThreadGroupSizes(kernel, out xc, out yc, out zc);

            x = (x + (int)xc - 1) / (int)xc;
            y = (y + (int)yc - 1) / (int)yc;
            z = (z + (int)zc - 1) / (int)zc;

            compute.Dispatch(kernel, x, y, z);
        }
        public static void Dispatch1D(this ComputeShader compute, int kernel, int count) {
            uint tx, ty, tz;
            compute.GetKernelThreadGroupSizes(kernel, out tx, out ty, out tz);
            compute.Dispatch(kernel, GetKernelBlock(count, (int)tx), (int)ty, (int)tz);
        }
        public static void Dispatch2D(this ComputeShader compute, int kernel, int width, int height) {
            uint tx, ty, tz;
            compute.GetKernelThreadGroupSizes(kernel, out tx, out ty, out tz);
            compute.Dispatch(kernel, GetKernelBlock(width, (int)tx), GetKernelBlock(height, (int)ty), 1);
        }
        public static void Dispatch3D(this ComputeShader compute, int kernel, int width, int height, int depth) {
            uint tx, ty, tz;
            compute.GetKernelThreadGroupSizes(kernel, out tx, out ty, out tz);
            compute.Dispatch(kernel, GetKernelBlock(width, (int)tx), GetKernelBlock(height, (int)ty), GetKernelBlock(depth, (int)tz));
        }
        static int GetKernelBlock(int count, int blockSize) => (count + blockSize - 1) / blockSize;
    }
    
    public class ComputeShaderUtil {
        public static void ReleaseBuffer(ComputeBuffer buffer) {
            if (buffer != null) {
                buffer.Release();
                buffer = null;
            }
        }

        public static void SwapBuffer(ref ComputeBuffer ping, ref ComputeBuffer pong) {
            var temp = pong;
            pong = ping;
            ping = temp;
        }

        public static void InitialCheck(int count, GPUThreads gpuThreads) {
            Assert.IsTrue(SystemInfo.graphicsShaderLevel >= 50, "Under the DirectCompute5.0 (DX11 GPU) doesn't work");
            Assert.IsTrue(gpuThreads.x * gpuThreads.y * gpuThreads.z <= DirectCompute5_0.MAX_PROCESS, "Resolution is too heigh");
            Assert.IsTrue(gpuThreads.x <= DirectCompute5_0.MAX_X, "THREAD_X is too large");
            Assert.IsTrue(gpuThreads.y <= DirectCompute5_0.MAX_Y, "THREAD_Y is too large");
            Assert.IsTrue(gpuThreads.z <= DirectCompute5_0.MAX_Z, "THREAD_Z is too large");
            Assert.IsTrue(count <= DirectCompute5_0.MAX_PROCESS, "particleNumber is too large");
        }
    }


    public class ComputeKernel<T> where T : Enum {

        public GPUThreads Threads => threads;

        public int GetKernelIndex(T type) {
            return kernelMap[type];
        }

        private Dictionary<T, int> kernelMap = new Dictionary<T, int>();
        private GPUThreads threads;

        public ComputeKernel(ComputeShader cs) {
            kernelMap = Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(t => t, t => cs.FindKernel(t.ToString()));
            threads = cs.GetThreadGroupSize(kernelMap.FirstOrDefault().Value);
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