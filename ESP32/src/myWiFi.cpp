#include <myWiFi.h>
#include <ArduinoJson.h>
#include <pc.h>

// Create AsyncWebServer object on port 80
AsyncWebServer server(80);

AsyncWebSocket ws("/ws");

// Search for parameter in HTTP POST request
const char* PARAM_INPUT_1 = "ssid";
const char* PARAM_INPUT_2 = "pass";
const char* PARAM_INPUT_3 = "ip";
const char* PARAM_INPUT_4 = "gateway";


//Variables to save values from HTML form
String ssid;
String pass;
String ip;
String gateway;

// File paths to save input values permanently
const char* ssidPath = "/wifi/ssid.txt";
const char* passPath = "/wifi/pass.txt";
const char* ipPath = "/wifi/ip.txt";
const char* gatewayPath = "/wifi/gateway.txt";

IPAddress localIP;
//IPAddress localIP(192, 168, 1, 200); //hardcoded

// Set your Gateway IP address
IPAddress localGateway;
//IPAddress localGateway(192, 168, 1, 1); //hardcoded
IPAddress subnet(255, 255, 255, 0);

// Timer variables
unsigned long previousMillis = 0;
const long interval = 10000;  // interval to wait for Wi-Fi connection (milliseconds)

String ledState;

WiFiUDP UDP;
WakeOnLan WOL(UDP);

// Initialize WiFi
bool initWiFi() {
  if(ssid=="" || ip==""){
    Serial.println("Undefined SSID or IP address.");
    return false;
  }

  WiFi.mode(WIFI_STA);
  localIP.fromString(ip.c_str());
  localGateway.fromString(gateway.c_str());


  if (!WiFi.config(localIP, localGateway, subnet)){
    Serial.println("STA Failed to configure");
    return false;
  }
  WiFi.begin(ssid.c_str(), pass.c_str());
  Serial.println("Connecting to WiFi...");

  unsigned long currentMillis = millis();
  previousMillis = currentMillis;

  while(WiFi.status() != WL_CONNECTED) {
    currentMillis = millis();
    if (currentMillis - previousMillis >= interval) {
      Serial.println("Failed to connect.");
      return false;
    }
  }

  Serial.println(WiFi.localIP());
  return true;
}

void setupWifi() {
  setupSD();
  
  // Load values saved in SPIFFS
  ssid = readFileSD(SD, ssidPath);
  pass = readFileSD(SD, passPath);
  ip = readFileSD(SD, ipPath);
  gateway = readFileSD(SD, gatewayPath);
  Serial.println(ssid);
  Serial.println(pass);
  Serial.println(ip);
  Serial.println(gateway);

  if(initWiFi()) {
    // Route to accept HTTP POST request for PC commands
    server.onRequestBody([](AsyncWebServerRequest *request, uint8_t *data, size_t len, size_t index, size_t total){
      if (request->url() == "/command") {
        if (!parseRequest(request, data))
          request->send(200, "text/plain", "false");

        request->send(200, "text/plain", "true");
      }
    });

    server.begin();
    
    WOL.setRepeat(3, 100);
    WOL.calculateBroadcastAddress(WiFi.localIP(), WiFi.subnetMask());

    tft.println("Ready.");
    tft.println("Waiting for API..");

    initWebSocket();
  }
  else {
    // Connect to Wi-Fi network with SSID and password
    Serial.println("Setting AP (Access Point)");
    // NULL sets an open Access Point
    WiFi.softAP("ESP-WIFI-MANAGER", NULL);
    tft.println("ESP-WIFI-MANAGER");

    IPAddress IP = WiFi.softAPIP();
    Serial.print("AP IP address: ");
    tft.println("AP IP address: ");
    Serial.println(IP);
    tft.println(IP);

    // Web Server Root URL
    server.on("/", HTTP_GET, [](AsyncWebServerRequest *request){
      request->send(SD, "/www/wifimanager.html", "text/html");
    });
    
    server.serveStatic("/", SD, "/www/");
    
    server.on("/", HTTP_POST, [](AsyncWebServerRequest *request) {
      int params = request->params();
      for(int i=0;i<params;i++){
        AsyncWebParameter* p = request->getParam(i);
        if(p->isPost()){
          // HTTP POST ssid value
          if (p->name() == PARAM_INPUT_1) {
            ssid = p->value().c_str();
            Serial.print("SSID set to: ");
            Serial.println(ssid);
            // Write file to save value
            writeFileSD(SD, ssidPath, ssid.c_str());
          }
          // HTTP POST pass value
          if (p->name() == PARAM_INPUT_2) {
            pass = p->value().c_str();
            Serial.print("Password set to: ");
            Serial.println(pass);
            // Write file to save value
            writeFileSD(SD, passPath, pass.c_str());
          }
          // HTTP POST ip value
          if (p->name() == PARAM_INPUT_3) {
            ip = p->value().c_str();
            Serial.print("IP Address set to: ");
            Serial.println(ip);
            // Write file to save value
            writeFileSD(SD, ipPath, ip.c_str());
          }
          // HTTP POST gateway value
          if (p->name() == PARAM_INPUT_4) {
            gateway = p->value().c_str();
            Serial.print("Gateway set to: ");
            Serial.println(gateway);
            // Write file to save value
            writeFileSD(SD, gatewayPath, gateway.c_str());
          }
        }
      }
      request->send(200, "text/plain", "Done. ESP will restart, connect to your router and go to IP address: " + ip);
      delay(3000);
      ESP.restart();
    });
    server.begin();
  }
}

