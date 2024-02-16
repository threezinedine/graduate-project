/*
 * The gioi linh kien dien tu
 * IC DAY ROI
 * Website: www.icdayroi.com
 * SDT: 035 618 4078
 * Dia chi: 4 Duong So 7, P. Linh Trung, TP. Thu Duc, TP. HCM
 */


#include <SoftwareSerial.h>

SoftwareSerial mySerial(2, 3); // RX, TX
// pin 2 -> TXD lora
// pin 3 -> RXD lora


int BAUD[] = {1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200};
float RATE[] = {0.3, 1.2, 2.4, 4.8, 9.6, 19.2, 19.2, 19.2};
int WAKEUP[] = {250, 500, 750, 1000, 1250, 1500, 1750, 2000};
int POWER[] = {20, 17, 14, 11};

void setup() {
  Serial.begin(9600);
  mySerial.begin(9600);
  delay(200);

// writeConfigLora(05, 9600, 2.4, 15, 250, true, 20);
// delay(500); //delay chut moi read duoc
}

void loop() {
  Serial.println("Read Config");
  readConfigLora();
  delay(5000);
}

/*
 * address: 0 -> 65535
 * baud -> check array BAUD
 * rate -> check array RATE
 * channel: 0 -> 31
 * wakeUp: check array WAKEUP
 * turnOnFEC: true, false
 * power -> check array power
 */
void writeConfigLora(int address, int baud, float rate, int channel, int wakeUp, boolean turnOnFEC, int power) {
  
  Serial.println("\nWrite config...");
  
  byte data[6];
  data[0] = 0xC0;
  data[1] = (address >> 8);
  data[2] = address;

  //config baud
  data[3] = B00011010;
  for (int i = 0; i < 8; i++) {
    if (baud == BAUD[i]) {
      data[3] = ((data[3] & B11000111) | (i << 3));
      break;
    }
  }
  //end config baud

  //config data rate
  for (int i = 0; i < 8; i++) {
    if (rate == RATE[i]) {
      data[3] = ((data[3] & B11111000) | i);
      break;
    }
  }
  //end config data rate

  //config channel
  data[4] = (channel & B00011111);
  //end config channel

  data[5] = B01000000;
  //config data rate
  for (int i = 0; i < 8; i++) {
    if (wakeUp == WAKEUP[i]) {
      data[5] = ((data[5] & B11000111) | (i << 3));
      break;
    }
  }
  //end config data rate

  //config channel
  data[5] = turnOnFEC ? (data[5] | B00000100) : (data[5] & B11111011);
  //end config channel
  
  //config power
  for (int i = 0; i < 4; i++) {
    if (power == POWER[i]) {
      data[5] = ((data[5] & B11111100) | i);
      break;
    }
  }
  //end config power
  
  for (int i = 0; i < 6; i++) {
    mySerial.write(data[i]);
  }
  Serial.println("Write config done\n");
}

void readConfigLora() {
  Serial.println("\nRead config\n");

  mySerial.write(0xC1);
  mySerial.write(0xC1);
  mySerial.write(0xC1);

  long t = millis() + 2000;
  byte data[6];
  int i = 0;
  while (millis() < t) {
    if (mySerial.available()) {
      if (i == 0) {
        data[i] = mySerial.read();
        if (data[i] == 0xC0) i++;
      } else {
        data[i++] = mySerial.read();
        if (i >= 6) break;
      }
    }
  }
  if (i >= 6) {
    Serial.println("Read success");
    
    int address = data[1];
    address = address << 8;
    address = address | data[2];
    int baud = ((data[3] & B00111000) >> 3);
    int rate = (data[3] & B00000111);
    int chan = (data[4] & B00011111);
    int wakeUp = ((data[5] & B00111000) >> 3);
    int fec = ((data[5] & B00000100) >> 2);
    int power = (data[5] & B00000011);

    Serial.print("Address: ");
    Serial.println(address);
    
    Serial.print("Baud: ");
    Serial.print(BAUD[baud]);
    Serial.println("bps");
    
    Serial.print("Data rate: ");
    Serial.print(RATE[rate]);
    Serial.println("Kbps");
    
    Serial.print("Channel: ");
    Serial.print(chan);
    Serial.print(" => ");
    Serial.print(410 + chan);
    Serial.println("MHz");
    
    Serial.print("Wake-up time: ");
    Serial.print(WAKEUP[wakeUp]);
    Serial.println("ms");
    
    Serial.print("FEC: ");
    Serial.println((fec == 1) ? "Turn on" : "Turn off");
    
    Serial.print("Power: ");
    Serial.print(POWER[power]);
    Serial.println("dBm");
  } else {
    Serial.println("Read fail");
  }
}
