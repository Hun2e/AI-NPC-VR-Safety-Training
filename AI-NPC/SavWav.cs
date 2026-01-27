using System.IO;
using UnityEngine;

public static class SavWav
{
    public static bool Save(string filename, AudioClip clip)
    {
        // 확장자 확인 및 추가
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }

        // 저장 경로 생성
        var filepath = Path.Combine(Application.persistentDataPath, filename);
        Debug.Log($"오디오 저장 경로: {filepath}");

        try
        {
            using (var fileStream = CreateEmpty(filepath))
            {
                ConvertAndWrite(fileStream, clip);
                WriteHeader(fileStream, clip);
            }
            Debug.Log("WAV 파일 저장 성공!");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"WAV 파일 저장 중 오류 발생: {ex.Message}");
            return false;
        }
    }

    private static FileStream CreateEmpty(string filepath)
    {
        // 빈 WAV 헤더 생성
        var fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();
        for (int i = 0; i < 44; i++) // WAV 헤더 크기: 44바이트
        {
            fileStream.WriteByte(emptyByte);
        }
        return fileStream;
    }

    private static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        // 오디오 데이터 가져오기
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // 데이터 유효성 검사
        if (samples == null || samples.Length == 0)
        {
            Debug.LogError("오디오 데이터가 비어 있습니다. 저장을 중단합니다.");
            return;
        }

        // float -> short 변환 (LINEAR16 형식)
        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];

        float rescaleFactor = 32767; // float 범위를 short 범위로 변환

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor); // float -> short
            byte[] byteArr = System.BitConverter.GetBytes(intData[i]); // short -> byte[]
            byteArr.CopyTo(bytesData, i * 2); // byte 데이터 복사
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    private static void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        // WAV 헤더 작성
        fileStream.Seek(0, SeekOrigin.Begin);

        byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        byte[] chunkSize = System.BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        byte[] subChunk1 = System.BitConverter.GetBytes(16); // PCM 형식 서브 청크 크기
        fileStream.Write(subChunk1, 0, 4);

        ushort one = 1; // PCM 형식
        byte[] audioFormat = System.BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        byte[] numChannels = System.BitConverter.GetBytes((ushort)clip.channels);
        fileStream.Write(numChannels, 0, 2);

        byte[] sampleRate = System.BitConverter.GetBytes(clip.frequency);
        fileStream.Write(sampleRate, 0, 4);

        byte[] byteRate = System.BitConverter.GetBytes(clip.frequency * clip.channels * 2);
        fileStream.Write(byteRate, 0, 4);

        ushort blockAlign = (ushort)(clip.channels * 2);
        fileStream.Write(System.BitConverter.GetBytes(blockAlign), 0, 2);

        ushort bps = 16; // 비트 깊이
        byte[] bitsPerSample = System.BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(dataString, 0, 4);

        byte[] subChunk2 = System.BitConverter.GetBytes(clip.samples * clip.channels * 2);
        fileStream.Write(subChunk2, 0, 4);
    }
}
