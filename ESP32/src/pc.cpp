#include <pc.h>
#include <Arduino.h>
#include <ESP32ping.h>
#include <libssh/libssh.h>
#include <libssh_esp32.h>

void shutdownTaskFunc(void * pvParameters);
void startupTaskFunc(void * pvParameters);
void awaitTaskFunc(void * pvParameters);

enum class PC_COMMAND
{
    STARTUP,
    SHUTDOWN
};

struct param_struct
{
    PC pc;
    AsyncWebSocket *ws;
    PC_COMMAND command;
} typedef params_t;

PC::PC(int id, String name, bool isNetworked, uint32_t ip, uint8_t mac[6], uint8_t pins[2], AsyncWebSocket *ws)
{
    this->id = id;
    this->name = name;
    this->isNetworked = isNetworked;
    this->ip = ip;
    this->ws = ws;
    for(int i = 0; i < 6; ++i)
        this->mac[i] = mac[i];
    for(int i = 0; i < 2; ++i)
        this->pins[i] = pins[i];
    validate();
}

PC::PC()
{
    isValid = false;
}

void PC::validate()
{
    isValid = false;
    if (isNetworked)
    {
        if (ip == 0 || mac[0] == 0 && mac[1] == 0 && mac[2] == 0 && mac[3] == 0 && mac[4] == 0 && mac[5] == 0)
        {
            Serial.println("PC invalid");
            return;
        }
    }
    else
    {
        if (pins[0] == 0 || pins[1] == 0)
        {
            Serial.println("PC invalid");
            return;
        }
        pinMode(pins[0], INPUT_PULLUP);
        pinMode(pins[1], OUTPUT);
    }
    isValid = true;
}

bool PC::doCommand(String command)
{
    if (command == "heartbeat")
        return heartbeat();
    if (command == "shutdown")
        return shutdown();
    if (command == "start")
        return start();
    return false;
}

bool PC::heartbeat()
{
    if (!isNetworked)
        return (digitalRead(pins[0]) == LOW);
    
    return Ping.ping(ip, (byte)1U);
}

bool PC::shutdown()
{
    if (isNetworked)
    {
        params_t *params = new params_t { *this, ws, PC_COMMAND::SHUTDOWN};
        xTaskCreate(awaitTaskFunc, "AwaitTask", 2048, params, 5, NULL);
        return SSH_Shutdown();
    }
        
    if (heartbeat())
    {
        Serial.println("Shutting down");
        xTaskCreate(shutdownTaskFunc, "ShutdownTask", 2048, &pins[1], 5, NULL);
        params_t *params = new params_t { *this, ws, PC_COMMAND::SHUTDOWN};
        xTaskCreate(awaitTaskFunc, "AwaitTask", 2048, params, 5, NULL);
        return true;
    }
    return false;
}

bool PC::start()
{
    if (heartbeat())
    {
        return false;
    }

    if (isNetworked)
    {
        String mac = "";
        for(int i = 0; i < 6; ++i)
        {
            if (this->mac[i] < 16)
                mac += "0" + (String(this->mac[i], HEX));
            else
                mac += String(this->mac[i], HEX);
            if (i < 5)
                mac += ":";
        }
        WOL.sendMagicPacket(mac);
        params_t *params = new params_t { *this, ws, PC_COMMAND::STARTUP};
        xTaskCreate(awaitTaskFunc, "AwaitTask", 2048, params, 5, NULL);
        return true;
    }

    Serial.println("Starting");
    xTaskCreate(startupTaskFunc, "StartupTask", 2048, &pins[1], 5, NULL);
    params_t *params = new params_t { *this, ws, PC_COMMAND::STARTUP};
    xTaskCreate(awaitTaskFunc, "AwaitTask", 2048, params, 5, NULL);
    return true;
}

String PC::toJson()
{
    JsonDocument doc;
    String json;
    doc["name"] = name;
    doc["isNetworked"] = isNetworked;
    doc["ip"] = ip;
    
    JsonArray macArr = doc["mac"].to<JsonArray>();
    for(int i = 0; i < 6; ++i)
        if (mac[i] < 16)
            macArr.add("0" + (String(mac[i], HEX)));
        else
            macArr.add(String(mac[i], HEX));

    JsonArray pinsArr = doc["pins"].to<JsonArray>();
    for(int i = 0; i < 2; ++i)
        pinsArr.add(pins[i]);

    serializeJson(doc, json);
    return json;
}

