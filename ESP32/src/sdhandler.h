/**
 * @file sdhandler.h
 * @name SDHandler.h
 * @ref https://randomnerdtutorials.com/esp32-microsd-card-arduino/
 *
 * This header file contains function prototypes for handling the SD card.
 * All functions come from the linked tutorial.
 * 
 */
#include <Arduino.h>
#include <globals.h>

#include <FS.h>
#include <SD.h>
#include <SPI.h>

void listDirSD(fs::FS &fs, const char * dirname, uint8_t levels);
void createDirSD(fs::FS &fs, const char * path);
void removeDirSD(fs::FS &fs, const char * path);
String readFileSD(fs::FS &fs, const char * path);
void writeFileSD(fs::FS &fs, const char * path, const char * message);
void appendFileSD(fs::FS &fs, const char * path, const char * message);
void renameFileSD(fs::FS &fs, const char * path1, const char * path2);
void deleteFileSD(fs::FS &fs, const char * path);
void testFileIOSD(fs::FS &fs, const char * path);

/**
 * @brief Sets up the SD card reader and tries to mount the SD card.
 * 
 */
void setupSD();