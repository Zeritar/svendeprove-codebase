import 'dart:convert';
import 'package:android_app/model/response.dart';
import 'package:http/http.dart' as http;
import 'package:dart_jsonwebtoken/dart_jsonwebtoken.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class AuthService {
  final String loginUrl = "https://192.168.0.109:8001/api/auth/login";
  final String registerUrl = "https://192.168.0.109:8001/api/auth/register";
  final String confirmUrl = "https://192.168.0.109:8001/api/auth/confirm-email";
  String token = "";
  DateTime expiry = DateTime.now();

  final storage = const FlutterSecureStorage();

  static AuthService? instance;

  AuthService() {
    instance = this;
  }

  static AuthService getInstance() {
    instance ??= AuthService();
    return instance!;
  }

  Future<JWT> login(String username, String password) async {
    final response = await http.post(
      Uri.parse(loginUrl),
      headers: {'Content-Type': 'application/json'},
      body: json.encode({'Username': username, 'Password': password}),
    );
    if (response.statusCode == 200) {
      dynamic jobj = json.decode(response.body);
      saveToken(jobj);

      JWT jwt = JWT.decode(jobj['token']);
      return jwt;
    } else {
      throw Exception('Failed to login: ${response.statusCode}');
    }
  }

  Future<AppResponse> register(
      String username, String email, String password) async {
    final response = await http.post(
      Uri.parse(registerUrl),
      headers: {'Content-Type': 'application/json'},
      body: json
          .encode({'Username': username, 'Email': email, 'Password': password}),
    );
    dynamic jobj = json.decode(response.body);
    AppResponse res = AppResponse(
        statusCode: response.statusCode, message: jobj['message'] ?? '');
    return res;
  }

  Future<AppResponse> confirm(String username) async {
    final response = await http.post(Uri.parse(confirmUrl),
        headers: {'Content-Type': 'application/json'}, body: '"$username"');
    dynamic jobj = json.decode(response.body);
    AppResponse res = AppResponse(
        statusCode: response.statusCode, message: jobj['message'] ?? '');
    return res;
  }

  void logout() {
    _deleteToken();
  }

  void saveToken(dynamic jobj) {
    token = jobj['token'];
    expiry = DateTime.parse(jobj['expiration']);

    storage.write(key: 'jwt', value: token);
    storage.write(key: 'expires', value: expiry.toIso8601String());
  }

  Future<JWT?> loadToken() async {
    String? keyRead = await storage.read(key: 'jwt');
    String? expiresRead = await storage.read(key: 'expires');

    if (keyRead == null ||
        expiresRead == null ||
        keyRead == '' ||
        expiresRead == '') {
      _deleteToken();
      return null;
    }

    token = keyRead;
    expiry = DateTime.parse(expiresRead);

    if (DateTime.now().isAfter(expiry)) {
      _deleteToken();
      return null;
    }

    return JWT.decode(token);
  }

  void _deleteToken() {
    token = '';
    expiry = DateTime.now();
    storage.deleteAll();
  }
}
