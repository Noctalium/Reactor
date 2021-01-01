using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Reactor.Extensions
{
    /// <summary>
    /// General utilities
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Stops <paramref name="obj"/> from being destroyed
        /// </summary>
        /// <param name="obj">Object to stop from being destroyed</param>
        /// <returns>Passed <paramref name="obj"/></returns>
        public static T DontDestroy<T>(this T obj) where T : Object
        {
            obj.hideFlags |= HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(obj);

            return obj;
        }

        /// <summary>
        /// Adds a delegate to get one notification when a scene has loaded.
        /// </summary>
        public static void once_sceneLoaded(Action<Scene, LoadSceneMode> value)
        {
            UnityAction<Scene, LoadSceneMode> unityAction = null;

            unityAction = (Action<Scene, LoadSceneMode>) ((scene, loadMode) =>
            {
                SceneManager.remove_sceneLoaded(unityAction);
                value.Invoke(scene, loadMode);
            });

            SceneManager.add_sceneLoaded(unityAction);
        }

        /// <summary>
        /// Returns random <typeparamref name="T"/> from <paramref name="input"/>
        /// </summary>
        public static T Random<T>(this IEnumerable<T> input)
        {
            var list = input as IList<T> ?? input.ToList();
            return list.Count == 0 ? default : list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Returns the color as a hexadecimal string in the format "RRGGBBAA".
        /// </summary>
        /// <param name="color">The color to be converted.</param>
        /// <returns>Hexadecimal string representing the color.</returns>
        /// <remarks>https://docs.unity3d.com/ScriptReference/ColorUtility.ToHtmlStringRGBA.html</remarks>
        public static string ToHtmlStringRGBA(this Color32 color)
        {
            return $"{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}";
        }

        /// <inheritdoc cref="ToHtmlStringRGBA(UnityEngine.Color32)"/>
        public static string ToHtmlStringRGBA(this Color color)
        {
            return ((Color32) color).ToHtmlStringRGBA();
        }

        /// <summary>
        /// Fully read <paramref name="input"/> stream, can be used as workaround for il2cpp streams.
        /// </summary>
        public static byte[] ReadFully(this Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Sends <see cref="UnityWebRequest"/> and return task that finishes on <paramref name="request"/> completion.
        /// </summary>
        public static Task SendAsync(this UnityWebRequest request)
        {
            var task = new TaskCompletionSource<object>();

            request.Send().m_completeCallback = (Action<AsyncOperation>) (x =>
            {
                task.SetResult(null);
            });

            return task.Task;
        }
    }
}