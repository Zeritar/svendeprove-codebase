#include <TFT_eSPI.h>
#include <WiFiUdp.h>
#include <WakeOnLan.h>
/**
 * @file globals.h
 * @name Global definitions
 *
 * This header file contains definitions of the global variables used in the program.
 */

#ifndef GLOBALS_H
#define GLOBALS_H

#define CS 5
#define MOSI 32
#define MISO 35
#define SCK 33

extern int btnTick;

extern String ledStatus[8];
extern TFT_eSPI tft;

extern WiFiUDP UDP;
extern WakeOnLan WOL;

#endif