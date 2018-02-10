using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static PolarisBiosEditor.PolarisBiosEditorHelper;

namespace PolarisBiosEditor
{
    public class PolarisBiosEditorData
    {
        /* DATA */

        public string[] manufacturers = new string[] 
        {
            "SAMSUNG",
            "ELPIDA",
            "HYNIX",
            "MICRON"
        };

        public string[] supportedDeviceID = new string[] { "67DF", "67EF", "1002", "67FF", "699F" };

        public string[] timings = new string[] 
        {
    		// FIXME Try UberMix 3.2 Timings:
    		// 777000000000000022CC1C00CEE55C46C0590E1532CD66090060070014051420FA8900A00300000012123442C3353C19
    		// UberMix 3.1
    		"777000000000000022CC1C00AD615C41C0590E152ECC8608006007000B031420FA8900A00300000010122F3FBA354019",
          //"777000000000000022CC1C00AD615C41C0590E152ECCA60B006007000B031420FA8900A00300000010122F3FBA354019", // new, please test
           	// UberMix 2.3 (less extreme)
    		"777000000000000022CC1C00AD615B41C0570E152DCB7409006007000B031420FA8900A00300000010123A46DB354019",
    		// 1750/2000MHz Mix Timings
    		"777000000000000022CC1C00106A6D4DD0571016B90D060C006AE70014051420FA8900A0030000001E123A46DB354019",
    		// 1625/2000MHz Mix Timings
    		"777000000000000022CC1C00CE616C47D0570F15B48C250B006AE7000B031420FA8900A0030000001E123A46DB354019",

            // Good HYNIX_2
    		"777000000000000022AA1C00B56A6D46C0551017BE8E060C006AE6000C081420EA8900AB030000001B162C31C0313F17",
         		
            // Good Micron
    	  //"777000000000000022AA1C0073626C41B0551016BA0D260B006AE60004061420EA8940AA030000001914292EB22E3B16", old
            "777000000000000022AA1C0073626C41B0551016BA0D260B0060060004061420EA8940AA030000001914292EB22E3B16", // new tested timings (much better xmr performance @ rx560 sapphire pulse)
             
    		// Good Hynix_1
    		"999000000000000022559D0010DE5B4480551312B74C450A00400600750414206A8900A00200312010112D34A42A3816",

    		// Good Elpida (fixed with version 1.6.4, see issue #19)
    		"777000000000000022AA1C00315A5B36A0550F15B68C1506004082007C041420CA8980A9020004C01712262B612B3715"
          //"777000000000000022AA1C00AC615B3CA0550F142C8C1506006004007C041420CA8980A9020004C01712262B612B3715" // new, please test
        };

        public Dictionary<string, string> rc { get; set; } = new Dictionary<string, string>();

        [StructLayout(LayoutKind.Explicit, Size = 96, CharSet = CharSet.Ansi)]
        public class VRAM_TIMING_RX
        {

        }

        public Byte[] Buffer { get; set; }
        public Int32Converter int32 { get; set; } = new Int32Converter();
        public UInt32Converter uint32 { get; set; } = new UInt32Converter();

        public string deviceID = "";
        public bool hasInternetAccess = false;

        public int atom_rom_checksum_offset { get; set; } = 0x21;
        public int atom_rom_header_ptr { get; set; } = 0x48;
        public int atom_rom_header_offset { get; set; }


        public ATOM_ROM_HEADER atom_rom_header;


        public ATOM_DATA_TABLES atom_data_table;

        public int atom_powerplay_offset { get; set; }
        public ATOM_POWERPLAY_TABLE atom_powerplay_table;

        public int atom_powertune_offset { get; set; }
        public ATOM_POWERTUNE_TABLE atom_powertune_table;

        public int atom_fan_offset { get; set; }
        public ATOM_FAN_TABLE atom_fan_table;

        public int atom_mclk_table_offset { get; set; }
        public ATOM_MCLK_TABLE atom_mclk_table { get; set; }
        public ATOM_MCLK_ENTRY[] atom_mclk_entries { get; set; }

        public int atom_sclk_table_offset { get; set; }
        public ATOM_SCLK_TABLE atom_sclk_table { get; set; }
        public ATOM_SCLK_ENTRY[] atom_sclk_entries { get; set; }

        public int atom_vddc_table_offset { get; set; }
        public ATOM_VOLTAGE_TABLE atom_vddc_table { get; set; }
        public ATOM_VOLTAGE_ENTRY[] atom_vddc_entries { get; set; }

