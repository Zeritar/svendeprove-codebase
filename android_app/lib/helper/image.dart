import 'package:flutter/material.dart';

ElevatedButton imageButton(String image, Color color, Function() function) {
  return ElevatedButton(
    style: ElevatedButton.styleFrom(
        elevation: 0.0,
        backgroundColor: Colors.transparent,
        tapTargetSize: MaterialTapTargetSize.shrinkWrap,
        minimumSize: const Size(50, 32),
        padding: const EdgeInsets.all(0)),
    onPressed: function,
    child: ColorFiltered(
      colorFilter: ColorFilter.mode(
        color,
        BlendMode.srcIn,
      ),
      child: Image.asset(image),
    ),
  );
}

ColorFiltered coloredImage(String image, Color color) {
  return ColorFiltered(
    colorFilter: ColorFilter.mode(
      color,
      BlendMode.srcIn,
    ),
    child: Image.asset(image),
  );
}
