import 'package:android_app/helper/sized_text.dart';
import 'package:android_app/page/pc_page.dart';
import 'package:android_app/page/user_page.dart';
import 'package:flutter/material.dart';

class AdminPage extends StatefulWidget {
  const AdminPage({super.key, required this.title});

  static const String route = '/admin';

  final String title;

  @override
  State<AdminPage> createState() => _AdminPageState();
}

class _AdminPageState extends State<AdminPage> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.title),
      ),
      body: Column(mainAxisAlignment: MainAxisAlignment.center, children: [
        Center(
          child: SizedBox(
            width: 320,
            height: 80,
            child: ElevatedButton(
              onPressed: () async {
                await Navigator.pushNamed(context, UserPage.route);
              },
              child: sizedText('Log ind som admin', 30),
            ),
          ),
        ),
        const SizedBox(height: 20),
        Center(
          child: SizedBox(
            width: 320,
            height: 80,
            child: ElevatedButton(
              onPressed: () async {
                await Navigator.pushNamed(context, PCPage.route);
              },
              child: sizedText('Log ind som bruger', 30),
            ),
          ),
        ),
      ]),
    );
  }
}
