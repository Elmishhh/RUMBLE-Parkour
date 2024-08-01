using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime;
using System;
using IL2CPPType = Il2CppInterop.Runtime.Il2CppType;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;
using UniverseLib.Runtime.Il2Cpp;

namespace AssetBundleLoader
{
    public class AssetBundle : UnityEngine.Object
    {
        static AssetBundle()
        {
            ClassInjector.RegisterTypeInIl2Cpp<AssetBundle>();
        }

        // ~~~~~~~~~~~~ Static ~~~~~~~~~~~~

        // AssetBundle.LoadFromFile(string path)

        internal delegate IntPtr d_LoadFromFile(IntPtr path, uint crc, ulong offset);

        [HideFromIl2Cpp]
        public static AssetBundle LoadFromFile(string path)
        {
            IntPtr ptr = ICallManager.GetICallUnreliable<d_LoadFromFile>(
                    "UnityEngine.AssetBundle::LoadFromFile_Internal",
                    "UnityEngine.AssetBundle::LoadFromFile")
                .Invoke(IL2CPP.ManagedStringToIl2Cpp(path), 0u, 0UL);

            return ptr != IntPtr.Zero ? new AssetBundle(ptr) : null;
        }

        // AssetBundle.LoadFromMemory(byte[] binary)

        private delegate IntPtr d_LoadFromMemory(IntPtr binary, uint crc);

        [HideFromIl2Cpp]
        public static AssetBundle LoadFromMemory(byte[] binary, uint crc = 0)
        {
            IntPtr ptr = ICallManager.GetICallUnreliable<d_LoadFromMemory>(
                    "UnityEngine.AssetBundle::LoadFromMemory_Internal",
                    "UnityEngine.AssetBundle::LoadFromMemory")
                .Invoke(((Il2CppStructArray<byte>)binary).Pointer, crc);

            return ptr != IntPtr.Zero ? new AssetBundle(ptr) : null;
        }

        // AssetBundle.GetAllLoadedAssetBundles()

        public delegate IntPtr d_GetAllLoadedAssetBundles_Native();

        [HideFromIl2Cpp]
        public static AssetBundle[] GetAllLoadedAssetBundles()
        {
            IntPtr ptr = ICallManager.GetICall<d_GetAllLoadedAssetBundles_Native>("UnityEngine.AssetBundle::GetAllLoadedAssetBundles_Native")
                .Invoke();

            return ptr != IntPtr.Zero ? (AssetBundle[])new Il2CppReferenceArray<AssetBundle>(ptr) : null;
        }

        // ~~~~~~~~~~~~ Instance ~~~~~~~~~~~~

        public readonly IntPtr m_bundlePtr = IntPtr.Zero;

        public AssetBundle(IntPtr ptr) : base(ptr) { m_bundlePtr = ptr; }

        // LoadAllAssets()

        internal delegate IntPtr d_LoadAssetWithSubAssets_Internal(IntPtr _this, IntPtr name, IntPtr type);

        [HideFromIl2Cpp]
        public UnityEngine.Object[] LoadAllAssets()
        {
            IntPtr ptr = ICallManager.GetICall<d_LoadAssetWithSubAssets_Internal>("UnityEngine.AssetBundle::LoadAssetWithSubAssets_Internal")
                .Invoke(m_bundlePtr, IL2CPP.ManagedStringToIl2Cpp(""), IL2CPPType.Of<UnityEngine.Object>().Pointer);

            return ptr != IntPtr.Zero ? (UnityEngine.Object[])new Il2CppReferenceArray<UnityEngine.Object>(ptr) : new UnityEngine.Object[0];
        }

        // LoadAsset<T>(string name, Type type)

        internal delegate IntPtr d_LoadAsset_Internal(IntPtr _this, IntPtr name, IntPtr type);

        [HideFromIl2Cpp]
        public T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            IntPtr ptr = ICallManager.GetICall<d_LoadAsset_Internal>("UnityEngine.AssetBundle::LoadAsset_Internal")
                .Invoke(m_bundlePtr, IL2CPP.ManagedStringToIl2Cpp(name), IL2CPPType.Of<T>().Pointer);

            return ptr != IntPtr.Zero ? new UnityEngine.Object(ptr).TryCast<T>() : null;
        }

        // Unload(bool unloadAllLoadedObjects);

        internal delegate void d_Unload(IntPtr _this, bool unloadAllLoadedObjects);

        [HideFromIl2Cpp]
        public void Unload(bool unloadAllLoadedObjects)
        {
            ICallManager.GetICall<d_Unload>("UnityEngine.AssetBundle::Unload")
                .Invoke(this.m_bundlePtr, unloadAllLoadedObjects);
        }
    }
}

namespace UniverseLib.Runtime.Il2Cpp
{
    /// <summary>
    /// Helper class for using Unity ICalls (internal calls).
    /// </summary>
    public static class ICallManager
    {
        // cache used by GetICall
        private static readonly Dictionary<string, Delegate> iCallCache = new Dictionary<string, Delegate>();
        // cache used by GetICallUnreliable
        private static readonly Dictionary<string, Delegate> unreliableCache = new Dictionary<string, Delegate>();

        /// <summary>
        /// Helper to get and cache an iCall by providing the signature (eg. "UnityEngine.Resources::FindObjectsOfTypeAll").
        /// </summary>
        /// <typeparam name="T">The Type of Delegate to provide for the iCall.</typeparam>
        /// <param name="signature">The signature of the iCall you want to get.</param>
        /// <returns>The <typeparamref name="T"/> delegate if successful.</returns>
        /// <exception cref="MissingMethodException" />
        public static T GetICall<T>(string signature) where T : Delegate
        {
            if (iCallCache.ContainsKey(signature))
                return (T)iCallCache[signature];

            IntPtr ptr = IL2CPP.il2cpp_resolve_icall(signature);

            if (ptr == IntPtr.Zero)
                throw new MissingMethodException($"Could not find any iCall with the signature '{signature}'!");

            Delegate iCall = Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
            iCallCache.Add(signature, iCall);

            return (T)iCall;
        }

        /// <summary>
        /// Get an iCall which may be one of multiple different signatures (ie, the name changed in different Unity versions).
        /// Each possible signature must have the same Delegate type, it can only vary by name.
        /// </summary>
        public static T GetICallUnreliable<T>(params string[] possibleSignatures) where T : Delegate
        {
            // use the first possible signature as the 'key'.
            string key = possibleSignatures.First();

            if (unreliableCache.ContainsKey(key))
                return (T)unreliableCache[key];

            T iCall;
            IntPtr ptr;
            foreach (string sig in possibleSignatures)
            {
                ptr = IL2CPP.il2cpp_resolve_icall(sig);
                if (ptr != IntPtr.Zero)
                {
                    iCall = (T)Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
                    unreliableCache.Add(key, iCall);
                    return iCall;
                }
            }

            throw new MissingMethodException($"Could not find any iCall from list of provided signatures starting with '{key}'!");
        }
    }
}
