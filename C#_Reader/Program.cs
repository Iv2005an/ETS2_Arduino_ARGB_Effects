using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;
using System.Drawing;

namespace ReadMemoryAp
{
    class Program
    {
        struct telemetry_state_t
        {
            float speed;        // Скорость грузовика
            bool parking_brake; // Ручник
            bool low_air;       // Нехватка воздуха
            bool low_fuel;      // Нехватка топлива
            bool l_blinker;     // Левый поворотник
            bool r_blinker;     // Правый поворотник
            bool hazard;        // Аварийка
        };
        static void Main(string[] args)
        {
            telemetry_state_t telemetry = new telemetry_state_t();
            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(telemetry_state_t));
            while (true)
            {
                try
                {
                    MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("ARGB_Telemetry_Data");

                    using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor())
                    {
                        reader.Read(size, out telemetry);
                    }
                }
                catch { continue; }
            }
        }
    }
}