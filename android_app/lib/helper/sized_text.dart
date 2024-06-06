import 'package:flutter/material.dart';

Text sizedText(String text, double size) {
  return Text(
    text,
    style: TextStyle(fontSize: size),
  );
}

Text coloredText(String text, double size, Color color) {
  return Text(
    text,
    style: TextStyle(fontSize: size, color: color),
  );
}
