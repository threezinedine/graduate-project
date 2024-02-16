################################################################################
# Automatically-generated file. Do not edit!
# Toolchain: GNU Tools for STM32 (11.3.rel1)
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../SX1278/Src/SX1278.c \
../SX1278/Src/SX1278_hw.c 

OBJS += \
./SX1278/Src/SX1278.o \
./SX1278/Src/SX1278_hw.o 

C_DEPS += \
./SX1278/Src/SX1278.d \
./SX1278/Src/SX1278_hw.d 


# Each subdirectory must supply rules for building sources it contributes
SX1278/Src/%.o SX1278/Src/%.su SX1278/Src/%.cyclo: ../SX1278/Src/%.c SX1278/Src/subdir.mk
	arm-none-eabi-gcc "$<" -mcpu=cortex-m3 -std=gnu11 -g3 -DDEBUG -DUSE_HAL_DRIVER -DSTM32F103xB -c -I../Core/Inc -I../Drivers/STM32F1xx_HAL_Driver/Inc/Legacy -I../Drivers/STM32F1xx_HAL_Driver/Inc -I../Drivers/CMSIS/Device/ST/STM32F1xx/Include -I../Drivers/CMSIS/Include -I../USB_DEVICE/App -I../USB_DEVICE/Target -I../Middlewares/ST/STM32_USB_Device_Library/Core/Inc -I../Middlewares/ST/STM32_USB_Device_Library/Class/CDC/Inc -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Queue/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/HashMap/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Command/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/StringUtils/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/LCD/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Communication/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/LinkList/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/Sensors/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/LoRa/Inc" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/AirQualitMonirtoringStationMainBoard/SX1278/Inc" -O0 -ffunction-sections -fdata-sections -Wall -fstack-usage -fcyclomatic-complexity -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" --specs=nano.specs -mfloat-abi=soft -mthumb -o "$@"

clean: clean-SX1278-2f-Src

clean-SX1278-2f-Src:
	-$(RM) ./SX1278/Src/SX1278.cyclo ./SX1278/Src/SX1278.d ./SX1278/Src/SX1278.o ./SX1278/Src/SX1278.su ./SX1278/Src/SX1278_hw.cyclo ./SX1278/Src/SX1278_hw.d ./SX1278/Src/SX1278_hw.o ./SX1278/Src/SX1278_hw.su

.PHONY: clean-SX1278-2f-Src
