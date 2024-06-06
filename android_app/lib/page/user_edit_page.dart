import 'package:android_app/helper/sized_text.dart';
import 'package:android_app/model/pc.dart';
import 'package:android_app/model/pc_user.dart';
import 'package:android_app/model/response.dart';
import 'package:android_app/service/pc_service.dart';
import 'package:android_app/service/user_service.dart';
import 'package:flutter/material.dart';

class UserEditPage extends StatefulWidget {
  UserEditPage({super.key, required this.title, required this.user});
  static const String route = '/edituser';

  final String title;

  final PCUser user;

  final UserService userService = UserService();

  final PCService pcService = PCService();

  @override
  State<UserEditPage> createState() => _UserEditPageState();
}

class _UserEditPageState extends State<UserEditPage> {
  late String username;
  late List<PC> allPCs = List.empty();
  late List<PC> pcs = List.empty();

  @override
  void initState() {
    super.initState();

    username = widget.user.username;

    widget.pcService.getPCs().then((e) => setState(() {
          if (e.statusCode == 200) {
            allPCs = (e.data as List<PC>).map<PC>((pc) => pc).toList();
            pcs = allPCs
                .where((element) =>
                    !(widget.user.pcs.any((e) => element.id == e.id)))
                .toList();
          }
        }));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.title),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: ListView(
        padding: const EdgeInsets.all(16.0),
        children: <Widget>[
          const SizedBox(height: 18.0),
          sizedText('Bruger', 18),
          sizedText(widget.user.username, 24),
          const SizedBox(height: 24.0),
          Center(
            child: buildUserPCTiles(widget.user.pcs),
          ),
          const SizedBox(height: 24.0),
          Center(
            child: buildPCTiles(),
          ),
        ],
      ),
    );
  }

  ListView buildUserPCTiles(List<PCSimple> pcs) {
    List<ListTile> tiles = [];
    if (pcs.isEmpty) {
      tiles.add(const ListTile(
        title: Text('Ingen PCer tildelt'),
      ));
    } else {
      tiles.add(ListTile(
        title: sizedText('Tildelte PCer:', 18),
      ));
      tiles += pcs.map((pc) {
        return ListTile(
          title: sizedText(pc.name, 20),
          trailing: ElevatedButton(
            onPressed: () async {
              try {
                final AppResponse reponse =
                    await widget.userService.removePCFromUser(pc.entryId);
                if (reponse.statusCode == 200 && reponse.data != null) {
                  widget.user.pcs.remove(pc);
                  setState(() {
                    this.pcs.add(
                        allPCs.firstWhere((element) => element.id == pc.id));
                  });
                } else {
                  setState(() {});
                }
              } catch (e) {
                //
              }
            },
            child: const Text('Fjern'),
          ),
          shape: Border.all(),
        );
      }).toList();
    }

    return ListView(
      scrollDirection: Axis.vertical,
      shrinkWrap: true,
      children: tiles,
    );
  }

  ListView buildPCTiles() {
    List<ListTile> tiles = [];
    if (pcs.isEmpty) {
      tiles.add(const ListTile(
        title: Text('Ingen tilgængelige PCer'),
      ));
    } else {
      tiles.add(ListTile(
        title: sizedText('Tilgængelige PCer:', 18),
      ));
      tiles += pcs.map((pc) {
        return ListTile(
          title: sizedText(pc.name, 20),
          trailing: ElevatedButton(
            onPressed: () async {
              try {
                final AppResponse reponse = await widget.userService
                    .addPCToUser(PCUserRequest(
                        pcId: pc.id, username: widget.user.username));

                if (reponse.statusCode == 200 && reponse.data != null) {
                  widget.user.pcs
                      .add(PCSimple.fromPCAndPCResponse(pc, reponse.data));
                  setState(() {
                    pcs.remove(pc);
                  });
                } else {
                  setState(() {});
                }
              } catch (e) {
                //
              }
            },
            child: const Text('Tildel'),
          ),
          shape: Border.all(),
        );
      }).toList();
    }

    return ListView(
      scrollDirection: Axis.vertical,
      shrinkWrap: true,
      children: tiles,
    );
  }
}
