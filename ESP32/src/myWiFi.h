#include <Arduino.h>

#include <sdhandler.h>
#include <WiFi.h>
#include <ESPAsyncWebServer.h>
#include <AsyncTCP.h>

bool initWiFi();
void setupWifi();
bool parseRequest(AsyncWebServerRequest *request, uint8_t *datas);
void initWebSocket();