        public int atom_vram_info_offset { get; set; }
        public ATOM_VRAM_INFO atom_vram_info;
        public ATOM_VRAM_ENTRY[] atom_vram_entries;

        private ATOM_VRAM_TIMING_ENTRY[] atom_vram_timing_entries;
        public ATOM_VRAM_TIMING_ENTRY[] Atom_vram_timing_entries
        {
            get { return atom_vram_timing_entries; }
            set { atom_vram_timing_entries = value; }
        }

        public int atom_vram_index { get; set; } = 0;
        public const int MAX_VRAM_ENTRIES = 48; // e.g. MSI-Armor-RX-580-4GB has 36 entries
        public int atom_vram_timing_offset { get; set; }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_COMMON_TABLE_HEADER
        {
            Int16 usStructureSize;
            Byte ucTableFormatRevision;
            Byte ucTableContentRevision;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_ROM_HEADER
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            //public UInt32 uaFirmWareSignature;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
            public Char[] uaFirmWareSignature;
            public UInt16 usBiosRuntimeSegmentAddress;
            public UInt16 usProtectedModeInfoOffset;
            public UInt16 usConfigFilenameOffset;
            public UInt16 usCRC_BlockOffset;
            public UInt16 usBIOS_BootupMessageOffset;
            public UInt16 usInt10Offset;
            public UInt16 usPciBusDevInitCode;
            public UInt16 usIoBaseAddress;
            public UInt16 usSubsystemVendorID;
            public UInt16 usSubsystemID;
            public UInt16 usPCI_InfoOffset;
            public UInt16 usMasterCommandTableOffset;
            public UInt16 usMasterDataTableOffset;
            public Byte ucExtendedFunctionCode;
            public Byte ucReserved;
            public UInt32 ulPSPDirTableOffset;
            public UInt16 usVendorID;
            public UInt16 usDeviceID;
        }

