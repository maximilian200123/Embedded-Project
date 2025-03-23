#include <MFRC522v2.h>
#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <MFRC522DriverSPI.h>
//#include <MFRC522DriverI2C.h>
#include <MFRC522DriverPinSimple.h>
#include <MFRC522Debug.h>
#include <NTPClient.h>
#include <WiFiUdp.h>

#ifndef STASSID 
#define STASSID "OMiLAB"
#define STAPSK "digifofulbs"
#define serverUrl "http://10.14.11.134:5041/api/data"
#endif

// Define NTP Client to get time
WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "pool.ntp.org");

// Learn more about using SPI/I2C or check the pin assigment for your board: https://github.com/OSSLibraries/Arduino_MFRC522v2#pin-layout
MFRC522DriverPinSimple ss_pin(2);

MFRC522DriverSPI driver{ss_pin}; // Create SPI driver
//MFRC522DriverI2C driver{};     // Create I2C driver
MFRC522 mfrc522{driver};         // Create MFRC522 instance

unsigned long lastReadTime = 0;  // Store the last read time
const unsigned long readInterval = 5000; // 5 seconds


void setup() {
  Serial.begin(115200);  // Initialize serial communication

  WiFi.begin(STASSID, STAPSK);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected! IP address: ");
  Serial.println(WiFi.localIP());

  // Initialize a NTPClient to get time
  timeClient.begin();

  timeClient.setTimeOffset(0);

  while (!Serial);       // Do nothing if no serial port is opened (added for Arduinos based on ATMEGA32U4).
  
  mfrc522.PCD_Init();    // Init MFRC522 board.
  MFRC522Debug::PCD_DumpVersionToSerial(mfrc522, Serial);
	Serial.println(F("Scan PICC to see UID"));
}

void loop() {

  unsigned long currentTime = millis();

  timeClient.update();

  String formattedTime = timeClient.getFormattedTime();

  // Only allow a new scan if 5 seconds have passed
  if (currentTime - lastReadTime < readInterval) {
    return;  
  }

	// Reset the loop if no new card present on the sensor/reader. This saves the entire process when idle.
	if (!mfrc522.PICC_IsNewCardPresent()) {
		return;
	}

	if (!mfrc522.PICC_ReadCardSerial()) {
		return;
	}

  lastReadTime = millis();  

  Serial.print("Card UID: ");
  MFRC522Debug::PrintUID(Serial, (mfrc522.uid));
  Serial.println();

  // Save the UID on a String variable
  String uidString = "";
  for (byte i = 0; i < mfrc522.uid.size; i++) {
    if (mfrc522.uid.uidByte[i] < 0x10) {
      uidString += "0"; 
    }
    uidString += String(mfrc522.uid.uidByte[i], HEX);
  }
  Serial.println(uidString);

  sendUIDToServer(uidString);
}

void sendUIDToServer(String uid) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    WiFiClient client;

    http.begin(client, serverUrl);
    http.addHeader("Content-Type", "application/json");

    timeClient.update();

    unsigned long epochTime = timeClient.getEpochTime();

    Serial.print("Epoch time: ");
    Serial.println(epochTime);

    if (epochTime < 1000000000) {
      Serial.println("Epoch time is invalid, something went wrong with time synchronization.");
      return;
    }

    int year = 1970 + (epochTime / 31536000);
    int days = epochTime / 86400;
    int hours = (epochTime % 86400) / 3600;
    int minutes = (epochTime % 3600) / 60;
    int seconds = epochTime % 60;

    char timeBuffer[25];
    sprintf(timeBuffer, "%04d-%02d-%02dT%02d:%02d:%02dZ", year, 1, 1, hours, minutes, seconds);  // Dummy date for now

    String formattedTime = String(timeBuffer);

    String jsonPayload = "{\"idGarbageBin\":\"" + uid + "\", \"collectionTime\":\"" + formattedTime + "\"}";

    Serial.println("Sending JSON: " + jsonPayload);

    int httpResponseCode = http.POST(jsonPayload);

    if (httpResponseCode > 0) {
      Serial.println("Server Response: " + http.getString());
    } else {
      Serial.println("Error sending request: " + String(httpResponseCode));
    }

    http.end();
  } else {
    Serial.println("WiFi not connected");
  }
}




