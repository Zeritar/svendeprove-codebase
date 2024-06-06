#include <Arduino.h>

 // Graphics and font library for ST7735 driver chip
#include <SPI.h>
#include <myWiFi.h>
#include <pc.h>
#include <ESP32ping.h>
#include <ArduinoJson.h>
//#include <sdhandler.h>

TFT_eSPI tft = TFT_eSPI();  // Invoke library, pins defined in User_Setup.h

#define TFT_GREY 0x5AEB // New colour

int btnTick = 0;

TaskHandle_t Task1;

String ledStatus[8];

void task1Func(void * pvParameters);

void setup(void) {
  tft.init();
  tft.setRotation(0);
  tft.fillScreen(TFT_BLACK);
  Serial.begin(115200);
  // setupSD();
  // listDirSD(SD, "/", 0);

  setupWifi();
}

void loop() {

}