using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static PolarisBiosEditor.PolarisBiosEditorData;
using static PolarisBiosEditor.PolarisBiosEditorHelper;
using System.Runtime.InteropServices;

namespace PolarisBiosEditor
{
    public partial class PolarisBiosEditorUserControl : UserControl
    {
        private PolarisBiosEditorData oData = new PolarisBiosEditorData();

        public PolarisBiosEditorUserControl()
        {
            InitializeComponent();

            foreach(Control oControl in Controls)
                if(oControl.GetType() == typeof(ListView)) oControl.MouseClick += ListView_ChangeSelection;

            save.Enabled = false;
            boxROM.Enabled = false;
            boxPOWERPLAY.Enabled = false;
            boxPOWERTUNE.Enabled = false;
            boxFAN.Enabled = false;
            boxGPU.Enabled = false;
            boxMEM.Enabled = false;
            boxVRAM.Enabled = false;
        }

        private void SaveFileDialog_Click(object oSender, EventArgs oArgs)
        {
            var SaveFileDialog = new SaveFileDialog();
            SaveFileDialog.Title = "Save As";
            SaveFileDialog.Filter = "BIOS (*.rom)|*.rom";

            if(SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(SaveFileDialog.FileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);

                for(var i = 0; i < tableROM.Items.Count; i++)
                {
                    ListViewItem container = tableROM.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;

                    if(name == "VendorID")
                    {
                        var num = (int)oData.int32.ConvertFromString(value);
                        oData.atom_rom_header.usVendorID = (UInt16)num;
                    }
                    else if(name == "DeviceID")
                    {
                        var num = (int)oData.int32.ConvertFromString(value);
                        oData.atom_rom_header.usDeviceID = (UInt16)num;
                    }
                    else if(name == "Sub ID")
                    {
                        var num = (int)oData.int32.ConvertFromString(value);
                        oData.atom_rom_header.usSubsystemID = (UInt16)num;
                    }
                    else if(name == "Sub VendorID")
                    {
                        var num = (int)oData.int32.ConvertFromString(value);
                        oData.atom_rom_header.usSubsystemVendorID = (UInt16)num;
                    }
                    else if(name == "Firmware Signature")
                    {
                        oData.atom_rom_header.uaFirmWareSignature = value.ToCharArray();
                    }
                }

                for(var i = 0; i < tablePOWERPLAY.Items.Count; i++)
                {
                    ListViewItem container = tablePOWERPLAY.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var num = (int)oData.int32.ConvertFromString(value);

                    if(name == "Max GPU Freq. (MHz)")
                    {
                        oData.atom_powerplay_table.ulMaxODEngineClock = (UInt32)(num * 100);
                    }
                    else if(name == "Max Memory Freq. (MHz)")
                    {
                        oData.atom_powerplay_table.ulMaxODMemoryClock = (UInt32)(num * 100);
                    }
                    else if(name == "Power Control Limit (%)")
                    {
                        oData.atom_powerplay_table.usPowerControlLimit = (UInt16)num;
                    }
                }

                for(var i = 0; i < tablePOWERTUNE.Items.Count; i++)
                {
                    ListViewItem container = tablePOWERTUNE.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var num = (int)oData.int32.ConvertFromString(value);

                    if(name == "TDP (W)")
                    {
                        oData.atom_powertune_table.usTDP = (UInt16)num;
                    }
                    else if(name == "TDC (A)")
                    {
                        oData.atom_powertune_table.usTDC = (UInt16)num;
                    }
                    else if(name == "Max Power Limit (W)")
                    {
                        oData.atom_powertune_table.usMaximumPowerDeliveryLimit = (UInt16)num;
                    }
                    else if(name == "Max Temp. (C)")
                    {
                        oData.atom_powertune_table.usTjMax = (UInt16)num;
                    }
                    else if(name == "Shutdown Temp. (C)")
                    {
                        oData.atom_powertune_table.usSoftwareShutdownTemp = (UInt16)num;
                    }
                    else if(name == "Hotspot Temp. (C)")
                    {
                        oData.atom_powertune_table.usTemperatureLimitHotspot = (UInt16)num;
                    }
                }

                for(var i = 0; i < tableFAN.Items.Count; i++)
                {
                    ListViewItem container = tableFAN.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var num = (int)oData.int32.ConvertFromString(value);

                    if(name == "Temp. Hysteresis")
                    {
                        oData.atom_fan_table.ucTHyst = (Byte)num;
                    }
                    else if(name == "Min Temp. (C)")
                    {
                        oData.atom_fan_table.usTMin = (UInt16)(num * 100);
                    }
                    else if(name == "Med Temp. (C)")
                    {
                        oData.atom_fan_table.usTMed = (UInt16)(num * 100);
                    }
                    else if(name == "High Temp. (C)")
                    {
                        oData.atom_fan_table.usTHigh = (UInt16)(num * 100);
                    }
                    else if(name == "Max Temp. (C)")
                    {
                        oData.atom_fan_table.usTMax = (UInt16)(num * 100);
                    }
                    else if(name == "Target Temp. (C)")
                    {
                        oData.atom_fan_table.ucTargetTemperature = (Byte)num;
                    }
                    else if(name == "Legacy or Fuzzy Fan Mode")
                    {
                        oData.atom_fan_table.ucFanControlMode = (Byte)(num);
                    }
                    else if(name == "Min PWM (%)")
                    {
                        oData.atom_fan_table.usPWMMin = (UInt16)(num * 100);
                    }
                    else if(name == "Med PWM (%)")
                    {
                        oData.atom_fan_table.usPWMMed = (UInt16)(num * 100);
                    }
                    else if(name == "High PWM (%)")
                    {
                        oData.atom_fan_table.usPWMHigh = (UInt16)(num * 100);
                    }
                    else if(name == "Max PWM (%)")
                    {
                        oData.atom_fan_table.usFanPWMMax = (UInt16)num;
                    }
                    else if(name == "Max RPM")
                    {
                        oData.atom_fan_table.usFanRPMMax = (UInt16)num;
                    }
                    else if(name == "Sensitivity")
                    {
                        oData.atom_fan_table.usFanOutputSensitivity = (UInt16)num;
                    }
                    else if(name == "Acoustic Limit (MHz)")
                    {
                        oData.atom_fan_table.ulMinFanSCLKAcousticLimit = (UInt32)(num * 100);
                    }
                }

                for(var i = 0; i < tableGPU.Items.Count; i++)
                {
                    ListViewItem container = tableGPU.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var mhz = (int)oData.int32.ConvertFromString(name) * 100;
                    var mv = (int)oData.int32.ConvertFromString(value);

                    oData.atom_sclk_entries[i].ulSclk = (UInt32)mhz;
                    oData.atom_vddc_entries[oData.atom_sclk_entries[i].ucVddInd].usVdd = (UInt16)mv;

                    if(mv < 0xFF00)
                        oData.atom_sclk_entries[i].usVddcOffset = 0;
                }

                for(var i = 0; i < tableMEMORY.Items.Count; i++)
                {
                    ListViewItem container = tableMEMORY.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var mhz = (int)oData.int32.ConvertFromString(name) * 100;
                    var mv = (int)oData.int32.ConvertFromString(value);

                    oData.atom_mclk_entries[i].ulMclk = (UInt32)mhz;
                    oData.atom_mclk_entries[i].usMvdd = (UInt16)mv;
                }

                oData.updateVRAM_entries(tableVRAM);
                for(var i = 0; i < tableVRAM_TIMING.Items.Count; i++)
                {
                    ListViewItem container = tableVRAM_TIMING.Items[i];
                    var name = container.Text;
                    var value = container.SubItems[1].Text;
                    var arr = StringToByteArray(value);
                    UInt32 mhz;
                    if(name.IndexOf(':') > 0)
                    {
                        mhz = (UInt32)oData.uint32.ConvertFromString(name.Substring(name.IndexOf(':') + 1)) * 100;
                        mhz += (UInt32)oData.uint32.ConvertFromString(name.Substring(0, name.IndexOf(':'))) << 24; // table id
                    }
                    else
                    {
                        mhz = (UInt32)oData.uint32.ConvertFromString(name) * 100;
                    }
                    oData.Atom_vram_timing_entries[i].ulClkRange = mhz;
                    oData.Atom_vram_timing_entries[i].ucLatency = arr;
                }

                oData.SetBytesAtPosition(oData.Buffer, oData.atom_rom_header_offset, GetBytes(oData.atom_rom_header));
                oData.SetBytesAtPosition(oData.Buffer, oData.atom_powerplay_offset, GetBytes(oData.atom_powerplay_table));
                oData.SetBytesAtPosition(oData.Buffer, oData.atom_powertune_offset, GetBytes(oData.atom_powertune_table));
                oData.SetBytesAtPosition(oData.Buffer, oData.atom_fan_offset, GetBytes(oData.atom_fan_table));

                for(var i = 0; i < oData.atom_mclk_table.ucNumEntries; i++)
                {
                    oData.SetBytesAtPosition(oData.Buffer, oData.atom_mclk_table_offset + Marshal.SizeOf(typeof(ATOM_MCLK_TABLE)) + Marshal.SizeOf(typeof(ATOM_MCLK_ENTRY)) * i, GetBytes(oData.atom_mclk_entries[i]));
                }

                for(var i = 0; i < oData.atom_sclk_table.ucNumEntries; i++)
                {
                    oData.SetBytesAtPosition(oData.Buffer, oData.atom_sclk_table_offset + Marshal.SizeOf(typeof(ATOM_SCLK_TABLE)) + Marshal.SizeOf(typeof(ATOM_SCLK_ENTRY)) * i, GetBytes(oData.atom_sclk_entries[i]));
                }

                for(var i = 0; i < oData.atom_vddc_table.ucNumEntries; i++)
                {
                    oData.SetBytesAtPosition(oData.Buffer, oData.atom_vddc_table_offset + Marshal.SizeOf(typeof(ATOM_VOLTAGE_TABLE)) + Marshal.SizeOf(typeof(ATOM_VOLTAGE_ENTRY)) * i, GetBytes(oData.atom_vddc_entries[i]));
                }

                var atom_vram_entry_offset = oData.atom_vram_info_offset + Marshal.SizeOf(typeof(ATOM_VRAM_INFO));
                for(var i = 0; i < oData.atom_vram_info.ucNumOfVRAMModule; i++)
                {
                    oData.SetBytesAtPosition(oData.Buffer, atom_vram_entry_offset, GetBytes(oData.atom_vram_entries[i]));
                    atom_vram_entry_offset += oData.atom_vram_entries[i].usModuleSize;
                }

                oData.atom_vram_timing_offset = oData.atom_vram_info_offset + oData.atom_vram_info.usMemClkPatchTblOffset + 0x2E;
                for(var i = 0; i < oData.Atom_vram_timing_entries.Length; i++)
                {
                    oData.SetBytesAtPosition(oData.Buffer, oData.atom_vram_timing_offset + Marshal.SizeOf(typeof(ATOM_VRAM_TIMING_ENTRY)) * i, GetBytes(oData.Atom_vram_timing_entries[i]));
                }

                oData.BIOS_BootupMessage = txtBIOSBootupMessage.Text.Substring(0, oData.BIOS_BootupMessage.Length);

                oData.SetBytesAtPosition(oData.Buffer, oData.atom_rom_header.usBIOS_BootupMessageOffset + 2, Encoding.ASCII.GetBytes(oData.BIOS_BootupMessage));
                oData.fixChecksum(true, txtChecksum);
                bw.Write(oData.Buffer);

                fs.Close();
                bw.Close();
            }
        }

