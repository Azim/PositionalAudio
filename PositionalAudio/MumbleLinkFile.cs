using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace mumblelib
{

    using wchar = UInt16;

    class Text
    {
        public static Encoding Encoding = Encoding.Unicode;
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct Frame
    {
        public uint uiVersion;

        public uint uiTick;

        public fixed float fAvatarPosition[3];

        public fixed float fAvatarFront[3];

        public fixed float fAvatarTop[3];

        public fixed wchar name[256];

        public fixed float fCameraPosition[3];

        public fixed float fCameraFront[3];

        public fixed float fCameraTop[3];

        public fixed wchar id[256];

        public uint context_len;

        public fixed byte context[256];

        public fixed wchar description[2048];

        public void SetName(string name)
        {
            fixed (Frame* ptr = &this)
            {
                byte[] bytes = Text.Encoding.GetBytes(name + "\u0000");
                Marshal.Copy(bytes, 0, new IntPtr(ptr->name), bytes.Length);
            }
        }

        public void SetDescription(string desc)
        {
            fixed (Frame* ptr = &this)
            {
                byte[] bytes = Text.Encoding.GetBytes(desc + "\u0000");
                Marshal.Copy(bytes, 0, new IntPtr(ptr->description), bytes.Length);
            }
        }

        public void SetID(string id)
        {
            fixed (Frame* ptr = &this)
            {
                byte[] bytes = Text.Encoding.GetBytes(id + "\u0000");
                Marshal.Copy(bytes, 0, new IntPtr(ptr->id), bytes.Length);
            }
        }

        public void SetContext(string context)
        {
            fixed (Frame* ptr = &this)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(context);
                this.context_len = (uint)Math.Min(256, bytes.Length);
                Marshal.Copy(bytes, 0, new IntPtr(ptr->context), (int)this.context_len);
            }
        }
    }

    public unsafe class MumbleLinkFile : IDisposable
    {

        /// <summary>Holds a reference to the shared memory block.</summary>
        private readonly MemoryMappedFile memoryMappedFile;

        private readonly Frame* ptr;

        /// <summary>Indicates whether this object is disposed.</summary>
        private bool disposed;

        private static string LinkFileName()
        {
            return "MumbleLink";
        }

        public MumbleLinkFile(MemoryMappedFile memoryMappedFile)
        {
            this.memoryMappedFile = memoryMappedFile ?? throw new ArgumentNullException("memoryMappedFile");
            byte* tmp = null;
            memoryMappedFile.CreateViewAccessor().SafeMemoryMappedViewHandle.AcquirePointer(ref tmp);
            ptr = (Frame*)tmp;
        }

        public Frame* FramePtr()
        {
            return ptr;
        }

        public static MumbleLinkFile CreateOrOpen()
        {
            return new MumbleLinkFile(MemoryMappedFile.CreateOrOpen(LinkFileName(), Marshal.SizeOf<Frame>()));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void assertNotDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                memoryMappedFile.Dispose();
            }
            disposed = true;
        }
    }
}
