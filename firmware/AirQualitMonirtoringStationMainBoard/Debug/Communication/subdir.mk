################################################################################
# Automatically-generated file. Do not edit!
# Toolchain: GNU Tools for STM32 (11.3.rel1)
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../Communication/Communication.c 

OBJS += \
./Communication/Communication.o 

C_DEPS += \
./Communication/Communication.d 


# Each subdirectory must supply rules for building sources it contributes
Communication/%.o Communication/%.su Communication/%.cyclo: ../Communication/%.c Communication/subdir.mk
	arm-none-eabi-gcc "$<" -mcpu=cortex-m3 -std=gnu11 -g3 -DDEBUG -DUSE_HAL_DRIVER -DSTM32F103xB -c -I../Core/Inc -I../Drivers/STM32F1xx_HAL_Driver/Inc/Legacy -I../Drivers/STM32F1xx_HAL_Driver/Inc -I../Drivers/CMSIS/Device/ST/STM32F1xx/Include -I../Drivers/CMSIS/Include -I../USB_DEVICE/App -I../USB_DEVICE/Target -I../Middlewares/ST/STM32_USB_Device_Library/Core/Inc -I../Middlewares/ST/STM32_USB_Device_Library/Class/CDC/Inc -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Queue/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/HashMap/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Command/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/StringUtils/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/LCD/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Communication/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/LinkList/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Sensors/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/LoRa/Inc" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/SX1278" -O0 -ffunction-sections -fdata-sections -Wall -fstack-usage -fcyclomatic-complexity -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" --specs=nano.specs -mfloat-abi=soft -mthumb -o "$@"

clean: clean-Communication

clean-Communication:
	-$(RM) ./Communication/Communication.cyclo ./Communication/Communication.d ./Communication/Communication.o ./Communication/Communication.su

.PHONY: clean-Communication

