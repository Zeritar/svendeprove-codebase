import 'dart:math';

import 'package:android_app/helper/sized_text.dart';
import 'package:android_app/model/response.dart';
import 'package:android_app/page/admin_page.dart';
import 'package:android_app/page/login_page.dart';
import 'package:android_app/page/pc_page.dart';
import 'package:android_app/page/register_page.dart';
import 'package:android_app/service/auth_service.dart';
import 'package:flutter/material.dart';

class AuthPage extends StatefulWidget {
  const AuthPage({super.key, required this.title});

  static const String route = '/';

  final String title;

  @override
  State<AuthPage> createState() => _AuthPageState();
}

class _AuthPageState extends State<AuthPage> {
  String errorMessage = '';

  bool tryAuthenticate = false;

  @override
  void initState() {
    super.initState();
    tryAuthenticate = true;
  }

  void authenticate() async {
    AuthService.getInstance().loadToken().then((value) async {
      if (value != null) {
        bool isAdmin = false;
        final payload = value.payload;
        if (payload[JWTType.role] is String &&
            payload[JWTType.role] == 'admin') {
          isAdmin = true;
        }
        if (payload[JWTType.role] is List) {
          final roles = payload[JWTType.role] as List;
          if (roles.contains('admin')) {
            isAdmin = true;
          }
        }
        if (isAdmin) {
          await Navigator.pushNamed(context, AdminPage.route);
        } else {
          await Navigator.pushNamed(context, PCPage.route);
        }
        AuthService.getInstance().logout();
        setState(() {
          tryAuthenticate = false;
        });
      } else {
        setState(() {
          tryAuthenticate = false;
        });
      }
    }).catchError((e) {
      setState(() {
        errorMessage = e.toString();
        tryAuthenticate = false;
      });
    });
  }

  @override
  Widget build(BuildContext context) {
    if (tryAuthenticate) {
      authenticate();
      return Scaffold(
        appBar: AppBar(
          backgroundColor: Theme.of(context).colorScheme.inversePrimary,
          title: Text(widget.title),
        ),
        body: const Center(
          child: CircularProgressIndicator(),
        ),
      );
    }

    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Text(widget.title),
      ),
      body: Column(mainAxisAlignment: MainAxisAlignment.center, children: [
        Center(
          child: SizedBox(
            width: 220,
            height: 80,
            child: ElevatedButton(
              onPressed: () async {
                await Navigator.pushNamed(context, LoginPage.route);
                setState(() {
                  tryAuthenticate = true;
                });
              },
              child: sizedText('Log ind', 30),
            ),
          ),
        ),
        const SizedBox(height: 20),
        Center(
          child: SizedBox(
            width: 220,
            height: 80,
            child: ElevatedButton(
              onPressed: () async {
                await Navigator.pushNamed(context, RegisterPage.route);
                setState(() {
                  tryAuthenticate = true;
                });
              },
              child: sizedText('Registrer', 30),
            ),
          ),
        ),
      ]),
    );
  }
}
