import 'package:android_app/model/response.dart';
import 'package:android_app/service/auth_service.dart';
import 'package:flutter/material.dart';
import 'package:android_app/helper/sized_text.dart';
import 'package:android_app/helper/image.dart';
import 'package:android_app/model/pc.dart';
import 'package:android_app/service/pc_service.dart';
import 'package:web_socket_channel/web_socket_channel.dart';

class PCPage extends StatefulWidget {
  PCPage({super.key, required this.title});

  static const String route = '/pcs';

  final String title;

  final PCService pcService = PCService();

  @override
  State<PCPage> createState() => _PCPageState();
}

class _PCPageState extends State<PCPage> {
  String errorMessage = '';
  bool logout = false;

  final ValueNotifier<int> _notifier = ValueNotifier(1);

  @override
  Widget build(BuildContext context) {
    if (logout) {
      AuthService.getInstance().logout();
      Navigator.pop(context);
    }

    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Text(widget.title),
        automaticallyImplyLeading: false,
        actions: [
          ElevatedButton(
            onPressed: () {
              Navigator.pop(context);
            },
            child: sizedText('Log ud', 20),
          ),
        ],
      ),
      body: Column(
        children: [
          Center(
            child: errorMessage == ''
                ? buildPCsListTiles()
                : sizedText(errorMessage, 30),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          _refresh();
        },
        backgroundColor: Colors.transparent,
        child: ClipOval(
          clipBehavior: Clip.antiAlias,
          child: ColorFiltered(
            colorFilter: const ColorFilter.mode(
              Colors.green,
              BlendMode.screen,
            ),
            child: Image.asset('assets/refresh.png'),
          ),
        ),
      ),
    );
  }

  void _refresh() {
    setState(() {});
  }

  Future<List<PCListTile>> getPCsListTiles() async {
    try {
      // final List<PC> pcs = await widget.pcService.getPCsForUser();
      final AppResponse response = await widget.pcService.getPCsForUser();
      if (response.statusCode == 200) {
        return (response.data as List<PC>).map((pc) {
          return PCListTile(pc: pc, pcService: widget.pcService);
        }).toList();
      } else if (response.statusCode > 400 && response.statusCode < 404) {
        logout = true;
        return [];
      } else {
        throw Exception('Fejl ved hentning af PCer: ${response.statusCode}');
      }
    } catch (e) {
      setState(() {
        errorMessage = e.toString();
      });
      return [];
    }
  }

  FutureBuilder<List<PCListTile>> buildPCsListTiles() {
    FutureBuilder<List<PCListTile>> ft = FutureBuilder<List<PCListTile>>(
      future: getPCsListTiles(),
      builder: (
        context,
        snapshot,
      ) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(
            child: Column(children: [
              SizedBox(
                height: 20,
              ),
              CircularProgressIndicator()
            ]),
          );
        } else if (snapshot.hasError) {
          return Text('Fejl: ${snapshot.error}');
        } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
          return const Text('Ingen data tilgængelig.');
        } else {
          return ListView(
              scrollDirection: Axis.vertical,
              shrinkWrap: true,
              children: snapshot.data!);
        }
      },
    );
    return ft;
  }

  @override
  void dispose() {
    _notifier.dispose();
    super.dispose();
  }
}

class PCListTile extends StatefulWidget {
  const PCListTile({super.key, required this.pc, required this.pcService});

  final PC pc;

  final PCService pcService;

  @override
  State<PCListTile> createState() => _PCListTileState();
}

class _PCListTileState extends State<PCListTile> {
  bool _isLoading = false;

  Tween<double> tween = Tween<double>(
    begin: -0.7,
    end: 1.7,
  );

  bool reset = false;
  int mills = 3000;
  double begin = -0.7;
  double end = 1.7;