        public string BIOS_BootupMessage { get; set; }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_DATA_TABLES
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            public UInt16 UtilityPipeLine;
            public UInt16 MultimediaCapabilityInfo;
            public UInt16 MultimediaConfigInfo;
            public UInt16 StandardVESA_Timing;
            public UInt16 FirmwareInfo;
            public UInt16 PaletteData;
            public UInt16 LCD_Info;
            public UInt16 DIGTransmitterInfo;
            public UInt16 SMU_Info;
            public UInt16 SupportedDevicesInfo;
            public UInt16 GPIO_I2C_Info;
            public UInt16 VRAM_UsageByFirmware;
            public UInt16 GPIO_Pin_LUT;
            public UInt16 VESA_ToInternalModeLUT;
            public UInt16 GFX_Info;
            public UInt16 PowerPlayInfo;
            public UInt16 GPUVirtualizationInfo;
            public UInt16 SaveRestoreInfo;
            public UInt16 PPLL_SS_Info;
            public UInt16 OemInfo;
            public UInt16 XTMDS_Info;
            public UInt16 MclkSS_Info;
            public UInt16 Object_Header;
            public UInt16 IndirectIOAccess;
            public UInt16 MC_InitParameter;
            public UInt16 ASIC_VDDC_Info;
            public UInt16 ASIC_InternalSS_Info;
            public UInt16 TV_VideoMode;
            public UInt16 VRAM_Info;
            public UInt16 MemoryTrainingInfo;
            public UInt16 IntegratedSystemInfo;
            public UInt16 ASIC_ProfilingInfo;
            public UInt16 VoltageObjectInfo;
            public UInt16 PowerSourceInfo;
            public UInt16 ServiceInfo;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct ATOM_POWERPLAY_TABLE
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            public Byte ucTableRevision;
            public UInt16 usTableSize;
            public UInt32 ulGoldenPPID;
            public UInt32 ulGoldenRevision;
            public UInt16 usFormatID;
            public UInt16 usVoltageTime;
            public UInt32 ulPlatformCaps;
            public UInt32 ulMaxODEngineClock;
            public UInt32 ulMaxODMemoryClock;
            public UInt16 usPowerControlLimit;
            public UInt16 usUlvVoltageOffset;
            public UInt16 usStateArrayOffset;
            public UInt16 usFanTableOffset;
            public UInt16 usThermalControllerOffset;
            public UInt16 usReserv;
            public UInt16 usMclkDependencyTableOffset;
            public UInt16 usSclkDependencyTableOffset;
            public UInt16 usVddcLookupTableOffset;
            public UInt16 usVddgfxLookupTableOffset;
            public UInt16 usMMDependencyTableOffset;
            public UInt16 usVCEStateTableOffset;
            public UInt16 usPPMTableOffset;
            public UInt16 usPowerTuneTableOffset;
            public UInt16 usHardLimitTableOffset;
            public UInt16 usPCIETableOffset;
            public UInt16 usGPIOTableOffset;
            public fixed UInt16 usReserved[6];
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_MCLK_ENTRY
        {
            public Byte ucVddcInd;
            public UInt16 usVddci;
            public UInt16 usVddgfxOffset;
            public UInt16 usMvdd;
            public UInt32 ulMclk;
            public UInt16 usReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_MCLK_TABLE
        {
            public Byte ucRevId;
            public Byte ucNumEntries;
            // public ATOM_MCLK_ENTRY entries[ucNumEntries];
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_SCLK_ENTRY
        {
            public Byte ucVddInd;
            public UInt16 usVddcOffset;
            public UInt32 ulSclk;
            public UInt16 usEdcCurrent;
            public Byte ucReliabilityTemperature;
            public Byte ucCKSVOffsetandDisable;
            public UInt32 ulSclkOffset;
            // Polaris Only, remove for compatibility with Fiji
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_SCLK_TABLE
        {
            public Byte ucRevId;
            public Byte ucNumEntries;
            // public ATOM_SCLK_ENTRY entries[ucNumEntries];
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VOLTAGE_ENTRY
        {
            public UInt16 usVdd;
            public UInt16 usCACLow;
            public UInt16 usCACMid;
            public UInt16 usCACHigh;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VOLTAGE_TABLE
        {
            public Byte ucRevId;
            public Byte ucNumEntries;
            // public ATOM_VOLTAGE_ENTRY entries[ucNumEntries];
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_FAN_TABLE
        {
            public Byte ucRevId;
            public Byte ucTHyst;
            public UInt16 usTMin;
            public UInt16 usTMed;
            public UInt16 usTHigh;
            public UInt16 usPWMMin;
            public UInt16 usPWMMed;
            public UInt16 usPWMHigh;
            public UInt16 usTMax;
            public Byte ucFanControlMode;
            public UInt16 usFanPWMMax;
            public UInt16 usFanOutputSensitivity;
            public UInt16 usFanRPMMax;
            public UInt32 ulMinFanSCLKAcousticLimit;
            public Byte ucTargetTemperature;
            public Byte ucMinimumPWMLimit;
            public UInt16 usFanGainEdge;
            public UInt16 usFanGainHotspot;
            public UInt16 usFanGainLiquid;
            public UInt16 usFanGainVrVddc;
            public UInt16 usFanGainVrMvdd;
            public UInt16 usFanGainPlx;
            public UInt16 usFanGainHbm;
            public UInt16 usReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_POWERTUNE_TABLE
        {
            public Byte ucRevId;
            public UInt16 usTDP;
            public UInt16 usConfigurableTDP;
            public UInt16 usTDC;
            public UInt16 usBatteryPowerLimit;
            public UInt16 usSmallPowerLimit;
            public UInt16 usLowCACLeakage;
            public UInt16 usHighCACLeakage;
            public UInt16 usMaximumPowerDeliveryLimit;
            public UInt16 usTjMax;
            public UInt16 usPowerTuneDataSetID;
            public UInt16 usEDCLimit;
            public UInt16 usSoftwareShutdownTemp;
            public UInt16 usClockStretchAmount;
            public UInt16 usTemperatureLimitHotspot;
            public UInt16 usTemperatureLimitLiquid1;
            public UInt16 usTemperatureLimitLiquid2;
            public UInt16 usTemperatureLimitVrVddc;
            public UInt16 usTemperatureLimitVrMvdd;
            public UInt16 usTemperatureLimitPlx;
            public Byte ucLiquid1_I2C_address;
            public Byte ucLiquid2_I2C_address;
            public Byte ucLiquid_I2C_Line;
            public Byte ucVr_I2C_address;
            public Byte ucVr_I2C_Line;
            public Byte ucPlx_I2C_address;
            public Byte ucPlx_I2C_Line;
            public UInt16 usReserved;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VRAM_TIMING_ENTRY
        {
            public UInt32 ulClkRange;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
            public Byte[] ucLatency;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VRAM_ENTRY
        {
            public UInt32 ulChannelMapCfg;
            public UInt16 usModuleSize;
            public UInt16 usMcRamCfg;
            public UInt16 usEnableChannels;
            public Byte ucExtMemoryID;
            public Byte ucMemoryType;
            public Byte ucChannelNum;
            public Byte ucChannelWidth;
            public Byte ucDensity;
            public Byte ucBankCol;
            public Byte ucMisc;
            public Byte ucVREFI;
            public UInt16 usReserved;
            public UInt16 usMemorySize;
            public Byte ucMcTunningSetId;
            public Byte ucRowNum;
            public UInt16 usEMRS2Value;
            public UInt16 usEMRS3Value;
            public Byte ucMemoryVenderID;
            public Byte ucRefreshRateFactor;
            public Byte ucFIFODepth;
            public Byte ucCDR_Bandwidth;
            public UInt32 ulChannelMapCfg1;
            public UInt32 ulBankMapCfg;
            public UInt32 ulReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public Byte[] strMemPNString;
        };

        internal void LoadFile(Stream fileStream, TextBox txtChecksum)
        {
            if((fileStream.Length != 524288) && (fileStream.Length != 524288 / 2))
                MessageBox.Show(PolarisBiosEditorResource.WRONG_BIOS_SIZE, PolarisBiosEditorResource.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            using(var br = new BinaryReader(fileStream))
            {
                Buffer = br.ReadBytes((int)fileStream.Length);

                atom_rom_header_offset = GetValueAtPosition(16, atom_rom_header_ptr);
                atom_rom_header = FromBytes<ATOM_ROM_HEADER>(Buffer.Skip(atom_rom_header_offset).ToArray());
                deviceID = atom_rom_header.usDeviceID.ToString("X");
                fixChecksum(false, txtChecksum);

                var firmwareSignature = new string(atom_rom_header.uaFirmWareSignature);
                if(!firmwareSignature.Equals("ATOM"))
                    MessageBox.Show("WARNING! BIOS Signature is not valid. Only continue if you are 100% sure what you are doing!");

                var oMsgSupported = DialogResult.Yes;
                if(!supportedDeviceID.Contains(deviceID))
                    oMsgSupported = MessageBox.Show(String.Format(PolarisBiosEditorResource.UNSUPPORTED_DEVICE_ID, deviceID),
                        PolarisBiosEditorResource.WARNING, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if(oMsgSupported == DialogResult.Yes)
                {
                    var sb = new StringBuilder();

                    var ptr = atom_rom_header.usBIOS_BootupMessageOffset + 2;
                    while(ptr != -1)
                    {
                        var c = (char)Buffer[ptr];
                        if(c == '\0')
                        {
                            ptr = -1;
                        }
                        else if(c == '\n' || c == '\r')
                        {
                            ptr++;
                        }
                        else
                        {
                            sb.Append(c);
                            ptr++;
                        }
                    }

                    BIOS_BootupMessage = sb.ToString();

                    atom_data_table = FromBytes<ATOM_DATA_TABLES>(Buffer.Skip(atom_rom_header.usMasterDataTableOffset).ToArray());
                    atom_powerplay_offset = atom_data_table.PowerPlayInfo;
                    atom_powerplay_table = FromBytes<ATOM_POWERPLAY_TABLE>(Buffer.Skip(atom_powerplay_offset).ToArray());

                    atom_powertune_offset = atom_data_table.PowerPlayInfo + atom_powerplay_table.usPowerTuneTableOffset;
                    atom_powertune_table = FromBytes<ATOM_POWERTUNE_TABLE>(Buffer.Skip(atom_powertune_offset).ToArray());

                    atom_fan_offset = atom_data_table.PowerPlayInfo + atom_powerplay_table.usFanTableOffset;
                    atom_fan_table = FromBytes<ATOM_FAN_TABLE>(Buffer.Skip(atom_fan_offset).ToArray());

                    atom_mclk_table_offset = atom_data_table.PowerPlayInfo + atom_powerplay_table.usMclkDependencyTableOffset;
                    atom_mclk_table = FromBytes<ATOM_MCLK_TABLE>(Buffer.Skip(atom_mclk_table_offset).ToArray());
                    atom_mclk_entries = new ATOM_MCLK_ENTRY[atom_mclk_table.ucNumEntries];

                    for(var i = 0; i < atom_mclk_entries.Length; i++)
                        atom_mclk_entries[i] = FromBytes<ATOM_MCLK_ENTRY>(Buffer.Skip(atom_mclk_table_offset + Marshal.SizeOf(typeof(ATOM_MCLK_TABLE)) + Marshal.SizeOf(typeof(ATOM_MCLK_ENTRY)) * i).ToArray());

                    atom_sclk_table_offset = atom_data_table.PowerPlayInfo + atom_powerplay_table.usSclkDependencyTableOffset;
                    atom_sclk_table = FromBytes<ATOM_SCLK_TABLE>(Buffer.Skip(atom_sclk_table_offset).ToArray());
                    atom_sclk_entries = new ATOM_SCLK_ENTRY[atom_sclk_table.ucNumEntries];

                    for(var i = 0; i < atom_sclk_entries.Length; i++)
                        atom_sclk_entries[i] = FromBytes<ATOM_SCLK_ENTRY>(Buffer.Skip(atom_sclk_table_offset + Marshal.SizeOf(typeof(ATOM_SCLK_TABLE)) + Marshal.SizeOf(typeof(ATOM_SCLK_ENTRY)) * i).ToArray());

                    atom_vddc_table_offset = atom_data_table.PowerPlayInfo + atom_powerplay_table.usVddcLookupTableOffset;
                    atom_vddc_table = FromBytes<ATOM_VOLTAGE_TABLE>(Buffer.Skip(atom_vddc_table_offset).ToArray());
                    atom_vddc_entries = new ATOM_VOLTAGE_ENTRY[atom_vddc_table.ucNumEntries];

                    for(var i = 0; i < atom_vddc_table.ucNumEntries; i++)
                        atom_vddc_entries[i] = FromBytes<ATOM_VOLTAGE_ENTRY>(Buffer.Skip(atom_vddc_table_offset + Marshal.SizeOf(typeof(ATOM_VOLTAGE_TABLE)) + Marshal.SizeOf(typeof(ATOM_VOLTAGE_ENTRY)) * i).ToArray());

                    atom_vram_info_offset = atom_data_table.VRAM_Info;
                    atom_vram_info = FromBytes<ATOM_VRAM_INFO>(Buffer.Skip(atom_vram_info_offset).ToArray());
                    atom_vram_entries = new ATOM_VRAM_ENTRY[atom_vram_info.ucNumOfVRAMModule];
                    var atom_vram_entry_offset = atom_vram_info_offset + Marshal.SizeOf(typeof(ATOM_VRAM_INFO));
                    for(var i = 0; i < atom_vram_info.ucNumOfVRAMModule; i++)
                    {
                        atom_vram_entries[i] = FromBytes<ATOM_VRAM_ENTRY>(Buffer.Skip(atom_vram_entry_offset).ToArray());
                        atom_vram_entry_offset += atom_vram_entries[i].usModuleSize;
                    }

                    atom_vram_timing_offset = atom_vram_info_offset + atom_vram_info.usMemClkPatchTblOffset + 0x2E;
                    Atom_vram_timing_entries = new ATOM_VRAM_TIMING_ENTRY[MAX_VRAM_ENTRIES];
                    for(var i = 0; i < MAX_VRAM_ENTRIES; i++)
                    {
                        Atom_vram_timing_entries[i] = FromBytes<ATOM_VRAM_TIMING_ENTRY>(Buffer.Skip(atom_vram_timing_offset + Marshal.SizeOf(typeof(ATOM_VRAM_TIMING_ENTRY)) * i).ToArray());

                        // atom_vram_timing_entries have an undetermined length
                        // attempt to determine the last entry in the array
                        if(Atom_vram_timing_entries[i].ulClkRange == 0)
                        {
                            ResizeVRAMTiming(i);
                            break;
                        }
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_VRAM_INFO
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            public UInt16 usMemAdjustTblOffset;
            public UInt16 usMemClkPatchTblOffset;
            public UInt16 usMcAdjustPerTileTblOffset;
            public UInt16 usMcPhyInitTableOffset;
            public UInt16 usDramDataRemapTblOffset;
            public UInt16 usReserved1;
            public Byte ucNumOfVRAMModule;
            public Byte ucMemoryClkPatchTblVer;
            public Byte ucVramModuleVer;
            public Byte ucMcPhyTileNum;
            // public ATOM_VRAM_ENTRY aVramInfo[ucNumOfVRAMModule];
        }

        public PolarisBiosEditorData()
        {
            rc.Add("MT51J256M3", "MICRON");
            rc.Add("EDW4032BAB", "ELPIDA");
            rc.Add("H5GC4H24AJ", "HYNIX_1");
            rc.Add("H5GQ8H24MJ", "HYNIX_2");
            rc.Add("H5GC8H24MJ", "HYNIX_2");
            rc.Add("K4G80325FB", "SAMSUNG");
            rc.Add("K4G41325FE", "SAMSUNG");
            rc.Add("K4G41325FC", "SAMSUNG");
            rc.Add("K4G41325FS", "SAMSUNG");
        }

        public void SetBytesAtPosition(byte[] dest, int ptr, byte[] src)
        {
            for (var i = 0; i < src.Length; i++)
            {
                dest[ptr + i] = src[i];
            }
        }

        public int GetValueAtPosition(int bits, int position, bool isFrequency = false)
        {
            int value = 0;
            if (position <= Buffer.Length - 4)
            {
                switch (bits)
                {
                    case 8:
                    default:
                        value = Buffer[position];
                        break;
                    case 16:
                        value = (Buffer[position + 1] << 8) | Buffer[position];
                        break;
                    case 24:
                        value = (Buffer[position + 2] << 16) | (Buffer[position + 1] << 8) | Buffer[position];
                        break;
                    case 32:
                        value = (Buffer[position + 3] << 24) | (Buffer[position + 2] << 16) | (Buffer[position + 1] << 8) | Buffer[position];
                        break;
                }
                if (isFrequency)
                    return value / 100;
                return value;
            }
            return -1;
        }

        public bool SetValueAtPosition(int value, int bits, int position, bool isFrequency = false)
        {
            if (isFrequency)
                value *= 100;
            if (position <= Buffer.Length - 4)
            {
                switch (bits)
                {
                    case 8:
                    default:
                        Buffer[position] = (byte)value;
                        break;
                    case 16:
                        Buffer[position] = (byte)value;
                        Buffer[position + 1] = (byte)(value >> 8);
                        break;
                    case 24:
                        Buffer[position] = (byte)value;
                        Buffer[position + 1] = (byte)(value >> 8);
                        Buffer[position + 2] = (byte)(value >> 16);
                        break;
                    case 32:
                        Buffer[position] = (byte)value;
                        Buffer[position + 1] = (byte)(value >> 8);
                        Buffer[position + 2] = (byte)(value >> 16);
                        Buffer[position + 3] = (byte)(value >> 32);
                        break;
                }
                return true;
            }
            return false;
        }

        public bool setValueAtPosition(String text, int bits, int position, bool isFrequency = false)
        {
            int value = 0;
            if (!int.TryParse(text, out value))
            {
                return false;
            }
            return SetValueAtPosition(value, bits, position, isFrequency);
        }

        public void fixChecksum(bool save, TextBox txtChecksum)
        {
            Byte checksum = Buffer[atom_rom_checksum_offset];
            int size = Buffer[0x02] * 512;
            Byte offset = 0;

            for (int i = 0; i < size; i++)
            {
                offset += Buffer[i];
            }
            if (checksum == (Buffer[atom_rom_checksum_offset] - offset))
            {
                txtChecksum.ForeColor = Color.Green;
            }
            else if (!save)
            {
                txtChecksum.ForeColor = Color.Red;
                MessageBox.Show("Invalid checksum - Save to fix!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (save)
            {
                Buffer[atom_rom_checksum_offset] -= offset;
                txtChecksum.ForeColor = Color.Green;
            }
            txtChecksum.Text = "0x" + Buffer[atom_rom_checksum_offset].ToString("X");
        }

        public void updateVRAM_entries(ListView tableVRAM)
        {
            for (var i = 0; i < tableVRAM.Items.Count; i++)
            {
                var container = tableVRAM.Items[i];
                var name = container.Text;
                var value = container.SubItems[1].Text;
                var num = (int)int32.ConvertFromString(value);

                if (name == "VendorID")
                {
                    atom_vram_entries[atom_vram_index].ucMemoryVenderID = (Byte)num;
                }
                else if (name == "Size (MB)")
                {
                    atom_vram_entries[atom_vram_index].usMemorySize = (UInt16)num;
                }
                else if (name == "Density")
                {
                    atom_vram_entries[atom_vram_index].ucDensity = (Byte)num;
                }
                else if (name == "Type")
                {
                    atom_vram_entries[atom_vram_index].ucMemoryType = (Byte)num;
                }
            }
        }

        internal void ResizeVRAMTiming(int i)
        {
            Array.Resize(ref atom_vram_timing_entries, i);
        }
    }
}
