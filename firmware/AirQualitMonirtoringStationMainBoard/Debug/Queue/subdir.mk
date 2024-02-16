################################################################################
# Automatically-generated file. Do not edit!
# Toolchain: GNU Tools for STM32 (11.3.rel1)
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../Queue/Queue.c 

OBJS += \
./Queue/Queue.o 

C_DEPS += \
./Queue/Queue.d 


# Each subdirectory must supply rules for building sources it contributes
Queue/%.o Queue/%.su Queue/%.cyclo: ../Queue/%.c Queue/subdir.mk
	arm-none-eabi-gcc "$<" -mcpu=cortex-m3 -std=gnu11 -g3 -DDEBUG -DUSE_HAL_DRIVER -DSTM32F103xB -c -I../Core/Inc -I../Drivers/STM32F1xx_HAL_Driver/Inc/Legacy -I../Drivers/STM32F1xx_HAL_Driver/Inc -I../Drivers/CMSIS/Device/ST/STM32F1xx/Include -I../Drivers/CMSIS/Include -I../USB_DEVICE/App -I../USB_DEVICE/Target -I../Middlewares/ST/STM32_USB_Device_Library/Core/Inc -I../Middlewares/ST/STM32_USB_Device_Library/Class/CDC/Inc -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Queue/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/HashMap/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Command/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/StringUtils/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/LCD/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Communication/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/LinkList/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Sensors/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/LoRa/Inc" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/SX1278" -O0 -ffunction-sections -fdata-sections -Wall -fstack-usage -fcyclomatic-complexity -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" --specs=nano.specs -mfloat-abi=soft -mthumb -o "$@"

clean: clean-Queue

clean-Queue:
	-$(RM) ./Queue/Queue.cyclo ./Queue/Queue.d ./Queue/Queue.o ./Queue/Queue.su

.PHONY: clean-Queue

