﻿//-----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th155Replay
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.InteropServices;
    using ReimuPlugins.Common;
    using RGiesecke.DllExport;
    using IO = System.IO;

    public static class Plugin
    {
        private static readonly PluginImpl Impl = new PluginImpl();

        [DllExport]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "To comply with the REIMU plugin spec.")]
        public static Revision GetPluginRevision()
            => Impl.GetPluginRevision();

        [DllExport]
        public static int GetPluginInfo(int index, IntPtr info, uint size)
            => Impl.GetPluginInfo(index, info, size);

        [DllExport]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetColumnInfo(out IntPtr info)
            => Impl.GetColumnInfo(out info);

        [DllExport]
        public static uint IsSupported(IntPtr src, uint size)
            => Impl.IsSupported(src, size);

        [DllExport]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info)
            => Impl.GetFileInfoList(src, size, out info);

        [DllExport]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst)
            => Impl.GetFileInfoText1(src, size, out dst);

        [DllExport]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        public static ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
            => Impl.GetFileInfoText2(src, size, out dst);

        private sealed class PluginImpl : ReimuPluginRev1<PluginImpl.ColumnKey>
        {
            private const string ValidSignature = "TFRP\0";

            private static readonly string[] PluginInfoImpl =
            {
                "REIMU Plug-in For 東方憑依華 Ver2.00 (C) IIHOSHI Yoshinori, 2018\0",
                "東方憑依華\0",
                "*.rep\0",
                "東方憑依華 リプレイファイル (*.rep)\0",
            };

            private static readonly Dictionary<ColumnKey, ColumnInfo> Columns =
                new Dictionary<ColumnKey, ColumnInfo>
                {
                    {
                        ColumnKey.Filename,
                        new ColumnInfo
                        {
                            Title = "ファイル名\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.Title,
                        }
                    },
                    {
                        ColumnKey.LastWriteDate,
                        new ColumnInfo
                        {
                            Title = "更新日時\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.LastWriteTime,
                        }
                    },
                    {
                        ColumnKey.Version,
                        new ColumnInfo
                        {
                            Title = "バージョン\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.GameMode,
                        new ColumnInfo
                        {
                            Title = "モード\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Difficulty,
                        new ColumnInfo
                        {
                            Title = "難易度\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Master1,
                        new ColumnInfo
                        {
                            Title = "マスター 1\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.MasterColor1,
                        new ColumnInfo
                        {
                            Title = "マスター 色 1\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Slave1,
                        new ColumnInfo
                        {
                            Title = "スレイブ 1\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.SlaveColor1,
                        new ColumnInfo
                        {
                            Title = "スレイブ 色 1\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.SpellCard1,
                        new ColumnInfo
                        {
                            Title = "スペルカード 1\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Player1,
                        new ColumnInfo
                        {
                            Title = "プレイヤー 1\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Master2,
                        new ColumnInfo
                        {
                            Title = "マスター 2\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.MasterColor2,
                        new ColumnInfo
                        {
                            Title = "マスター 色 2\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Slave2,
                        new ColumnInfo
                        {
                            Title = "スレイブ 2\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.SlaveColor2,
                        new ColumnInfo
                        {
                            Title = "スレイブ 色 2\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.SpellCard2,
                        new ColumnInfo
                        {
                            Title = "スペルカード 2\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Player2,
                        new ColumnInfo
                        {
                            Title = "プレイヤー 2\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Background,
                        new ColumnInfo
                        {
                            Title = "ステージ\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.Bgm,
                        new ColumnInfo
                        {
                            Title = "BGM\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.DateTime,
                        new ColumnInfo
                        {
                            Title = "日時\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                    {
                        ColumnKey.FileSize,
                        new ColumnInfo
                        {
                            Title = "ファイルサイズ\0",
                            Align = TextAlign.Right,
                            Sort = SortType.Integer,
                            System = SystemInfoType.FileSize,
                        }
                    },
                    {
                        ColumnKey.Directory,
                        new ColumnInfo
                        {
                            Title = "ディレクトリ\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.Directory,
                        }
                    },
                    {
                        ColumnKey.Sentinel,
                        new ColumnInfo
                        {
                            Title = "\0",
                            Align = TextAlign.Left,
                            Sort = SortType.String,
                            System = SystemInfoType.None,
                        }
                    },
                };

            private static readonly Dictionary<ColumnKey, Func<ReplayDataAdapter, string>> FileInfoGetters =
                InitializeFileInfoGetters();

            internal enum ColumnKey
            {
                Filename = 0,
                LastWriteDate,
                Version,
                GameMode,
                Difficulty,
                Master1,
                MasterColor1,
                Slave1,
                SlaveColor1,
                SpellCard1,
                Player1,
                Master2,
                MasterColor2,
                Slave2,
                SlaveColor2,
                SpellCard2,
                Player2,
                Background,
                Bgm,
                DateTime,
                FileSize,
                Directory,
                Sentinel,
            }

            protected override ReadOnlyCollection<string> ManagedPluginInfo
                => Array.AsReadOnly(PluginInfoImpl);

            protected override IDictionary<PluginImpl.ColumnKey, ColumnInfo> ManagedColumnInfo
                => Columns;

            public override uint IsSupported(IntPtr src, uint size)
            {
                if (src == IntPtr.Zero)
                {
                    return (uint)ValidSignature.Length;
                }

                var signature = string.Empty;

                try
                {
                    using (var pair = ReimuPluginRev1<ColumnKey>.CreateStream(src, size))
                    {
                        if (pair.Item1 == ErrorCode.AllRight)
                        {
                            using (var reader = new IO.BinaryReader(pair.Item2, Enc.UTF8NoBOM, true))
                            {
                                var readSize = Math.Min((int)reader.BaseStream.Length, ValidSignature.Length);
                                signature = Enc.CP932.GetString(reader.ReadBytes(readSize));
                            }
                        }
                    }
                }
                catch (OutOfMemoryException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (IO.IOException)
                {
                }
                catch (NotSupportedException)
                {
                }
                catch (ObjectDisposedException)
                {
                }

                return (signature == ValidSignature) ? 1u : 0u;
            }

            public override ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info)
            {
                var errorCode = ErrorCode.UnknownError;

                info = IntPtr.Zero;

                try
                {
                    var pair = CreateReplayData(src, size);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        var fileInfoSize = Marshal.SizeOf(typeof(FileInfo));
                        var keys = Utils.GetEnumerator<ColumnKey>().Where(key => key != ColumnKey.Sentinel);

                        info = Marshal.AllocHGlobal(fileInfoSize * keys.Count());

                        var address = info.ToInt64();
                        foreach (var key in keys)
                        {
                            var fileInfo = new FileInfo { Text = string.Empty };
                            if (FileInfoGetters.TryGetValue(key, out Func<ReplayDataAdapter, string> getter))
                            {
                                fileInfo.Text = getter(pair.Item2);
                            }

                            var pointer = new IntPtr(address);
                            Marshal.StructureToPtr(fileInfo, pointer, false);
                            address += fileInfoSize;
                        }
                    }

                    errorCode = pair.Item1;
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

            public override ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst)
            {
                var errorCode = ErrorCode.UnknownError;

                dst = IntPtr.Zero;

                try
                {
                    var pair = CreateReplayData(src, size);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        var bytes = Enc.CP932.GetBytes(pair.Item2.GetPlayer1Info().ToCStr());
                        dst = Marshal.AllocHGlobal(bytes.Length);
                        Marshal.Copy(bytes, 0, dst, bytes.Length);
                    }

                    errorCode = pair.Item1;
                }
                catch (OutOfMemoryException)
                {
                    errorCode = ErrorCode.NoMemory;
                }
                catch (ArgumentException)
                {
                }
                finally
                {
                    if (errorCode != ErrorCode.AllRight)
                    {
                        Marshal.FreeHGlobal(dst);
                        dst = IntPtr.Zero;
                    }
                }

                return errorCode;
            }

            public override ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst)
            {
                var errorCode = ErrorCode.UnknownError;

                dst = IntPtr.Zero;

                try
                {
                    var pair = CreateReplayData(src, size);
                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        var bytes = Enc.CP932.GetBytes(pair.Item2.GetPlayer2Info().ToCStr());
                        dst = Marshal.AllocHGlobal(bytes.Length);
                        Marshal.Copy(bytes, 0, dst, bytes.Length);
                    }

                    errorCode = pair.Item1;
                }
                catch (OutOfMemoryException)
                {
                    errorCode = ErrorCode.NoMemory;
                }
                catch (ArgumentException)
                {
                }
                finally
                {
                    if (errorCode != ErrorCode.AllRight)
                    {
                        Marshal.FreeHGlobal(dst);
                        dst = IntPtr.Zero;
                    }
                }

                return errorCode;
            }

            public override ErrorCode EditDialog(IntPtr parent, string file)
            {
                throw new NotImplementedException();
            }

            public override ErrorCode ConfigDialog(IntPtr parent)
            {
                throw new NotImplementedException();
            }

            [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed.")]
            private static Dictionary<ColumnKey, Func<ReplayDataAdapter, string>> InitializeFileInfoGetters()
            {
                return new Dictionary<ColumnKey, Func<ReplayDataAdapter, string>>
                {
                    { ColumnKey.Version,      (data) => data.Version },
                    { ColumnKey.GameMode,     (data) => data.GetGameMode() },
                    { ColumnKey.Difficulty,   (data) => data.GetDifficulty() },
                    { ColumnKey.Master1,      (data) => data.GetMasterName1() },
                    { ColumnKey.MasterColor1, (data) => data.GetMasterColor1() },
                    { ColumnKey.Slave1,       (data) => data.GetSlaveName1() },
                    { ColumnKey.SlaveColor1,  (data) => data.GetSlaveColor1() },
                    { ColumnKey.SpellCard1,   (data) => data.GetSpellCard1Name() },
                    { ColumnKey.Player1,      (data) => data.GetPlayer1Name() },
                    { ColumnKey.Master2,      (data) => data.GetMasterName2() },
                    { ColumnKey.MasterColor2, (data) => data.GetMasterColor2() },
                    { ColumnKey.Slave2,       (data) => data.GetSlaveName2() },
                    { ColumnKey.SlaveColor2,  (data) => data.GetSlaveColor2() },
                    { ColumnKey.SpellCard2,   (data) => data.GetSpellCard2Name() },
                    { ColumnKey.Player2,      (data) => data.GetPlayer2Name() },
                    { ColumnKey.Background,   (data) => data.GetBackgroundName() },
                    { ColumnKey.Bgm,          (data) => data.GetBgmName() },
                    { ColumnKey.DateTime,     (data) => data.GetDateTime() },
                };
            }

            private static Tuple<ErrorCode, ReplayDataAdapter> CreateReplayData(IntPtr src, uint size)
            {
                using (var pair = ReimuPluginRev1<ColumnKey>.CreateStream(src, size))
                {
                    ReplayDataAdapter replay = null;

                    if (pair.Item1 == ErrorCode.AllRight)
                    {
                        replay = new ReplayDataAdapter(pair.Item2);
                    }

                    return Tuple.Create(pair.Item1, replay);
                }
            }
        }
    }
}
