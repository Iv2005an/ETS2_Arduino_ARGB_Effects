#include "FastLED.h"

#define LED_PIN 2    // пин ленты
#define NUMLEDS 102  // количество светодиодов

CRGB argb[NUMLEDS];
byte brightness = 100;
void l_blinler() {
  for (int i = NUMLEDS; i >= 0; i--) {
    argb[i] = CRGB::OrangeRed;
    FastLED.show();
    delay(5);
  }
  for (int i = NUMLEDS; i >= 0; i--) {
    argb[i] = CRGB::Black;
    FastLED.show();
    delay(5);
  }
  FastLED.clear();
  // Serial.println("l_blinler");
}

void r_blinler() {
  for (int i = 0; i < NUMLEDS; i++) {
    argb[i] = CRGB::OrangeRed;
    FastLED.show();
    delay(5);
  }
  for (int i = 0; i < NUMLEDS; i++) {
    argb[i] = CRGB::Black;
    FastLED.show();
    delay(5);
  }
  FastLED.clear();
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
    delay(10);
  }
  for (int i_l = center, i_r = center + 1; i_l >= 0 && i_r < NUMLEDS; i_l--, i_r++) {
    argb[i_l] = CRGB::Black;
    argb[i_r] = CRGB::Black;
    FastLED.show();
    delay(10);
  }
  FastLED.clear();
  // Serial.println("r_blinler");
}

void warning() {
  for (int i = 0; i <= brightness; i++) {
    FastLED.setBrightness(i);
    FastLED.showColor(CRGB::Red);
    delay(10);
  }
  for (int i = NUMLEDS; i >= 0; i--) {
    FastLED.setBrightness(i);
    FastLED.showColor(CRGB::Red);
    delay(10);
  }
  FastLED.setBrightness(brightness);
  FastLED.clear();
}

void setup() {
  FastLED.addLeds< WS2812, LED_PIN, GRB>(argb, NUMLEDS);
  FastLED.clear();
  FastLED.show();
  FastLED.setBrightness(brightness);
  FastLED.setMaxPowerInVoltsAndMilliamps(5, 1500);
  Serial.begin(115200);
}

void loop() {
  if (Serial.available() > 0) {
    char input[7] = { 0 };
    for (int i = 0; i < 7; i++) {
      input[i] = (char)Serial.read();
      delay(1);
    }
    // for (int i = 0; i < 7; i++) {
    //   Serial.print(input[i]);
    // }
    if (input[0] == '1') {
      for (int i = 0; i < 7; i++) input[i] = NULL;
      hazard();
    } else if (input[2] == '1') {
      for (int i = 0; i < 7; i++) input[i] = NULL;
      l_blinler();
    } else if (input[4] == '1') {
      for (int i = 0; i < 7; i++) input[i] = NULL;
      r_blinler();
    } else if (input[6] == '1') {
      for (int i = 0; i < 7; i++) input[i] = NULL;
      warning();
    }
    // for (int i = 0; i < 7; i++) {
    //   Serial.print(input[i]);
    // }
  }
}
// l_blinler();
// r_blinler();
// hazard();
// warning();
