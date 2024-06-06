import 'package:android_app/helper/sized_text.dart';
import 'package:android_app/service/auth_service.dart';
import 'package:flutter/material.dart';

class RegisterPage extends StatefulWidget {
  RegisterPage({super.key, required this.title});

  static const String route = '/register';

  final String title;

  final AuthService authService = AuthService();

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final TextEditingController _usernameController = TextEditingController();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _repeatPasswordController =
      TextEditingController();

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
          Center(
            child: sizedText('Registrer bruger', 24),
          ),
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
                controller: _emailController,
                decoration: const InputDecoration(
                    labelText: 'Email',
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
          SizedBox(
            width: 400,
            height: 100,
            child: Center(
              child: TextField(
                controller: _repeatPasswordController,
                decoration: const InputDecoration(
                    labelText: 'Gentag Password',
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
                      .register(_usernameController.text, _emailController.text,
                          _passwordController.text)
                      .then((value) {
                    if (value.statusCode == 200) {
                      Navigator.pop(context);
                    } else {
                      setState(() {
                        errorMessage = value.message;
                      });
                    }
                  }).catchError((e) {
                    setState(() {
                      errorMessage = 'Ingen forbindelse til server.';
                    });
                  });
                },
                child: sizedText('Registrer', 24),
              ),
            ),
          ),
          const SizedBox(height: 20),
          Center(
            child: SizedBox(
              width: 220,
              height: 60,
              child: ElevatedButton(
                onPressed: () {
                  widget.authService
                      .confirm(_usernameController.text)
                      .then((value) {
                    if (value.statusCode == 200) {
                      Navigator.pop(context);
                    } else {
                      setState(() {
                        errorMessage = value.message;
                      });
                    }
                  }).catchError((e) {
                    setState(() {
                      errorMessage = e.toString();
                    });
                  });
                },
                child: sizedText('Bekræft Email', 24),
              ),
            ),
          ),
          Center(
              child: sizedText(
                  'Registrer bruger og vend tilbage til denne side for at bekræfte email.',
                  16)),
          (errorMessage == '')
              ? const SizedBox()
              : Center(
                  child: errorMessage == ''
                      ? sizedText('Registrer', 30)
                      : sizedText(errorMessage, 30),
                ),
        ],
      ),
    );
  }
}
