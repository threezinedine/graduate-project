#include <WiFi.h>
#include <HTTPClient.h>
#include <HardwareSerial.h>

#define BUFFER_SIZE 128

char ssid[32]     = "Loan";
char password[32] = "12345678";

char serverAddress[32] = "192.168.112.102";
int serverPort = 45457;
char apiPath[32] = "/stations/records";

void processSerialData();

HardwareSerial Port(2);

const int bufferSize = 256;=
char serialData[bufferSize];
char stationId[64];
char sensorData[128];

void setup() {
  Port.begin(9600, SERIAL_8N1, 16, 17);
  
  Serial.begin(115200);
    while(!Serial){delay(100);}

    Serial.print("Enter the WiFi's ssid: ");
    while (!Serial.available());
    indexTemp = 0;
    memset(ssid, 0, 32);
    while(Serial.available())
    {
      ssid[indexTemp++] = Serial.read();
    }

    Serial.print(ssid);
    Serial.println();

    Serial.print("Enter the WiFi's password: ");
    while (!Serial.available());
    indexTemp = 0;
    memset(password, 0, 32);
    while(Serial.available())
    {
      password[indexTemp++] = Serial.read();
    }

    Serial.print(password);
    Serial.println();

    
    Serial.print("Enter the server address: ");
    while (!Serial.available());
    indexTemp = 0;
    memset(serverAddress, 0, 32);
    while(Serial.available())
    {
      serverAddress[indexTemp++] = Serial.read();
    }

    Serial.print(serverAddress);
    Serial.println();

    
    Serial.print("Enter the port: ");
    while (!Serial.available());
    serverPort = 0;
    while(Serial.available())
    {
      String inputString = Serial.readStringUntil('\n');
      serverPort = inputString.toInt();
    }
    Serial.print(serverPort);

    Serial.println();
    Serial.println("******************************************************");
    Serial.print("Connecting to ");
    Serial.println(ssid);

    WiFi.begin(ssid, password);

    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
    }

    Serial.println("");
    Serial.println("WiFi connected");
    Serial.println("IP address: ");
    Serial.println(WiFi.localIP());
}


void loop() {
  if (Port.available() > 0) {
    // Read the incoming serial data into the buffer
    int bytesRead = Port.readBytesUntil('\n', serialData, bufferSize - 1);
    Serial.println(bytesRead);
    Serial.println(serialData);
    serialData[bytesRead] = '\0'; // Null-terminate the string

    // Extract station ID and data using strtok function
    char *token = strtok(serialData, ";");
    
    if (token != NULL) {
      strncpy(stationId, token, sizeof(stationId) - 1);
      stationId[sizeof(stationId) - 1] = '\0'; // Null-terminate the station ID string

      token = strtok(NULL, ";");
      
      if (token != NULL) {
        strncpy(sensorData, token, sizeof(sensorData) - 1);
        sensorData[sizeof(sensorData) - 1] = '\0'; // Null-terminate the sensor data string

        // Now you can use stationId and sensorData as needed
        // Example:
        Serial.print("Station ID: ");
        Serial.println(stationId);
        Serial.print("Sensor Data: ");
        Serial.println(sensorData);

        char jsonBody[1024];
      sprintf(jsonBody, "{"
                        "\"stationId\": \"%s\","
                        "\"createdTime\":\"2023-12-29T00:25:10.626Z\","
                        "\"data\":\"%s\""
                    "}", stationId, sensorData);

      Serial.print("Json Body: ");
    Serial.println(jsonBody);
    WiFiClient client;
    if (client.connect(serverAddress, serverPort)) {
      client.print(String("POST ") + apiPath + " HTTP/1.1\r\n" +
                   "Host: " + serverAddress + "\r\n" +
                   "Content-Type: application/json\r\n" +
                   "Content-Length: " + strlen(jsonBody) + "\r\n" +
                   "Connection: close\r\n\r\n" +
                   jsonBody);
  
      while (client.connected()) {
        if (client.available()) {
          String line = client.readStringUntil('\r');
          Serial.print(line);
        }
      }
      Serial.println();
  
      client.stop();
    }
    else
    {
      Serial.println("Cannot connect to the server");
    }
      }
    }
  }
}
