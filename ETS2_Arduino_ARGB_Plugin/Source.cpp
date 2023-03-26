#include <Windows.h>
#include <assert.h>

#include "scssdk_telemetry.h"
#include "eurotrucks2/scssdk_eut2.h"
#include "eurotrucks2/scssdk_telemetry_eut2.h"
#include "amtrucks/scssdk_ats.h"
#include "amtrucks/scssdk_telemetry_ats.h"

// Структура данных
struct telemetry_state_t
{
	bool running;        // Состояние
	float speed;	     // Скорость грузовика
	float speed_limit;   // Ограничение скорости
	float throttle;		 // Газ
	scs_s32_t gear;		 // передача
	bool parking_brake;  // Ручник
	bool parking_lights; // Габариты
	bool low_air;		 // Нехватка воздуха
	bool low_fuel;		 // Нехватка топлива
	bool l_blinker;		 // Левый поворотник
	bool r_blinker;		 // Правый поворотник
	bool hazard;		 // Аварийка
};

// Глобальные переменные для общей памяти
HANDLE data_map = NULL;
telemetry_state_t* share_data = NULL;

// Функция для заноса значений типа int в общую память
SCSAPI_VOID telemetry_store_s32(const scs_string_t name, const scs_u32_t index, const scs_value_t* const value, const scs_context_t context)
{
	assert(context);
	scs_s32_t* const storage = static_cast<scs_s32_t*>(context);

	if (value) {
		assert(value->type == SCS_VALUE_TYPE_s32);
		*storage = value->value_s32.value;
	}
	else {
		*storage = 0;
	}
}

// Функция для заноса значений типа float в общую память
SCSAPI_VOID telemetry_store_float(const scs_string_t name, const scs_u32_t index, const scs_value_t* const value, const scs_context_t context)
{
	assert(context);
	scs_float_t* const storage = static_cast<scs_float_t*>(context);
	if (value) {
		assert(value->type == SCS_VALUE_TYPE_float);
		*storage = value->value_float.value;
	}
	else {
		*storage = 0.0f;
	}
}

// Функция для заноса значений типа bool в общую память
SCSAPI_VOID telemetry_store_bool(const scs_string_t name, const scs_u32_t index, const scs_value_t* const value, const scs_context_t context)
{
	assert(context);
	bool* const storage = static_cast<bool*>(context);
	if (value) {
		assert(value->type == SCS_VALUE_TYPE_bool);
		*storage = (value->value_bool.value != 0);
	}
	else {
		*storage = 0;
	}
}

// Функция смены активности телеметрии
SCSAPI_VOID telemetry_pause(const scs_event_t event, const void* const event_info, const scs_context_t context)
{
	share_data->running = (event == SCS_TELEMETRY_EVENT_started) ? 1 : 0;
}

// Инициализация телеметрии
SCSAPI_RESULT scs_telemetry_init(const scs_u32_t version, const scs_telemetry_init_params_t* const params)
{
	// Проверка версии телеметрии
	if (version != SCS_TELEMETRY_VERSION_1_00)
	{
		return SCS_RESULT_unsupported;
	}

	const scs_telemetry_init_params_v100_t* const version_params = static_cast<const scs_telemetry_init_params_v100_t*>(params); // Хранение данных об игре
	version_params->common.log(SCS_LOG_TYPE_message, "ARGB: loading...");

	data_map = CreateFileMappingA(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE | SEC_COMMIT, 0, sizeof(telemetry_state_t), "ARGB_Telemetry_Data"); // Инициализация общей памяти
	// Если не удалось
	if (!data_map)
	{
		version_params->common.log(SCS_LOG_TYPE_error, "ARGB: Failed to create shared memory");
		if (share_data) {
			UnmapViewOfFile(share_data);
			share_data = NULL;
		}
		return SCS_RESULT_generic_error;
	}

	share_data = static_cast<telemetry_state_t*>(MapViewOfFile(data_map, FILE_MAP_ALL_ACCESS, 0, 0, 0)); // Обявление данных в общей памяти
	// Если не удалось
	if (!share_data)
	{
		version_params->common.log(SCS_LOG_TYPE_error, "ARGB: Failed to vies shared memory");
		if (data_map) {
			CloseHandle(data_map);
			data_map = NULL;
		}
		return SCS_RESULT_generic_error;
	}
	// Регистрация обработчиков необходимых евентов
	version_params->register_for_event(SCS_TELEMETRY_EVENT_paused, telemetry_pause, NULL);
	version_params->register_for_event(SCS_TELEMETRY_EVENT_started, telemetry_pause, NULL);

	// Регистрация обработчиков необходимых каналов данных
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_speed, SCS_U32_NIL, SCS_VALUE_TYPE_float, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_float, &share_data->speed);
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_navigation_speed_limit, SCS_U32_NIL, SCS_VALUE_TYPE_float, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_float, &share_data->speed_limit);
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_input_throttle, SCS_U32_NIL, SCS_VALUE_TYPE_float, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_float, &share_data->throttle);
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_displayed_gear, SCS_U32_NIL, SCS_VALUE_TYPE_s32, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_s32, &share_data->gear);
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_parking_brake, SCS_U32_NIL, SCS_VALUE_TYPE_bool, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_bool, &share_data->parking_brake);
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_light_parking, SCS_U32_NIL, SCS_VALUE_TYPE_bool, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_bool, &share_data->parking_lights);
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_brake_air_pressure_warning, SCS_U32_NIL, SCS_VALUE_TYPE_bool, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_bool, &share_data->low_air);
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_fuel_warning, SCS_U32_NIL, SCS_VALUE_TYPE_bool, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_bool, &share_data->low_fuel);
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_lblinker, SCS_U32_NIL, SCS_VALUE_TYPE_bool, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_bool, &share_data->l_blinker);
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_rblinker, SCS_U32_NIL, SCS_VALUE_TYPE_bool, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_bool, &share_data->r_blinker);
	version_params->register_for_channel(SCS_TELEMETRY_TRUCK_CHANNEL_hazard_warning, SCS_U32_NIL, SCS_VALUE_TYPE_bool, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_bool, &share_data->hazard);

	version_params->common.log(SCS_LOG_TYPE_message, "ARGB: loaded");
	return SCS_RESULT_ok;
}

// Выключение телеметрии
SCSAPI_VOID scs_telemetry_shutdown(void)
{
	if (share_data) {
		UnmapViewOfFile(share_data);
		share_data = NULL;
	}
	if (data_map) {
		CloseHandle(data_map);
		data_map = NULL;
	}
}

// Точка входа
BOOL APIENTRY DllMain(
	HMODULE module,
	DWORD  reason_for_call,
	LPVOID reseved
)
{
	switch (reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		system("tasklist /FI \"IMAGENAME eq ETS2_ARGB_Settings.exe\" | find /I \"ETS2_ARGB_Settings.exe\" || start /min plugins/ETS2_ARGB_Settings.exe start_from_ets2");
		break;

	case DLL_THREAD_ATTACH:
		break;

	case DLL_THREAD_DETACH:
		break;

	case DLL_PROCESS_DETACH:
		system("taskkill /IM ETS2_ARGB_Settings.exe");
		break;
	}
	return TRUE;
}