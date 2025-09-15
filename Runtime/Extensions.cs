using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace com.fscigliano.CommonExtensions
{
    /// <summary>
    /// Creation Date:   03/03/2020 7:33:26
    /// Product Name:    Common extensions
    /// Developers:      Franco Scigliano
    /// Description:     This helper class contains methods to execute coroutines in places that can't execute them, as
    ///                  ScriptableObjects or Editor scripts. Also, it contains several Texture Scaling methods.
    /// </summary>
    public static class Extensions
    {
        private static CoroutineHelper cHelper = null;

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }

        public static Coroutine StartCoroutine(System.Collections.IEnumerator routine)
        {
            if (cHelper == null)
            {
                GameObject o = new GameObject();
                o.name = "CoroutineHelper";
                GameObject.DontDestroyOnLoad(o);
                cHelper = o.AddComponent<CoroutineHelper>();
            }

            var result = cHelper.StartCoroutine(routine);
            return result;
        }

        public static Coroutine StartCoroutine(string methodName)
        {
            if (cHelper == null)
            {
                GameObject o = new GameObject();
                cHelper = o.AddComponent<CoroutineHelper>();
            }

            var result = cHelper.StartCoroutine(methodName);
            return result;
        }

        public static void StopCoroutine(Coroutine routine)
        {
            if (cHelper != null)
            {
                cHelper.StopCoroutine(routine);
            }
        }

        public static void StopCoroutine(string methodName)
        {
            if (cHelper != null)
            {
                cHelper.StopCoroutine(methodName);
            }
        }

        public static void StopCoroutine(System.Collections.IEnumerator routine)
        {
            if (cHelper != null)
            {
                cHelper.StopCoroutine(routine);
            }
        }

        public static void FillWith<T>(this List<T> target, List<T> source)
        {
            target.Clear();
            target.AddRange(source);
        }

        public static bool IsPrefab(this UnityEngine.Component target)
        {
            return string.IsNullOrEmpty(target.gameObject.scene.name);
        }

        private static Color[] texColors;
        private static Color[] newColors;
        private static int w;
        private static float ratioX;
        private static float ratioY;
        private static int w2;
        private static int finishCount;
        private static Mutex mutex;

        public static void TextureScalePoint(Texture2D tex, int newWidth, int newHeight)
        {
            ThreadedScale(tex, newWidth, newHeight, false);
        }

        public static void TextureScaleBilinear(Texture2D tex, int newWidth, int newHeight)
        {
            ThreadedScale(tex, newWidth, newHeight, true);
        }

        private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
        {
            texColors = tex.GetPixels();
            newColors = new Color[newWidth * newHeight];
            if (useBilinear)
            {
                ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
                ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
            }
            else
            {
                ratioX = ((float)tex.width) / newWidth;
                ratioY = ((float)tex.height) / newHeight;
            }

            w = tex.width;
            w2 = newWidth;
            var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
            var slice = newHeight / cores;

            finishCount = 0;
            if (mutex == null)
            {
                mutex = new Mutex(false);
            }

            if (cores > 1)
            {
                int i = 0;
                ThreadData threadData;
                for (i = 0; i < cores - 1; i++)
                {
                    threadData = new ThreadData(slice * i, slice * (i + 1));
                    ParameterizedThreadStart ts = useBilinear
                        ? new ParameterizedThreadStart(BilinearScale)
                        : new ParameterizedThreadStart(PointScale);
                    Thread thread = new Thread(ts);
                    thread.Start(threadData);
                }

                threadData = new ThreadData(slice * i, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }

                while (finishCount < cores)
                {
                    Thread.Sleep(1);
                }
            }
            else
            {
                ThreadData threadData = new ThreadData(0, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
            }

            tex.Reinitialize(newWidth, newHeight);
            tex.SetPixels(newColors);
            tex.Apply();

            texColors = null;
            newColors = null;
        }

        private static void BilinearScale(System.Object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (var y = threadData.start; y < threadData.end; y++)
            {
                int yFloor = (int)Mathf.Floor(y * ratioY);
                var y1 = yFloor * w;
                var y2 = (yFloor + 1) * w;
                var yw = y * w2;

                for (var x = 0; x < w2; x++)
                {
                    int xFloor = (int)Mathf.Floor(x * ratioX);
                    var xLerp = x * ratioX - xFloor;
                    newColors[yw + x] = ColorLerpUnclamped(
                        ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                        ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                        y * ratioY - yFloor);
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        private static void PointScale(System.Object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (var y = threadData.start; y < threadData.end; y++)
            {
                var thisY = (int)(ratioY * y) * w;
                var yw = y * w2;
                for (var x = 0; x < w2; x++)
                {
                    newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
        {
            return new Color(c1.r + (c2.r - c1.r) * value,
                c1.g + (c2.g - c1.g) * value,
                c1.b + (c2.b - c1.b) * value,
                c1.a + (c2.a - c1.a) * value);
        }

        // new one
        /// <summary>
        ///     Returns a scaled copy of given texture.
        /// </summary>
        /// <param name="tex">Source texure to scale</param>
        /// <param name="width">Destination texture width</param>
        /// <param name="height">Destination texture height</param>
        /// <param name="mode">Filtering mode</param>
        public static Texture2D GetScaledTexture(Texture2D src, int width, int height,
            FilterMode mode = FilterMode.Trilinear)
        {
            Rect texR = new Rect(0, 0, width, height);
            _gpu_scale(src, width, height, mode);

            //Get rendered data back to a new texture
            Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, true);
            result.Reinitialize(width, height);
            result.ReadPixels(texR, 0, 0, true);
            return result;
        }

        /// <summary>
        /// Scales the texture data of the given texture.
        /// </summary>
        /// <param name="tex">Texure to scale</param>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="mode">Filtering mode</param>
        public static void ScaleTexture(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            Rect texR = new Rect(0, 0, width, height);
            _gpu_scale(tex, width, height, mode);

            // Update new texture
            tex.Reinitialize(width, height);
            tex.ReadPixels(texR, 0, 0, true);
            tex.Apply(true); //Remove this if you hate us applying textures for you :)
        }

        // Internal unility that renders the source texture into the RTT - the scaling method itself.
        private static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
        {
            //We need the source texture in VRAM because we render with it
            src.filterMode = fmode;
            src.Apply(true);

            //Using RTT for best quality and performance. Thanks, Unity 5
            RenderTexture rtt = new RenderTexture(width, height, 32);

            //Set the RTT in order to render to it
            Graphics.SetRenderTarget(rtt);

            //Setup 2D matrix in range 0..1, so nobody needs to care about sized
            GL.LoadPixelMatrix(0, 1, 1, 0);

            //Then clear & draw the texture to fill the entire RTT.
            GL.Clear(true, true, new Color(0, 0, 0, 0));
            Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
        }

        public class ThreadData
        {
            public int start;
            public int end;

            public ThreadData(int s, int e)
            {
                start = s;
                end = e;
            }
        }

        public static bool ContainsLayer(this LayerMask layerMask, int layer)
        {
            return (layerMask == (layerMask | (1 << layer)));
        }

        public static Texture2D CreateColorTexture(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }

    public class CoroutineHelper : MonoBehaviour
    {
    }
}