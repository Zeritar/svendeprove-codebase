import 'package:android_app/helper/sized_text.dart';
import 'package:android_app/service/auth_service.dart';
import 'package:flutter/material.dart';

class LoginPage extends StatefulWidget {
  LoginPage({super.key, required this.title});

  static const String route = '/login';

  final String title;

  final AuthService authService = AuthService();

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final TextEditingController _usernameController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();

  String errorMessage = '';

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Text(widget.title),
      ),
      body: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          SizedBox(
            width: 400,
            height: 100,
            child: Center(
              child: TextField(
                controller: _usernameController,
                decoration: const InputDecoration(
                    labelText: 'Brugernavn',
                    labelStyle: TextStyle(fontSize: 20.0),
                    border: OutlineInputBorder()),
                style: TextStyle(
                  fontSize: 24.0,
                  color: Theme.of(context).colorScheme.primary,
                ),
              ),
            ),
          ),
          SizedBox(
            width: 400,
            height: 100,
            child: Center(
              child: TextField(
                controller: _passwordController,
                decoration: const InputDecoration(
                    labelText: 'Password',
                    labelStyle: TextStyle(fontSize: 20.0),
                    border: OutlineInputBorder()),
                style: TextStyle(
                  fontSize: 24.0,
                  color: Theme.of(context).colorScheme.primary,
                ),
                obscureText: true,
              ),
            ),
          ),
          const SizedBox(height: 20),
          Center(
            child: SizedBox(
              width: 180,
              height: 60,
              child: ElevatedButton(
                onPressed: () {
                  widget.authService
                      .login(_usernameController.text, _passwordController.text)
                      .then((value) {
                    Navigator.pop(context);
                  }).catchError((e) {
                    setState(() {
                      errorMessage = 'Ingen forbindelse til server.';
                    });
                  });
                },
                child: sizedText('Log ind', 24),
              ),
            ),
          ),
          (errorMessage == '')
              ? const SizedBox()
              : Center(
                  child: sizedText(errorMessage, 30),
                ),
        ],
      ),
    );
  }
}
