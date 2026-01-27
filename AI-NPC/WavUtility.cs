using System;
using UnityEngine;

public static class WavUtility
{
    public static AudioClip ToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "GeneratedAudio")
    {
        int channels = BitConverter.ToInt16(fileBytes, 22);
        int sampleRate = BitConverter.ToInt32(fileBytes, 24);
        int samples = BitConverter.ToInt32(fileBytes, 40) / 2;

        AudioClip audioClip = AudioClip.Create(name, samples, channels, sampleRate, false);
        float[] data = new float[samples];
        int dataIndex = 0;

        for (int i = 44; i < fileBytes.Length; i += 2)
        {
            short sample = BitConverter.ToInt16(fileBytes, i);
            data[dataIndex] = sample / 32768f;
            dataIndex++;
        }

        audioClip.SetData(data, offsetSamples);
        return audioClip;
    }
}
