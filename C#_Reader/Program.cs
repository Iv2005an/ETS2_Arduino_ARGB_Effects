//#define debug

using System.IO.MemoryMappedFiles;
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
    // Протокол: аварийка(0-1)л.поворотник(0-1)п.поворотник(0-1)опасность(0-1)R(000-255)G(000-255)B(000-255)
    string to_send = telemetry.running ? (
        (telemetry.hazard ? "1" : "0") +
        (telemetry.l_blinker ? "1" : "0") +
        (telemetry.r_blinker ? "1" : "0") +
        (telemetry.low_fuel || telemetry.low_air || telemetry.throttle > 0 && telemetry.parking_brake && telemetry.gear != 0 || telemetry.speed_limit + 1.6 < telemetry.speed ? "1" : "0") +
        (telemetry.parking_lights ? $"{100:D3}{0:D3}{0:D3}" : "000000000")
        ) : "0000000000000";
    port.Write(to_send);
#if debug
    Console.WriteLine($"Состояние: {telemetry.running}\n" +
        $"Скорость грузовика: {telemetry.speed}\n" +
        $"Ограничение скорости: {telemetry.speed_limit}\n" +
        $"Газ: {telemetry.throttle}\n" +
        $"Передача: {telemetry.gear}\n" +
        $"Ручник: {telemetry.parking_brake}\n" +
        $"Габариты: {telemetry.parking_lights}\n" +
        $"Нехватка воздуха: {telemetry.low_air}\n" +
        $"Нехватка топлива: {telemetry.low_fuel}\n" +
        $"Левый поворотник: {telemetry.l_blinker}\n" +
        $"Правый поворотник: {telemetry.r_blinker}\n" +
        $"Аварийка: {telemetry.hazard}");
#endif
    Thread.Sleep(100);
}
string GetArduinoPort()
{
    string[] ports = SerialPort.GetPortNames();
    int i = 1;
    foreach (var port in ports)
    {
        Console.WriteLine(i++ + ". " + port);
    }
    bool error;
    int portIndex = 1;
    do
    {
        error = false;
        Console.Write("Введите индекс порта arduino: ");
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

// Структура данных
struct telemetry_state_t
{
    internal bool running;        // Состояние
    internal float speed;         // Скорость грузовика
    internal float speed_limit;   // Ограничение скорости
    internal float throttle;	  // Газ
    internal int gear;			  // передача
    internal bool parking_brake;  // Ручник
    internal bool parking_lights; // Габариты
    internal bool low_air;        // Нехватка воздуха
    internal bool low_fuel;       // Нехватка топлива
    internal bool l_blinker;      // Левый поворотник
    internal bool r_blinker;      // Правый поворотник
    internal bool hazard;		  // Аварийка
};