  Duration duration = const Duration(milliseconds: 3000);

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return Stack(
        children: [
          ListTile(
              leading: sizedText(widget.pc.id.toString(), 20),
              title: sizedText(widget.pc.name, 24),
              subtitle: sizedText('Afventer svar...', 18),
              shape: Border.all()),
          TweenAnimationBuilder<double>(
            onEnd: () {
              setState(() {
                if (reset) {
                  begin = -0.7;
                  end = 1.7;
                  mills = 3000;
                  reset = false;
                } else {
                  begin = 1.7;
                  end = -0.7;
                  mills = 1;
                  reset = true;
                }

                tween = Tween<double>(
                  begin: begin,
                  end: end,
                );
                duration = Duration(milliseconds: mills);
              });
            },
            duration: duration,
            tween: tween,
            builder: (context, value, _) => ShaderMask(
              shaderCallback: (Rect bounds) {
                return LinearGradient(
                  colors: const [
                    Color.fromARGB(0, 50, 255, 50),
                    Color.fromARGB(150, 50, 255, 50),
                    Color.fromARGB(255, 50, 255, 50),
                    Color.fromARGB(150, 50, 255, 50),
                    Color.fromARGB(0, 50, 255, 50),
                  ],
                  stops: [
                    value - 0.6,
                    value - 0.3,
                    value,
                    value + 0.3,
                    value + 0.6
                  ],
                  begin: Alignment.centerLeft,
                  end: Alignment.centerRight,
                  tileMode: TileMode.repeated,
                ).createShader(bounds);
              },
              child: const LinearProgressIndicator(
                minHeight: 80,
                value: 1,
                backgroundColor: Colors.transparent,
                color: Color.fromARGB(100, 50, 255, 50),
              ),
            ),
          )
        ],
      );
    }
    return ListTile(
        leading: sizedText(widget.pc.id.toString(), 20),
        title: sizedText(widget.pc.name, 24),
        subtitle: coloredText(widget.pc.isOnline ? 'Online' : 'Offline', 18,
            widget.pc.isOnline ? Colors.green : Colors.red),
        trailing: rowBuilder(widget.pc),
        shape: Border.all()
        // You can customize ListTile properties as needed.
        );
  }

  SizedBox rowBuilder(PC pc) {
    List<Widget> children = List.empty(growable: true);

    if (pc.isOnline) {
      children.add(imageButton('assets/power.png', Colors.green, () {
        _awaitPCStop(context, pc);
      }));
    } else {
      children.add(imageButton('assets/power.png', Colors.red, () {
        _awaitPCStart(context, pc);
      }));
    }

    return SizedBox(
      height: 32,
      width: 250,
      child: Row(
        mainAxisAlignment: MainAxisAlignment.end,
        mainAxisSize: MainAxisSize.min,
        children: children,
      ),
    );
  }

  void _awaitPCStart(BuildContext context, PC pc) async {
    setState(() {
      _isLoading = true;
    });
    final result = await widget.pcService.startPC(pc);

    if (result.statusCode == 200 && result.data == false) {
      setState(() {
        pc.isOnline = true;
        _isLoading = false;
      });

      return;
    }

    WebSocketChannel channel = WebSocketChannel.connect(
      Uri.parse('wss://192.168.0.109:8001/ws/${pc.id}/start'),
    );

    channel.stream.listen((data) {
      if (data == '{"Id":${pc.id},"Message":"start","status":true}') {
        setState(() {
          pc.isOnline = true;
          _isLoading = false;
        });
        channel.sink.close();
      } else if (data == '{"Id":${pc.id},"Message":"start","status":false}') {
        setState(() {
          pc.isOnline = false;
          _isLoading = false;
        });
        channel.sink.close();
      }
    });
  }

  void _awaitPCStop(BuildContext context, PC pc) async {
    setState(() {
      _isLoading = true;
    });
    final result = await widget.pcService.shutdownPC(pc);

    if (result.statusCode == 200 && result.data == false) {
      setState(() {
        pc.isOnline = true;
        _isLoading = false;
      });
      return;
    }

    WebSocketChannel channel = WebSocketChannel.connect(
      Uri.parse('wss://192.168.0.109:8001/ws/${pc.id}/shutdown'),
    );

    channel.stream.listen((data) {
      if (data == '{"Id":${pc.id},"Message":"shutdown","status":true}') {
        setState(() {
          pc.isOnline = false;
          _isLoading = false;
        });
        channel.sink.close();
      } else if (data ==
          '{"Id":${pc.id},"Message":"shutdown","status":false}') {
        setState(() {
          pc.isOnline = true;
          _isLoading = false;
        });
        channel.sink.close();
      }
    });
  }
}
