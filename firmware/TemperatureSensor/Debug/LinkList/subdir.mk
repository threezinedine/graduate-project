################################################################################
# Automatically-generated file. Do not edit!
# Toolchain: GNU Tools for STM32 (11.3.rel1)
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../LinkList/LinkList.c 

OBJS += \
./LinkList/LinkList.o 

C_DEPS += \
./LinkList/LinkList.d 


# Each subdirectory must supply rules for building sources it contributes
LinkList/%.o LinkList/%.su LinkList/%.cyclo: ../LinkList/%.c LinkList/subdir.mk
	arm-none-eabi-gcc "$<" -mcpu=cortex-m3 -std=gnu11 -g3 -DDEBUG -DUSE_HAL_DRIVER -DSTM32F103x6 -c -I../Core/Inc -I../Drivers/STM32F1xx_HAL_Driver/Inc/Legacy -I../Drivers/STM32F1xx_HAL_Driver/Inc -I../Drivers/CMSIS/Device/ST/STM32F1xx/Include -I../Drivers/CMSIS/Include -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/TemperatureSensor/Communication/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/TemperatureSensor/Queue/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/TemperatureSensor/TimeTrigger" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/TemperatureSensor/AddressExtractor" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/TemperatureSensor/LinkList/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/TemperatureSensor/Command/include" -I../USB_DEVICE/App -I../USB_DEVICE/Target -I../Middlewares/ST/STM32_USB_Device_Library/Core/Inc -I../Middlewares/ST/STM32_USB_Device_Library/Class/CDC/Inc -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/TemperatureSensor/StringUtils/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/TemperatureSensor/HashMap/include" -I"C:/Users/Acer/Project/Air-quality-monitoring-station/firmware/TemperatureSensor/AnalogInput/include" -Os -ffunction-sections -fdata-sections -Wall -fstack-usage -fcyclomatic-complexity -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" --specs=nano.specs -mfloat-abi=soft -mthumb -o "$@"

clean: clean-LinkList

clean-LinkList:
	-$(RM) ./LinkList/LinkList.cyclo ./LinkList/LinkList.d ./LinkList/LinkList.o ./LinkList/LinkList.su

.PHONY: clean-LinkList

