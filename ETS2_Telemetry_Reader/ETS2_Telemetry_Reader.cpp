#include <Windows.h>
#include <iostream>

using namespace std;

int main() {
	// Структура данных
#pragma pack(push)
#pragma pack(1)
	struct telemetry_state_t
	{
		float speed;			 // Скорость грузовика
		bool parking_brake;		 // Ручник
		bool low_air;			 // Нехватка воздуха
		bool low_fuel;			 // Нехватка топлива
		bool l_blinker;			 // Левый поворотник
		bool r_blinker;			 // Правый поворотник
		bool hazard;			 // Аварийка
	};
#pragma pack(pop)

	while (true)
	{
		// SCSTelemetryExample 
		HANDLE data_map = OpenFileMappingA(FILE_MAP_READ, false, "ARGB_Telemetry_Data");
		if (!data_map)
		{
			continue;
		}
		telemetry_state_t* data = static_cast<telemetry_state_t*>(MapViewOfFile(data_map, FILE_MAP_READ, 0, 0, 0));
		if (!data)
		{
			continue;
		}
		cout << data->speed << " " << data->parking_brake << " " << data->low_air << " " << data->low_fuel << " " << data->l_blinker << " " << data->r_blinker << " " << data->hazard << endl;
	}
}