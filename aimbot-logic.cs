 string nash = "FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A5 43 00 00 00 00 00 00 00 00 ??";

        private Dictionary<long, int> originalValues = new Dictionary<long, int>();
        private bool aimbotActivated = false;

        private async void CheckBox1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Buscando o processo 'HD-Player'...");
            int proc = Process.GetProcessesByName("HD-Player")[0].Id;
            memory.OpenProcess(proc);
            Console.WriteLine($"Processo encontrado! ID: {proc}");

            Console.WriteLine("Iniciando a pesquisa por AoB...");
            var result = await memory.AoBScan(nash, true, true);

            if (result.Any())
            {
                Console.WriteLine("Resultados encontrados!");

                // Ativação do aimbot
                if (animatedColorGunaCheckBox1.Checked && !aimbotActivated)
                {
                    foreach (var CurrentAddress in result)
                    {
                        long Enderecoleitura = CurrentAddress + 160L;
                        long EnderecoEscrita = CurrentAddress + 0x6C;

                        int originalReadValue = memory.ReadMemory<int>(Enderecoleitura.ToString("X"));
                        int originalWriteValue = memory.ReadMemory<int>(EnderecoEscrita.ToString("X"));

                        Console.WriteLine($"Endereço leitura: {Enderecoleitura}, Endereço escrita: {EnderecoEscrita}");
                        Console.WriteLine($"Valor original leitura: {originalReadValue}, Valor original escrita: {originalWriteValue}");

                        if (!originalValues.ContainsKey(Enderecoleitura))
                        {
                            originalValues[Enderecoleitura] = originalReadValue;
                            Console.WriteLine($"Valor de leitura armazenado no dicionário para o endereço: {Enderecoleitura}");
                        }

                        if (!originalValues.ContainsKey(EnderecoEscrita))
                        {
                            originalValues[EnderecoEscrita] = originalWriteValue;
                            Console.WriteLine($"Valor de escrita armazenado no dicionário para o endereço: {EnderecoEscrita}");
                        }

                        memory.WriteMemory(EnderecoEscrita.ToString("X"), "int", originalReadValue.ToString());
                        Console.WriteLine($"Valor de escrita alterado para: {originalReadValue}");
                    }
                    aimbotActivated = true;
                }

                // Desativação do aimbot
                else if (!animatedColorGunaCheckBox1.Checked && aimbotActivated)
                {
                    Console.WriteLine("Desativando aimbot...");
                    foreach (var kvp in originalValues)
                    {
                        Console.WriteLine($"Restaurando valor para o endereço {kvp.Key}: {kvp.Value}");
                        memory.WriteMemory(kvp.Key.ToString("X"), "int", kvp.Value.ToString());
                    }
                    aimbotActivated = false; 
                    originalValues.Clear(); 
                    Console.WriteLine("Aimbot desativado e valores restaurados!");
                }
            }
            else
            {
                Console.WriteLine("Nenhum resultado encontrado no AoB scan.");
            }
        }
