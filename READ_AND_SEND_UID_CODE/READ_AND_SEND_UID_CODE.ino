#include <MFRC522v2.h>
#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <MFRC522DriverSPI.h>
//#include <MFRC522DriverI2C.h>
#include <MFRC522DriverPinSimple.h>
#include <MFRC522Debug.h>

#ifndef STASSID 
#define STASSID "OMiLAB"
#define STAPSK "digifofulbs"
#define serverUrl "http://10.14.10.113:3000/api/data"
#endif

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

  while (!Serial);       // Do nothing if no serial port is opened (added for Arduinos based on ATMEGA32U4).
  
  mfrc522.PCD_Init();    // Init MFRC522 board.
  MFRC522Debug::PCD_DumpVersionToSerial(mfrc522, Serial);
	Serial.println(F("Scan PICC to see UID"));
}

void loop() {

  unsigned long currentTime = millis();

  // Only allow a new scan if 5 seconds have passed
  if (currentTime - lastReadTime < readInterval) {
    return;  
  }

	// Reset the loop if no new card present on the sensor/reader. This saves the entire process when idle.
	if (!mfrc522.PICC_IsNewCardPresent()) {
		return;
	}

	// Select one of the cards.
	if (!mfrc522.PICC_ReadCardSerial()) {
		return;
	}

   // Card detected, update last read time
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

  String HolderName = "Darius si Adina";
  sendUIDToServer(uidString, HolderName);
}


void sendUIDToServer(String uid, String name) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    WiFiClient client;

    http.begin(client, serverUrl);
    http.addHeader("Content-Type", "application/json");
    
    String jsonPayload = "{\"id\":\"" + uid + "\", \"name\":\"" + name + "\"}";
    
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
