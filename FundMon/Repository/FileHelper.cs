using System;
using System.IO;
using System.Text;

namespace FundMon.Repository;

static class FileHelper
{
    public static int ReadInt(Stream fs)
    {
        byte[] buffer = new byte[4];
        fs.Read(buffer, 0, 4);
        return BitConverter.ToInt32(buffer, 0);
    }

    public static double ReadDouble(Stream fs)
    {
        byte[] buffer = new byte[8];
        fs.Read(buffer, 0, 8);
        return BitConverter.ToDouble(buffer, 0);
    }

    public static string ReadString(Stream fs)
    {
        int length = ReadInt(fs);
        byte[] buffer = new byte[length];
        fs.Read(buffer, 0, length);
        return Encoding.UTF8.GetString(buffer);
    }

    public static void WriteUTF8String(Stream fs, string s)
    {
        byte[] buffer = UTF8Encoding.UTF8.GetBytes(s);
        byte[] bufferLength = BitConverter.GetBytes(buffer.Length);
        fs.Write(bufferLength, 0, bufferLength.Length);
        fs.Write(buffer, 0, buffer.Length);
    }

    public static void WriteInt(Stream fs, int i)
    {
        byte[] buffer = BitConverter.GetBytes(i);
        fs.Write(buffer, 0, buffer.Length);
    }

    public static void WriteDouble(Stream fs, double d)
    {
        byte[] buffer = BitConverter.GetBytes(d);
        fs.Write(buffer, 0, buffer.Length);
    }
}
