import 'dart:convert';
import 'package:android_app/model/pc_user.dart';
import 'package:android_app/service/auth_service.dart';
import 'package:http/http.dart' as http;
import 'package:android_app/model/response.dart';

class UserService {
  final String baseUrl = "https://192.168.0.109:8001/api/pcuser";

  Future<AppResponse> getUsers() async {
    final response = await http.get(Uri.parse('$baseUrl/users'), headers: {
      'authorization': 'bearer ${AuthService.getInstance().token}'
    });
    if (response.statusCode == 200) {
      Iterable jsonResponse = json.decode(response.body);
      return AppResponse(
          statusCode: 200,
          data: jsonResponse.map((model) => PCUser.fromJson(model)).toList());
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      throw Exception(
          'Failed to load users: ${response.statusCode}\n${AuthService.getInstance().token}');
    }
  }

  Future<AppResponse> addPCToUser(PCUserRequest pcuser) async {
    final response = await http.post(
      Uri.parse(baseUrl),
      headers: {
        'Content-Type': 'application/json',
        'authorization': 'bearer ${AuthService.getInstance().token}'
      },
      body: json.encode(pcuser.toJson()),
    );
    if (response.statusCode == 200) {
      return AppResponse(
          statusCode: 200,
          data: PCUserResponse.fromJson(json.decode(response.body)));
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      throw Exception(
          'Failed to add PC: ${response.statusCode}\n${AuthService.getInstance().token}');
    }
  }

  Future<AppResponse> removePCFromUser(int id) async {
    final response = await http.delete(Uri.parse('$baseUrl/$id'), headers: {
      'authorization': 'bearer ${AuthService.getInstance().token}'
    });
    if (response.statusCode == 200) {
      return AppResponse(
          statusCode: 200,
          data: PCUserResponse.fromJson(json.decode(response.body)));
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      throw Exception(
          'Failed to remove PC: ${response.statusCode}\n${AuthService.getInstance().token}');
    }
  }
}
