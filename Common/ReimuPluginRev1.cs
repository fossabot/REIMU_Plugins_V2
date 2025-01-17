﻿//-----------------------------------------------------------------------
// <copyright file="ReimuPluginRev1.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable 1591

namespace ReimuPlugins.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    /// <summary>
    /// The base class for the classes implementing the REIMU plugin interface (Revision 1).
    /// </summary>
    /// <typeparam name="TColumnKey">The key type of <see cref="ManagedColumnInfo"/>.</typeparam>
    public abstract class ReimuPluginRev1<TColumnKey> : IReimuPluginRev1
        where TColumnKey : struct, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        /// Gets the information about the plugin implemented by the derived class.
        /// See <see cref="IReimuPluginRev1.GetPluginInfo"/> for details.
        /// </summary>
        protected abstract ReadOnlyCollection<string> ManagedPluginInfo { get; }

        /// <summary>
        /// Gets the information about the columns of the REIMU's list view provided by the derived class.
        /// See <see cref="IReimuPluginRev1.GetColumnInfo"/> for details.
        /// </summary>
        /// <remarks>
        /// Actually, I want to use <c>ReadOnlyDictionary</c>, but it is not available for .NET 4.0...
        /// </remarks>
        protected abstract IDictionary<TColumnKey, ColumnInfo> ManagedColumnInfo { get; }

        /// <inheritdoc/>
        public Revision GetPluginRevision()
        {
            return Revision.Rev1;
        }

        /// <inheritdoc/>
        public int GetPluginInfo(int index, IntPtr info, uint size)
        {
            try
            {
                var byteCount = Enc.CP932.GetByteCount(this.ManagedPluginInfo[index]);
                if (info == IntPtr.Zero)
                {
                    return byteCount - 1;   // except a null terminator
                }
                else
                {
                    if (size >= byteCount)
                    {
                        Marshal.Copy(Enc.CP932.GetBytes(this.ManagedPluginInfo[index]), 0, info, byteCount);
                        return byteCount - 1;   // except a null terminator
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            catch (EncoderFallbackException)
            {
            }

            return 0;
        }

        /// <inheritdoc/>
        public ErrorCode GetColumnInfo(out IntPtr info)
        {
            var errorCode = ErrorCode.UnknownError;

            info = IntPtr.Zero;

            try
            {
                var size = Marshal.SizeOf(typeof(ColumnInfo));

                info = Marshal.AllocHGlobal(size * this.ManagedColumnInfo.Count);

                var address = info.ToInt64();
                foreach (var key in Utils.GetEnumerator<TColumnKey>())
                {
                    var pointer = new IntPtr(address);
                    Marshal.StructureToPtr(this.ManagedColumnInfo[key], pointer, false);
                    address += size;
                }

                errorCode = ErrorCode.AllRight;
            }
            catch (OutOfMemoryException)
            {
                errorCode = ErrorCode.NoMemory;
            }
            catch (ArgumentException)
            {
            }
            catch (OverflowException)
            {
            }
            finally
            {
                if (errorCode != ErrorCode.AllRight)
                {
                    Marshal.FreeHGlobal(info);
                    info = IntPtr.Zero;
                }
            }

            return errorCode;
        }

        /// <inheritdoc/>
        public abstract uint IsSupported(IntPtr src, uint size);

        /// <inheritdoc/>
        public abstract ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info);

        /// <inheritdoc/>
        public abstract ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst);

        /// <inheritdoc/>
        public abstract ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst);

        /// <inheritdoc/>
        public abstract ErrorCode EditDialog(IntPtr parent, string file);

        /// <inheritdoc/>
        public abstract ErrorCode ConfigDialog(IntPtr parent);

        /// <summary>
        /// Creates a new instance of the <see cref="MemoryStream"/> or <see cref="FileStream"/> class.
        /// </summary>
        /// <param name="src">
        /// Same as the parameter <c>src</c> of <see cref="IReimuPluginRev1.IsSupported"/> etc.
        /// </param>
        /// <param name="size">
        /// Same as the parameter <c>size</c> of <see cref="IReimuPluginRev1.IsSupported"/> etc.
        /// </param>
        /// <returns>A pair of the error code and the created instance.</returns>
        /// <exception cref="OutOfMemoryException">Failed to create a new instance.</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "These objects are wrapped in the IDisposable return value.")]
        protected static DisposableTuple<ErrorCode, Stream> CreateStream(IntPtr src, uint size)
        {
            var errorCode = ErrorCode.UnknownError;
            Stream stream = null;

            if (size > 0)
            {
                try
                {
                    var content = new byte[size];
                    Marshal.Copy(src, content, 0, content.Length);
#pragma warning disable IDISP001 // Dispose created.
                    stream = new MemoryStream(content, false);
#pragma warning restore IDISP001 // Dispose created.
                }
                catch (OutOfMemoryException)
                {
                    errorCode = ErrorCode.NoMemory;
                }
                catch (ArgumentNullException)
                {
                }
            }
            else
            {
                try
                {
                    var path = Marshal.PtrToStringAnsi(src);
#pragma warning disable IDISP001 // Dispose created.
                    stream = new FileStream(path, FileMode.Open, FileAccess.Read);
#pragma warning restore IDISP001 // Dispose created.
                }
                catch (ArgumentException)
                {
                    errorCode = ErrorCode.FileReadError;
                }
                catch (IOException)
                {
                    errorCode = ErrorCode.FileReadError;
                }
                catch (NotSupportedException)
                {
                    errorCode = ErrorCode.FileReadError;
                }
                catch (SecurityException)
                {
                    errorCode = ErrorCode.FileReadError;
                }
                catch (UnauthorizedAccessException)
                {
                    errorCode = ErrorCode.FileReadError;
                }
            }

            if (stream != null)
            {
                errorCode = ErrorCode.AllRight;
            }

            try
            {
                return DisposableTuple.Create(errorCode, stream);
            }
            catch (OutOfMemoryException)
            {
                if (stream != null)
                {
                    stream.Dispose();
                }

                throw;
            }
        }
    }
}
