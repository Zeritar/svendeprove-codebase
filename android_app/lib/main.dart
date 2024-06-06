import 'package:android_app/model/pc_user.dart';
import 'package:android_app/page/admin_page.dart';
import 'package:android_app/page/auth_page.dart';
import 'package:android_app/page/login_page.dart';
import 'package:android_app/page/register_page.dart';
import 'package:android_app/page/user_edit_page.dart';
import 'package:android_app/page/user_page.dart';
import 'package:flutter/material.dart';
import 'package:android_app/page/pc_page.dart';
import 'dart:io';

///
/// FOR DEVELOPMENT ONLY
/// This bypasses SSL certificate verification to use self-signed certificate
///
class MyHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback =
          (X509Certificate cert, String host, int port) => true;
  }
}

void main() {
  HttpOverrides.global = MyHttpOverrides();
  runApp(const SystemOverseerApp());
}

class SystemOverseerApp extends StatelessWidget {
  const SystemOverseerApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
        title: 'System Overseer',
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
          useMaterial3: true,
        ),
        initialRoute: '/',
        routes: {
          AuthPage.route: (context) => const AuthPage(title: 'System Overseer'),
          PCPage.route: (context) => PCPage(title: 'PCer'),
          LoginPage.route: (context) => LoginPage(title: 'Login'),
          RegisterPage.route: (context) => RegisterPage(title: 'Registrer'),
          AdminPage.route: (context) => const AdminPage(title: 'Admin Login'),
          UserPage.route: (context) => UserPage(title: 'Brugere'),
        },
        onGenerateRoute: (settings) {
          if (settings.name == UserEditPage.route) {
            final user = settings.arguments as PCUser;
            return MaterialPageRoute(
              builder: (context) =>
                  UserEditPage(title: 'Rediger Bruger', user: user),
            );
          }
          // Handle other routes
          return null;
        });
  }
}
