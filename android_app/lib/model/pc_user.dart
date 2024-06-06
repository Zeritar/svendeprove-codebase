import 'package:android_app/model/pc.dart';

class PCSimple {
  final int id;
  final String name;
  final bool isOnline;
  final int entryId;

  PCSimple(
      {required this.id,
      required this.name,
      required this.isOnline,
      required this.entryId});

  factory PCSimple.fromJson(Map<String, dynamic> json) {
    return PCSimple(
      id: json['id'],
      name: json['name'],
      isOnline: json['isOnline'],
      entryId: json['entryID'],
    );
  }

  factory PCSimple.fromPC(PC pc) {
    return PCSimple(
      id: pc.id,
      name: pc.name,
      isOnline: pc.isOnline,
      entryId: 0,
    );
  }

  factory PCSimple.fromPCAndPCResponse(PC pc, PCUserResponse pcUserResponse) {
    return PCSimple(
      id: pc.id,
      name: pc.name,
      isOnline: pc.isOnline,
      entryId: pcUserResponse.id,
    );
  }
}

class PCUser {
  final String username;
  final List<PCSimple> pcs;

  PCUser({required this.username, required this.pcs});

  factory PCUser.fromJson(Map<String, dynamic> json) {
    return PCUser(
      username: json['username'],
      pcs: List<PCSimple>.from(
          json['pCs'].map((model) => PCSimple.fromJson(model))),
    );
  }
}

class PCUserRequest {
  final int pcId;
  final String username;

  PCUserRequest({required this.pcId, required this.username});

  Map<String, dynamic> toJson() {
    return {
      'pcId': pcId,
      'username': username,
    };
  }
}

class PCUserResponse {
  final int id;
  final int pcId;

  PCUserResponse({required this.id, required this.pcId});

  factory PCUserResponse.fromJson(Map<String, dynamic> json) {
    return PCUserResponse(
      id: json['id'],
      pcId: json['pcId'],
    );
  }
}
