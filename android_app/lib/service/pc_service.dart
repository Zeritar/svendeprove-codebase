import 'dart:convert';
import 'package:android_app/service/auth_service.dart';
import 'package:http/http.dart' as http;
import 'package:android_app/model/pc.dart';
import 'package:android_app/model/response.dart';

class PCService {
  final String baseUrl = "https://192.168.0.109:8001/api/PC";

  Future<AppResponse> getPCs() async {
    final response = await http.get(Uri.parse('$baseUrl/getAll'), headers: {
      'authorization': 'bearer ${AuthService.getInstance().token}'
    });
    if (response.statusCode == 200) {
      Iterable jsonResponse = json.decode(response.body);
      return AppResponse(
          statusCode: 200,
          data: jsonResponse.map((model) => PC.fromJson(model)).toList());
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      throw Exception(
          'Failed to load PCs: ${response.statusCode}\n${AuthService.getInstance().token}');
    }
  }

  Future<AppResponse> getPCsForUser() async {
    final response = await http.get(Uri.parse('$baseUrl/getAllForUser'),
        headers: {
          'authorization': 'bearer ${AuthService.getInstance().token}'
        });
    if (response.statusCode == 200) {
      Iterable jsonResponse = json.decode(response.body);
      return AppResponse(
          statusCode: 200,
          data: jsonResponse.map((model) => PC.fromJson(model)).toList());
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      throw Exception(
          'Failed to load PCs: ${response.statusCode}\n${AuthService.getInstance().token}');
    }
  }

  Future<AppResponse> getPC(int id) async {
    final response = await http.get(Uri.parse('$baseUrl/PC/$id'), headers: {
      'authorization': 'bearer ${AuthService.getInstance().token}'
    });
    if (response.statusCode == 200) {
      return AppResponse(
          statusCode: 200, data: PC.fromJson(json.decode(response.body)));
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      throw Exception('Failed to load PC');
    }
  }

  Future<AppResponse> postPC(PC p) async {
    final response = await http.post(
      Uri.parse('$baseUrl/PC'),
      headers: {
        'Content-Type': 'application/json',
        'authorization': 'bearer ${AuthService.getInstance().token}'
      },
      body: json.encode(p.toJson()),
    );
    if (response.statusCode == 200) {
      return AppResponse(
          statusCode: 200, data: PC.fromJson(json.decode(response.body)));
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      throw Exception('Failed to create PC');
    }
  }

  Future<AppResponse> startPC(PC p) async {
    final response = await http.get(Uri.parse('$baseUrl/start/${p.id}'),
        headers: {
          'authorization': 'bearer ${AuthService.getInstance().token}'
        });
    if (response.statusCode == 200) {
      return AppResponse(statusCode: 200, data: response.body == 'true');
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      return AppResponse(statusCode: 400, message: 'Failed to start PC');
    }
  }

  Future<AppResponse> shutdownPC(PC p) async {
    final response = await http.get(Uri.parse('$baseUrl/shutdown/${p.id}'),
        headers: {
          'authorization': 'bearer ${AuthService.getInstance().token}'
        });
    if (response.statusCode == 200) {
      return AppResponse(statusCode: 200, data: response.body == 'true');
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      return AppResponse(statusCode: 400, message: 'Failed to shutdown PC');
    }
  }

  Future<AppResponse> putPC(PC p) async {
    final response = await http.put(
      Uri.parse('$baseUrl/PC/${p.id}'),
      headers: {
        'Content-Type': 'application/json',
        'authorization': 'bearer ${AuthService.getInstance().token}'
      },
      body: json.encode(p.toJson()),
    );
    if (response.statusCode == 200) {
      return AppResponse(
          statusCode: 200, data: PC.fromJson(json.decode(response.body)));
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      throw Exception('Failed to update PC');
    }
  }

  Future<AppResponse> deletePC(int id) async {
    final response = await http.delete(Uri.parse('$baseUrl/PC/$id'), headers: {
      'authorization': 'bearer ${AuthService.getInstance().token}'
    });
    if (response.statusCode == 200) {
      return AppResponse(
          statusCode: 200, data: PC.fromJson(json.decode(response.body)));
    } else if (response.statusCode > 400 && response.statusCode < 404) {
      AuthService.getInstance().logout();
      return AppResponse(
          statusCode: response.statusCode, message: 'Unauthorized');
    } else {
      throw Exception('Failed to delete PC');
    }
  }
}
