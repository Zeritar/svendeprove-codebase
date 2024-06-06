class PC {
  int id;
  String name;
  bool isNetworked;
  int ipAddress;
  List<int> macAddress;
  List<int> pins;
  bool isOnline;

  PC({
    this.id = 0,
    this.name = "",
    this.isNetworked = false,
    this.ipAddress = 0,
    this.macAddress = const [0, 0, 0, 0, 0, 0],
    this.pins = const [0, 0],
    this.isOnline = false,
  });

  @override
  String toString() {
    if (isNetworked) {
      return 'PC{id: $id, name: $name, online: $isOnline, ip: $ipAddress, mac: $macAddress}';
    } else {
      return 'PC{id: $id, name: $name, online: $isOnline, pins: $pins}';
    }
  }

  factory PC.fromJson(Map<String, dynamic> json) {
    return PC(
      id: json['id'],
      name: json['name'],
      isOnline: json['isOnline'],
      ipAddress: json['ipAddress'],
      macAddress: List<int>.from(json['macAddress']),
      pins: List<int>.from(json['pins']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'isOnline': isOnline,
      'ipAddress': ipAddress,
      'macAddress': macAddress,
      'pins': pins,
    };
  }
}
