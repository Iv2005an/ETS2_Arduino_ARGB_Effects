#include "FastLED.h"

#define LED_PIN 2    // пин ленты
#define NUMLEDS 102  // количество светодиодов

CRGB argb[NUMLEDS];
byte brightness = 100;     // яркость 0-255
float blinker_time = 2.0;  // время анимации поворотника(секунды)
void l_blinler() {
  for (int i = NUMLEDS; i >= 0; i--) {
    argb[i] = CRGB::OrangeRed;
    FastLED.show();
    delayMicroseconds(blinker_time / 2 * 1000000 / NUMLEDS / 2);
  }
  for (int i = NUMLEDS; i >= 0; i--) {
    argb[i] = CRGB::Black;
    FastLED.show();
    delayMicroseconds(blinker_time / 2 * 1000000 / NUMLEDS / 2);
  }
  FastLED.clear();
  // Serial.println("l_blinler");
}

void r_blinler() {
  for (int i = 0; i < NUMLEDS; i++) {
    argb[i] = CRGB::OrangeRed;
    FastLED.show();
    delayMicroseconds(blinker_time / 2 * 1000000 / NUMLEDS / 2);
  }
  for (int i = 0; i < NUMLEDS; i++) {
    argb[i] = CRGB::Black;
    FastLED.show();
    delayMicroseconds(blinker_time / 2 * 1000000 / NUMLEDS / 2);
  }
  FastLED.clear();
  FastLED.show();
  // Serial.println("r_blinler");
}

void hazard() {
  int center = 0;
  if (NUMLEDS % 2 == 0) center = NUMLEDS / 2 - 1;
  else int center = NUMLEDS / 2;
  for (int i_l = center, i_r = center + 1; i_l >= 0 && i_r < NUMLEDS; i_l--, i_r++) {
    argb[i_l] = CRGB::OrangeRed;
    argb[i_r] = CRGB::OrangeRed;
    FastLED.show();
    delayMicroseconds(blinker_time / 2 * 1000000 / NUMLEDS);
  }
  for (int i_l = center, i_r = center + 1; i_l >= 0 && i_r < NUMLEDS; i_l--, i_r++) {
    argb[i_l] = CRGB::Black;
    argb[i_r] = CRGB::Black;
    FastLED.show();
    delayMicroseconds(blinker_time / 2 * 1000000 / NUMLEDS);
  }
  FastLED.clear();
  FastLED.show();
  // Serial.println("r_blinler");
}

void warning() {
  for (int i = 0; i <= brightness; i++) {
    FastLED.setBrightness(i);
    FastLED.showColor(CRGB::Red);
    delayMicroseconds(blinker_time / 2 * 1000000 / brightness / 2);
  }
  for (int i = brightness; i >= 0; i--) {
    FastLED.setBrightness(i);
    FastLED.showColor(CRGB::Red);
    delayMicroseconds(blinker_time / 2 * 1000000 / brightness / 2);
  }
  FastLED.setBrightness(brightness);
  FastLED.clear();
  FastLED.show();
}

void lighting(byte R, byte G, byte B) {
  FastLED.showColor(CRGB(R, G, B));
}

void setup() {
  FastLED.addLeds< WS2812, LED_PIN, GRB>(argb, NUMLEDS);
  FastLED.clear();
  FastLED.show();
  FastLED.setBrightness(brightness);
  FastLED.setMaxPowerInVoltsAndMilliamps(5, 1500);
  Serial.begin(9600);
}
// Протокол: аварийка(0-1)л.поворотник(0-1)п.поворотник(0-1)опасность(0-1)R(000-255)G(000-255)B(000-255)
void loop() {
  if (Serial.available() > 0) {
    byte input[13] = { 0 };
    for (int i = 0; i < 13; i++) {
      if (Serial.peek() != -1) input[i] = Serial.read() - '0';
      delay(5);
    }
    byte R = input[4] * 100 + input[5] * 10 + input[6];
    byte G = input[7] * 100 + input[8] * 10 + input[9];
    byte B = input[10] * 100 + input[11] * 10 + input[12];
    if (input[0] == 1) {
      hazard();
    } else if (input[1] == 1) {
      l_blinler();
    } else if (input[2] == 1) {
      r_blinler();
    } else if (input[3] == 1) {
      warning();
    } else {
      lighting(R, G, B);
    }
    for (int i = 0; i < 4; i++) {
      input[i] = 0;
    }
    while (Serial.available() > 0) Serial.read();
  }
}
// l_blinler();
// r_blinler();
// hazard();
// warning();
