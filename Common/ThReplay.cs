﻿//-----------------------------------------------------------------------
// <copyright file="ThReplay.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Indicates the Touhou replay file format.
    /// </summary>
    public class ThReplay
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThReplay"/> class.
        /// </summary>
        public ThReplay()
        {
        }

        /// <summary>
        /// Gets the information.
        /// </summary>
        public string Info
        {
            get
            {
                return this.UserInfo0.DataString;
            }
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment
        {
            get
            {
                return this.UserInfo1.DataString;
            }

            set
            {
                this.UserInfo1.DataString = value;
            }
        }

        /// <summary>
        /// Gets the replay data.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
        protected Replay ReplayData { get; private set; }

        /// <summary>
        /// Gets the user information indicating the replay file information.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
        protected UserInfo UserInfo0 { get; private set; }

        /// <summary>
        /// Gets the user information indicating the comment.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
        protected UserInfo UserInfo1 { get; private set; }

        /// <summary>
        /// Reads from the specified stream.
        /// </summary>
        /// <param name="input">An input stream.</param>
        public void Read(Stream input)
        {
            var reader = new BinaryReader(input);
            this.ReplayData.ReadFrom(reader);
            this.UserInfo0.ReadFrom(reader);
            this.UserInfo1.ReadFrom(reader);
        }

        /// <summary>
        /// Writes to the specified stream.
        /// </summary>
        /// <param name="output">An output stream.</param>
        public void Write(Stream output)
        {
            var writer = new BinaryWriter(output);
            this.ReplayData.WriteTo(writer);
            this.UserInfo0.WriteTo(writer);
            this.UserInfo1.WriteTo(writer);
            writer.Flush();
        }

        /// <summary>
        /// Indicates a replay data.
        /// </summary>
        protected class Replay
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Replay"/> class.
            /// </summary>
            public Replay()
            {
            }

            /// <summary>
            /// Gets the signature string.
            /// </summary>
            public byte[] Signature { get; private set; }

            /// <summary>
            /// Gets an unknown data.
            /// </summary>
            public byte[] Unknown1 { get; private set; }

            /// <summary>
            /// Gets the size of the replay header.
            /// <remarks>
            /// The replay header consists of:
            /// Signature, Unknown1, HeaderSize, Unknown2, the size of Data, and Unknown3.
            /// </remarks>
            /// </summary>
            public int HeaderSize { get; private set; }

            /// <summary>
            /// Gets an unknown data.
            /// </summary>
            public byte[] Unknown2 { get; private set; }

            /// <summary>
            /// Gets an unknown data.
            /// </summary>
            public byte[] Unknown3 { get; private set; }

            /// <summary>
            /// Gets the actual replay data.
            /// </summary>
            public byte[] Data { get; private set; }

            /// <summary>
            /// Reads data by using the specified <see cref="BinaryReader"/> instance.
            /// </summary>
            /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
            public void ReadFrom(BinaryReader reader)
            {
                this.Signature = reader.ReadBytes(4);
                this.Unknown1 = reader.ReadBytes(8);
                this.HeaderSize = reader.ReadInt32();
                this.Unknown2 = reader.ReadBytes(12);
                var dataSize = reader.ReadInt32();
                this.Unknown3 = reader.ReadBytes(4);
                this.Data = reader.ReadBytes(dataSize);
            }

            /// <summary>
            /// Writes data by using the specified <see cref="BinaryWriter"/> instance.
            /// </summary>
            /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
            public void WriteTo(BinaryWriter writer)
            {
                writer.Write(this.Signature);
                writer.Write(this.Unknown1);
                writer.Write(this.HeaderSize);
                writer.Write(this.Unknown2);
                writer.Write(this.Data.Length);
                writer.Write(this.Unknown3);
                writer.Write(this.Data);
            }
        }

        /// <summary>
        /// Indicates an user information stored in the replay file.
        /// </summary>
        protected class UserInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UserInfo"/> class.
            /// </summary>
            public UserInfo()
            {
            }

            /// <summary>
            /// Gets the signature string.
            /// </summary>
            public byte[] Signature { get; private set; }

            /// <summary>
            /// Gets the type of the user information. (0: replay file information, 1: comment)
            /// </summary>
            public int InfoType { get; private set; }

            /// <summary>
            /// Gets the actual data.
            /// </summary>
            public byte[] Data { get; private set; }

            /// <summary>
            /// Gets or sets the actual data represented as a code page 932 string.
            /// </summary>
            public string DataString
            {
                get
                {
                    return Enc.CP932.GetString(this.Data);
                }

                set
                {
                    this.Data = Enc.CP932.GetBytes(value);
                }
            }

            /// <summary>
            /// Reads data by using the specified <see cref="BinaryReader"/> instance.
            /// </summary>
            /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
            public void ReadFrom(BinaryReader reader)
            {
                this.Signature = reader.ReadBytes(4);
                var size = reader.ReadInt32();
                this.InfoType = reader.ReadInt32();
                this.Data = reader.ReadBytes(size);
            }

            /// <summary>
            /// Writes data by using the specified <see cref="BinaryWriter"/> instance.
            /// </summary>
            /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
            public void WriteTo(BinaryWriter writer)
            {
                writer.Write(this.Signature);
                writer.Write(this.Data.Length);
                writer.Write(this.InfoType);
                writer.Write(this.Data);
            }
        }
    }
}