bool PC::SSH_Shutdown()
{
    ssh_session session;
    char buffer[256];
    int nbytes;
    ssh_channel channel;
    int rc;
    String user = "user";
    String password = "password";
    String ipaddr = IPAddress(ip).toString();

    session = ssh_new();
    if (session == NULL)
    {
        Serial.println("Failed to create session");
        return false;
    }

    ssh_options_set(session, SSH_OPTIONS_HOST, ipaddr.c_str());
    ssh_options_set(session, SSH_OPTIONS_USER, user.c_str());

    rc = ssh_connect(session);
    if (rc != SSH_OK)
    {
        Serial.println("Failed to connect with error:");
        Serial.println(ssh_get_error(session));
        ssh_free(session);
        return false;
    }

    rc = ssh_userauth_password(session, NULL, password.c_str());
    if (rc != SSH_AUTH_SUCCESS)
    {
        fprintf(stderr, "Error authenticating with password: %s\n",
                ssh_get_error(session));
        ssh_disconnect(session);
        ssh_free(session);
        return false;
    }

    channel = ssh_channel_new(session);
    if (channel == NULL)
    {
        Serial.println("Failed to create channel");
        ssh_free(session);
        return false;
    }

    rc = ssh_channel_open_session(channel);
    if (rc != SSH_OK)
    {
        Serial.println("Failed to open channel");
        ssh_channel_free(channel);
        return false;
    }

    rc = ssh_channel_request_exec(channel, "echo password | sudo -S poweroff");
    if (rc != SSH_OK)
    {
        Serial.println("Failed to execute command");
        ssh_channel_close(channel);
        ssh_channel_free(channel);
        return false;
    }

    ssh_channel_send_eof(channel);
    ssh_channel_close(channel);
    ssh_channel_free(channel);

    return true;
}

void shutdownTaskFunc(void * pvParameters){
    uint8_t pin = *((uint8_t*)pvParameters);
    digitalWrite(pin, HIGH);
    vTaskDelay(pdMS_TO_TICKS(8000));
    digitalWrite(pin, LOW);
    vTaskDelete(NULL);
}

void startupTaskFunc(void * pvParameters){
    uint8_t pin = *((uint8_t*)pvParameters);
    digitalWrite(pin, HIGH);
    vTaskDelay(pdMS_TO_TICKS(200));
    digitalWrite(pin, LOW);
    vTaskDelete(NULL);
}

void awaitTaskFunc(void * pvParameters){
    PC pc = ((params_t*)pvParameters)->pc;
    AsyncWebSocket *ws = ((params_t*)pvParameters)->ws;
    PC_COMMAND command = ((params_t*)pvParameters)->command;
    uint8_t pings = 0;
    if (pc.isNetworked)
    {
        for(;;)
        {
            vTaskDelay(pdMS_TO_TICKS(5000));
            // If starting, wait for heartbeat to be true. Otherwise, wait for false.
            if (pc.heartbeat() == (command == PC_COMMAND::STARTUP))
            {
                Serial.println("PC command succeeded");

                AsyncWebSocket& ref = *ws;
                if (command == PC_COMMAND::STARTUP)
                    ref.textAll("{" + String(pc.id) + ",\"start\",\"Success\"}");
                else
                    ref.textAll("{" + String(pc.id) + ",\"shutdown\",\"Success\"}");

                vTaskDelete(NULL);
            }
            pings++;
            if (pings == 8)
            {
                Serial.println("PC command did not succeed");
                AsyncWebSocket& ref = *ws;
                if (command == PC_COMMAND::STARTUP)
                    ref.textAll("{" + String(pc.id) + ",\"start\",\"Failed\"}");
                else
                    ref.textAll("{" + String(pc.id) + ",\"shutdown\",\"Failed\"}");
                vTaskDelete(NULL);
            }
        }
    }
    else
    {
        for(;;)
        {
            vTaskDelay(pdMS_TO_TICKS(1000));
            // If starting, wait for heartbeat to be true. Otherwise, wait for false.
            if (pc.heartbeat() == (command == PC_COMMAND::STARTUP))
            {
                Serial.println("PC command succeeded");
                
                AsyncWebSocket& ref = *ws;
                if (command == PC_COMMAND::STARTUP)
                    ref.textAll("{" + String(pc.id) + ",\"start\",\"Success\"}");
                else
                    ref.textAll("{" + String(pc.id) + ",\"shutdown\",\"Success\"}");

                vTaskDelete(NULL);
            }
            pings++;
            if (pings == 10)
            {
                Serial.println("PC command did not succeed");
                AsyncWebSocket& ref = *ws;
                if (command == PC_COMMAND::STARTUP)
                    ref.textAll("{" + String(pc.id) + ",\"start\",\"Failed\"}");
                else
                    ref.textAll("{" + String(pc.id) + ",\"shutdown\",\"Failed\"}");
                vTaskDelete(NULL);
            }
        }
    }
}