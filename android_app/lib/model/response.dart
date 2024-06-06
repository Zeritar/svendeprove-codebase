class AppResponse {
  final int statusCode;
  final String message;
  final dynamic data;

  AppResponse({required this.statusCode, this.message = '', this.data});
}

class JWTType {
  static const String role =
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
  static const String name =
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
}
