public Mem MemLib = new Mem();

[DllImport("KERNEL32.DLL")]
public static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);

[DllImport("KERNEL32.DLL")]
public static extern int Process32First(IntPtr handle, ref ProcessEntry32 pe);

[DllImport("KERNEL32.DLL")]
public static extern int Process32Next(IntPtr handle, ref ProcessEntry32 pe);

public string GetProcID(int index)
        {
            if (index != 1 && index != 0) return string.Empty;
            uint maxThreads = 0;
            IntPtr bestProcess = IntPtr.Zero;

            IntPtr snapshot = CreateToolhelp32Snapshot(2U, 0U);
            if ((int)snapshot <= 0) return string.Empty;

            try
            {
                ProcessEntry32 entry = new ProcessEntry32 { dwSize = (uint)Marshal.SizeOf<ProcessEntry32>() };
                for (int processFound = Process32First(snapshot, ref entry); processFound == 1; processFound = Process32Next(snapshot, ref entry))
                {
                    if ((entry.szExeFile.Contains("HD-Player") || entry.szExeFile.Contains("AndroidProcess") ||
                         entry.szExeFile.Contains("LdVBoxHeadless") || entry.szExeFile.Contains("MEmuHeadless") ||
                         entry.szExeFile.Contains("NoxVMHandle") || entry.szExeFile.Contains("aow_exe")) &&
                        entry.cntThreads > maxThreads)
                    {
                        maxThreads = entry.cntThreads;
                        bestProcess = (IntPtr)entry.th32ProcessID;
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(snapshot);
            }
            string processId = bestProcess.ToString();
            this.PID.Text = processId; 
            return processId;
        }

 public async void Rep(string original, string replace)
        {
            try
            {
                this.MemLib.OpenProcess(Convert.ToInt32(this.PID.Text));
                IEnumerable<long> addresses = await this.MemLib.AoBScan(original, true, true, string.Empty);

                if (!addresses.Any()) return;

                foreach (long address in addresses)
                {
                    this.MemLib.WriteMemory(address.ToString("X"), "bytes", replace, string.Empty, null, true);
                    showtoast("by nash", "Function applied successfully!");
                    PlayBeep();
                }
            }
            catch (Exception ex)
            {
                showtoast("by nash", "Error writing memory in process");
                Console.WriteLine("Erro ao escrever memória: " + ex.Message);
            }
        }

 public struct ProcessEntry32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }
        public void PlayBeep()
        {
            using (SoundPlayer player = new SoundPlayer(Properties.Resources.beep))
            {
                player.Play();
            }
        }
