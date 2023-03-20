using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

telemetry_state_t telemetry = new telemetry_state_t();
while (true)
{
    try
    {
        using (MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("ARGB_Telemetry_Data"))
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor())
            {
                reader.Read(0, out telemetry);
            }
        }
    }
    catch { continue; }
    Console.WriteLine($"Скорость(м/с): {telemetry.speed};\nЛевый поворотник: {telemetry.l_blinker};\n" +
        $"Правый поворотник: {telemetry.r_blinker};\nАварийка: {telemetry.hazard};\n" +
        $"Ручник: {telemetry.parking_brake};\nНехватка воздуха: {telemetry.low_air};\n" +
        $"Нехватка топлива: {telemetry.low_fuel}");
    System.Threading.Thread.Sleep(50);
    Console.Clear();
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
struct telemetry_state_t
{
    internal float speed;        // Скорость грузовика
    internal bool parking_brake; // Ручник
    internal bool low_air;       // Нехватка воздуха
    internal bool low_fuel;      // Нехватка топлива
    internal bool l_blinker;     // Левый поворотник
    internal bool r_blinker;     // Правый поворотник
    internal bool hazard;        // Аварийка
};
