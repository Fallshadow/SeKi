using System;
using System.Runtime.CompilerServices;

public unsafe struct IntPtrEx
{
    public IntPtr ptr;
    private readonly uint version;

    public IntPtrEx(IntPtr p)
    {
        ptr = p;
        version = ExtensionMethods.IntPtrVersion;
    }

    public IntPtrEx(byte* p)
    {
        ptr = new IntPtr(p);
        version = ExtensionMethods.IntPtrVersion;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* static_cast<T>() where T : unmanaged
    {
        AssertPtr();
        return (T*)ptr.ToPointer();
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void AssertPtr()
    {
        if (version != ExtensionMethods.IntPtrVersion)
        {
            throw new Exception($"检查出野指针");
        }
    }
}