bool parseRequest(AsyncWebServerRequest *request, uint8_t *datas) {

  Serial.println("Received POST request");
  
  JsonDocument jsonBuffer;
  
  try
  {
    deserializeJson(jsonBuffer, (char*)datas);
    int id;
    String command;
    String name;
    uint32_t ip;
    uint8_t mac[6];
    uint8_t pins[2];
    bool isNetworked;
    PC pc;

    command = jsonBuffer["Command"].as<String>();

    if (command == "null")
      throw DeserializationError();

    if (command == "update")
    {
      copyArray(jsonBuffer["Body"].as<JsonArray>(), ledStatus);

      tft.fillScreen(TFT_BLACK);
      tft.setCursor(0, 0, 2);
      tft.setTextSize(1);

      for (int i = 0; i < 8; i++)
      {
        if (ledStatus[i] == "" || ledStatus[i] == "null") break;
        else if (ledStatus[i] == "ONLINE") tft.setTextColor(TFT_GREEN, TFT_BLACK);
        else if (ledStatus[i] == "OFFLINE") tft.setTextColor(TFT_RED, TFT_BLACK);
        else tft.setTextColor(TFT_WHITE, TFT_BLACK);
        
        tft.println(ledStatus[i]);
      }
      
      return true;
    }

    id = jsonBuffer["Body"]["Id"].as<int>();
    name = jsonBuffer["Body"]["Name"].as<String>();
    isNetworked = jsonBuffer["Body"]["IsNetworked"].as<bool>();
    ip = jsonBuffer["Body"]["IpAddress"].as<uint32_t>();
    copyArray(jsonBuffer["Body"]["MacAddress"].as<JsonArray>(), mac);
    copyArray(jsonBuffer["Body"]["Pins"].as<JsonArray>(), pins);

    pc = PC(id, name, isNetworked, ip, mac, pins, &ws);

    Serial.println("Command: " + command);

    return pc.doCommand(command);
  }
  catch(const DeserializationError e)
  {
    Serial.println("Error parsing JSON object from POST request");
    return 0;
  }
}

void notifyClients(String message) {
  ws.textAll(message);
}

void onEvent(AsyncWebSocket *server, AsyncWebSocketClient *client, AwsEventType type, void *arg, uint8_t *data, size_t len) {
  switch (type) {
    case WS_EVT_CONNECT:
      Serial.printf("WebSocket client #%u connected from %s\n", client->id(), client->remoteIP().toString().c_str());
      break;
    case WS_EVT_DISCONNECT:
      Serial.printf("WebSocket client #%u disconnected\n", client->id());
      break;
    case WS_EVT_DATA: // Event handler for receiving data would go here, but we will never receive any messages from the client
    case WS_EVT_PONG:
    case WS_EVT_ERROR:
      break;
  }
}

void initWebSocket() {
  ws.onEvent(onEvent);
  server.addHandler(&ws);
}