#include <Arduino.h>
#include <ArduinoJson.h>
#include <globals.h>
#include <AsyncWebSocket.h>

class PC{
    public:
        PC(int id, String name, bool isNetworked, uint32_t ip, uint8_t mac[6], uint8_t pins[2], AsyncWebSocket *ws);
        PC();
        int id;
        String name;
        bool isNetworked;
        bool isValid;
        uint32_t ip;
        uint8_t mac[6];
        uint8_t pins[2];
        AsyncWebSocket *ws;
        void validate();
        bool doCommand(String command);
        bool heartbeat();
        bool shutdown();
        bool start();
        String toJson();
    private:
        bool SSH_Shutdown();
};