        private ListViewItem handler;



        private void Apply_Click(object oSender, EventArgs oArgs)
        {
            if(handler != null)
            {
                handler.Text = editSubItem1.Text;
                handler.SubItems[1].Text = editSubItem2.Text;
            }
        }

        private void EditSubItem2_Click(object oSender, EventArgs oArgs)
        {
            MouseEventArgs me = (MouseEventArgs)oArgs;
            if(me.Button == MouseButtons.Right)
            {
                if(editSubItem2.Text.Length == 96)
                {
                    byte[] decode = PolarisBiosEditorHelper.StringToByteArray(editSubItem2.Text);
                    MessageBox.Show("Decode Memory Timings " + decode + " / not implemented yet!");
                }
            }
        }

        private void OpenFileDialog_Click(object oSender, EventArgs oArgs)
        {
            Console.WriteLine("OpenFileDialog");
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BIOS (.rom)|*.rom|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                save.Enabled = false;

                tableROM.Items.Clear();
                tablePOWERPLAY.Items.Clear();
                tablePOWERTUNE.Items.Clear();
                tableFAN.Items.Clear();
                tableGPU.Items.Clear();
                tableMEMORY.Items.Clear();
                tableVRAM.Items.Clear();
                tableVRAM_TIMING.Items.Clear();

                var fileStream = openFileDialog.OpenFile();
                oData.LoadFile(fileStream, txtChecksum);

                txtBIOSBootupMessage.Text = oData.BIOS_BootupMessage;
                txtBIOSBootupMessage.MaxLength = oData.BIOS_BootupMessage.Length;


                tableROM.Items.Add(new ListViewItem(new string[] {
                            "BootupMessageOffset",
                            "0x" + oData.atom_rom_header.usBIOS_BootupMessageOffset.ToString ("X")
                        }
                        ));
                tableROM.Items.Add(new ListViewItem(new string[] {
                            "VendorID",
                            "0x" + oData.atom_rom_header.usVendorID.ToString ("X")
                        }
                ));
                tableROM.Items.Add(new ListViewItem(new string[] {
                            "DeviceID",
                            "0x" + oData.atom_rom_header.usDeviceID.ToString ("X")
                        }
                ));
                tableROM.Items.Add(new ListViewItem(new string[] {
                            "Sub ID",
                            "0x" + oData.atom_rom_header.usSubsystemID.ToString ("X")
                        }
                ));
                tableROM.Items.Add(new ListViewItem(new string[] {
                            "Sub VendorID",
                            "0x" + oData.atom_rom_header.usSubsystemVendorID.ToString ("X")
                        }
                ));
                tableROM.Items.Add(new ListViewItem(new string[] {
                            "Firmware Signature",
                            //"0x" + oData.atom_rom_header.uaFirmWareSignature.ToString ("X")
                            new string(oData.atom_rom_header.uaFirmWareSignature)
                        }
                ));

                tablePOWERPLAY.Items.Clear();
                tablePOWERPLAY.Items.Add(new ListViewItem(new string[] {
                            "Max GPU Freq. (MHz)",
                            Convert.ToString (oData.atom_powerplay_table.ulMaxODEngineClock / 100)
                        }
                ));
                tablePOWERPLAY.Items.Add(new ListViewItem(new string[] {
                            "Max Memory Freq. (MHz)",
                            Convert.ToString (oData.atom_powerplay_table.ulMaxODMemoryClock / 100)
                        }
                ));
                tablePOWERPLAY.Items.Add(new ListViewItem(new string[] {
                            "Power Control Limit (%)",
                            Convert.ToString (oData.atom_powerplay_table.usPowerControlLimit)
                        }
                ));

                tablePOWERTUNE.Items.Clear();
                tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "TDP (W)",
                            Convert.ToString (oData.atom_powertune_table.usTDP)
                        }
                ));
                tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "TDC (A)",
                            Convert.ToString (oData.atom_powertune_table.usTDC)
                        }
                ));
                tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "Max Power Limit (W)",
                            Convert.ToString (oData.atom_powertune_table.usMaximumPowerDeliveryLimit)
                        }
                ));
                tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "Max Temp. (C)",
                            Convert.ToString (oData.atom_powertune_table.usTjMax)
                        }
                ));
                tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "Shutdown Temp. (C)",
                            Convert.ToString (oData.atom_powertune_table.usSoftwareShutdownTemp)
                        }
                ));
                tablePOWERTUNE.Items.Add(new ListViewItem(new string[] {
                            "Hotspot Temp. (C)",
                            Convert.ToString (oData.atom_powertune_table.usTemperatureLimitHotspot)
                        }
                ));

                tableFAN.Items.Clear();
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Temp. Hysteresis",
                            Convert.ToString (oData.atom_fan_table.ucTHyst)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Min Temp. (C)",
                            Convert.ToString (oData.atom_fan_table.usTMin / 100)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Med Temp. (C)",
                            Convert.ToString (oData.atom_fan_table.usTMed / 100)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "High Temp. (C)",
                            Convert.ToString (oData.atom_fan_table.usTHigh / 100)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Max Temp. (C)",
                            Convert.ToString (oData.atom_fan_table.usTMax / 100)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Target Temp. (C)",
                            Convert.ToString (oData.atom_fan_table.ucTargetTemperature)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Fuzzy Fan Mode",
                            Convert.ToString (oData.atom_fan_table.ucFanControlMode)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Min PWM (%)",
                            Convert.ToString (oData.atom_fan_table.usPWMMin / 100)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Med PWM (%)",
                            Convert.ToString (oData.atom_fan_table.usPWMMed / 100)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "High PWM (%)",
                            Convert.ToString (oData.atom_fan_table.usPWMHigh / 100)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Max PWM (%)",
                            Convert.ToString (oData.atom_fan_table.usFanPWMMax)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Max RPM",
                            Convert.ToString (oData.atom_fan_table.usFanRPMMax)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Sensitivity",
                            Convert.ToString (oData.atom_fan_table.usFanOutputSensitivity)
                        }
                ));
                tableFAN.Items.Add(new ListViewItem(new string[] {
                            "Acoustic Limit (MHz)",
                            Convert.ToString (oData.atom_fan_table.ulMinFanSCLKAcousticLimit / 100)
                        }
                ));

                tableGPU.Items.Clear();
                for(var i = 0; i < oData.atom_sclk_table.ucNumEntries; i++)
                {
                    tableGPU.Items.Add(new ListViewItem(new string[] {
                                Convert.ToString (oData.atom_sclk_entries [i].ulSclk / 100),
                                Convert.ToString (oData.atom_vddc_entries [oData.atom_sclk_entries [i].ucVddInd].usVdd)
                            }
                    ));
                }

                tableMEMORY.Items.Clear();
                for(var i = 0; i < oData.atom_mclk_table.ucNumEntries; i++)
                {
                    tableMEMORY.Items.Add(new ListViewItem(new string[] {
                                Convert.ToString (oData.atom_mclk_entries [i].ulMclk / 100),
                                Convert.ToString (oData.atom_mclk_entries [i].usMvdd)
                            }
                    ));
                }

                listVRAM.Items.Clear();
                for(var i = 0; i < oData.atom_vram_info.ucNumOfVRAMModule; i++)
                {
                    if(oData.atom_vram_entries[i].strMemPNString[0] != 0)
                    {
                        var mem_id = Encoding.UTF8.GetString(oData.atom_vram_entries[i].strMemPNString).Substring(0, 10);
                        string mem_vendor;

                        if(oData.rc.ContainsKey(mem_id))
                            mem_vendor = oData.rc[mem_id];
                        else
                            mem_vendor = "UNKNOWN";

                        listVRAM.Items.Add(mem_id + " (" + mem_vendor + ")");
                    }
                }
                listVRAM.SelectedIndex = 0;
                oData.atom_vram_index = listVRAM.SelectedIndex;

                tableVRAM_TIMING.Items.Clear();
                for(var i = 0; i < oData.Atom_vram_timing_entries.Length; i++)
                {
                    uint tbl = oData.Atom_vram_timing_entries[i].ulClkRange >> 24;
                    tableVRAM_TIMING.Items.Add(new ListViewItem(new string[] 
                            {
                                tbl.ToString () + ":" + (oData.Atom_vram_timing_entries [i].ulClkRange & 0x00FFFFFF) / 100,
                                ByteArrayToString(oData.Atom_vram_timing_entries[i].ucLatency)
                            }
                    ));
                }

                save.Enabled = true;
                boxROM.Enabled = true;
                boxPOWERPLAY.Enabled = true;
                boxPOWERTUNE.Enabled = true;
                boxFAN.Enabled = true;
                boxGPU.Enabled = true;
                boxMEM.Enabled = true;
                boxVRAM.Enabled = true;
            }

            tableROM.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableROM.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tableFAN.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableFAN.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tablePOWERPLAY.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tablePOWERPLAY.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tableGPU.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableGPU.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tablePOWERTUNE.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tablePOWERTUNE.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tableMEMORY.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableMEMORY.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tableVRAM.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableVRAM.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            tableVRAM_TIMING.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            tableVRAM_TIMING.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void listVRAM_SelectionChanged(object oSender, EventArgs oArgs)
        {
            oData.updateVRAM_entries(tableVRAM);
            tableVRAM.Items.Clear();
            if(listVRAM.SelectedIndex >= 0 && listVRAM.SelectedIndex < listVRAM.Items.Count)
            {
                oData.atom_vram_index = listVRAM.SelectedIndex;
                tableVRAM.Items.Add(new ListViewItem(new string[] {
                    "VendorID",
                    "0x" + oData.atom_vram_entries[oData.atom_vram_index].ucMemoryVenderID.ToString ("X")
                }
                ));
                tableVRAM.Items.Add(new ListViewItem(new string[] {
                    "Size (MB)",
                    Convert.ToString (oData.atom_vram_entries[oData.atom_vram_index].usMemorySize)
                }
                ));
                tableVRAM.Items.Add(new ListViewItem(new string[] {
                    "Density",
                    "0x" + oData.atom_vram_entries [oData.atom_vram_index].ucDensity.ToString ("X")
                }
                ));
                tableVRAM.Items.Add(new ListViewItem(new string[] {
                    "Type",
                    "0x" + oData.atom_vram_entries [oData.atom_vram_index].ucMemoryType.ToString ("X")
                }
                ));
            }
        }

        private void apply_timings(int vendor_index, int timing_index)
        {
            for(var i = 0; i < tableVRAM_TIMING.Items.Count; i++)
            {
                ListViewItem container = tableVRAM_TIMING.Items[i];
                var name = container.Text;
                UInt32 real_mhz = 0;
                int mem_index = -1;

                if(name.IndexOf(':') > 0)
                {
                    // get mem index
                    mem_index = (Int32)oData.int32.ConvertFromString(name.Substring(0, 1));
                }
                else
                {
                    mem_index = 32768;
                }

                real_mhz = (UInt32)oData.uint32.ConvertFromString(name.Substring(name.IndexOf(':') + 1));

                if(real_mhz >= 1500 && (mem_index == vendor_index || mem_index == 32768))
                {
                    // set the timings
                    container.SubItems[1].Text = oData.timings[timing_index];
                }
            }
        }

        private void button1_Click(object oSender, EventArgs oArgs)
        {
            int samsung_index = -1;
            int micron_index = -1;
            int elpida_index = -1;
            int hynix_1_index = -1;
            int hynix_2_index = -1;
            for(var i = 0; i < oData.atom_vram_info.ucNumOfVRAMModule; i++)
            {
                string mem_vendor;
                if(oData.atom_vram_entries[i].strMemPNString[0] != 0)
                {
                    var mem_id = Encoding.UTF8.GetString(oData.atom_vram_entries[i].strMemPNString).Substring(0, 10);

                    if(oData.rc.ContainsKey(mem_id))
                    {
                        mem_vendor = oData.rc[mem_id];
                    }
                    else
                    {
                        mem_vendor = "UNKNOWN";
                    }

                    switch(mem_vendor)
                    {
                        case "SAMSUNG":
                            samsung_index = i;
                            break;
                        case "MICRON":
                            micron_index = i;
                            break;
                        case "ELPIDA":
                            elpida_index = i;
                            break;
                        case "HYNIX_1":
                            hynix_1_index = i;
                            break;
                        case "HYNIX_2":
                            hynix_2_index = i;
                            break;
                    }
                }
            }

            if(samsung_index != -1)
            {
                MessageBox.Show("Samsung Memory found at index #" + samsung_index + ", now applying UBERMIX 3.1 timings to 1500+ strap(s)");
                apply_timings(samsung_index, 0);
            }

            if(hynix_2_index != -1)
            {
                MessageBox.Show("Hynix (2) Memory found at index #" + hynix_2_index + ", now applying GOOD HYNIX MINING timings to 1500+ strap(s)");
                apply_timings(hynix_2_index, 4);

            }

            if(micron_index != -1)
            {
                MessageBox.Show("Micron Memory found at index #" + micron_index + ", now applying GOOD MICRON MINING timings to 1500+ strap(s)");

                apply_timings(micron_index, 5);

            }

            if(hynix_1_index != -1)
            {
                MessageBox.Show("Hynix (1) Memory found at index #" + hynix_1_index + ", now applying GOOD HYNIX MINING timings to 1500+ strap(s)");

                apply_timings(hynix_1_index, 6);

            }

            if(elpida_index != -1)
            {
                MessageBox.Show("Elpida Memory found at index #" + elpida_index + ", now applying GOOD ELPIDA MINING timings to 1500+ strap(s)");

                apply_timings(elpida_index, 7);

            }
            if(samsung_index == -1 && hynix_2_index == -1 && hynix_1_index == -1 && elpida_index == -1 && micron_index == -1)
            {
                MessageBox.Show("Sorry, no supported memory found. If you think this is an error, please file a bugreport @ github.com/jaschaknack/PolarisBiosEditor");
            }

        }

        private void ListView_ChangeSelection(object oSender, MouseEventArgs oArgs)
        {
            var lb = oSender as ListView;
            var sel_name = lb.SelectedItems[0].Text;

            foreach(ListViewItem container in lb.Items)
            {
                var name = container.Text;
                var value = container.SubItems[1].Text;

                if(name == sel_name)
                {
                    editSubItem1.Text = name;
                    editSubItem2.Text = value;
                    handler = container;
                }
            }
        }
    }
}