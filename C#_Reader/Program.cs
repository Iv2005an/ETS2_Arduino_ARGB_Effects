using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.IO.Ports;
bool error;
SerialPort port;
do
{
    error = false;
    port = new SerialPort(GetArduinoPort(), 9600);
    port.Encoding = System.Text.Encoding.UTF8;
    try
    {
        port.Open();
    }
    catch (UnauthorizedAccessException)
    {
        error = true;
        Console.WriteLine("\nПорт уже открыт!!!");
    }
} while (error);
telemetry_state_t telemetry = new telemetry_state_t();
while (true)
{
    try
    {
#pragma warning disable CA1416 // Проверка совместимости платформы
        using (MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("ARGB_Telemetry_Data"))
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor())
            {
                reader.Read(0, out telemetry);
            }
        }
#pragma warning restore CA1416 // Проверка совместимости платформы
    }
    catch { continue; }
    //Console.WriteLine($"Скорость(м/с): {telemetry.speed};\nЛевый поворотник: {telemetry.l_blinker};\n" +
    //    $"Правый поворотник: {telemetry.r_blinker};\nАварийка: {telemetry.hazard};\n" +
    //    $"Ручник: {telemetry.parking_brake};\nНехватка воздуха: {telemetry.low_air};\n" +
    //    $"Нехватка топлива: {telemetry.low_fuel}");
    Console.Clear();
    string to_send = $"{(telemetry.hazard ? 1 : 0)} {(telemetry.l_blinker ? 1 : 0)} {(telemetry.r_blinker ? 1 : 0)} {(telemetry.parking_brake || telemetry.low_air || telemetry.low_fuel ? 1 : 0)}";
    Console.WriteLine(to_send);
    port.Write(to_send);
}
string GetArduinoPort()
{
    string[] ports = SerialPort.GetPortNames();
    int i = 1;
    foreach (var port in ports)
    {
        Console.WriteLine(i++ + " " + port);
    }
    bool error;
    int portIndex = 1;
    do
    {
        error = false;
        Console.Write("Введите индекс порта для чтения: ");
        try
        {
            portIndex = Convert.ToInt32(Console.ReadLine());
        }
        catch (Exception)
        {
            error = true;
        }
        if (portIndex > i - 1 || portIndex <= 0)
        {
            error = true;
        }
        if (error)
        {
            Console.WriteLine("Ошибка!!!");
        }
    } while (error);
    return ports[portIndex - 1];
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
