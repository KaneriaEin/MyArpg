using System;
using System.Reflection;
using UnityEngine;

public static class EditorAudioUtility
{
    private static MethodInfo playClipMethodInfo;
    private static MethodInfo stopAllClipMethodInfo;
    static EditorAudioUtility()
    {
        Assembly editorAssembly = typeof(UnityEditor.AudioImporter).Assembly;
        Type utilClassType = editorAssembly.GetType(" UnityEditor.AudioUtil");

        playClipMethodInfo = utilClassType.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public, null,
            new Type[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);
        stopAllClipMethodInfo = utilClassType.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public);
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="start">0~1的播放进度</param>
    public static void PlayAudio(AudioClip clip, float start)
    {
        playClipMethodInfo.Invoke(clip, new object[] { clip, (int)(start * clip.frequency), false });
    }

    public static void StopAllAudios()
    {
        stopAllClipMethodInfo.Invoke(null, null);
    }
}
