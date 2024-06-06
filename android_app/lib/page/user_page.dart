import 'package:android_app/helper/sized_text.dart';
import 'package:android_app/model/pc_user.dart';
import 'package:android_app/model/response.dart';
import 'package:android_app/page/user_edit_page.dart';
import 'package:android_app/service/auth_service.dart';
import 'package:android_app/service/user_service.dart';
import 'package:flutter/material.dart';

class UserPage extends StatefulWidget {
  UserPage({super.key, required this.title});

  static const String route = '/users';

  final String title;

  final UserService userService = UserService();

  @override
  State<UserPage> createState() => _UserPageState();
}

class _UserPageState extends State<UserPage> {
  PCUser? editedUser;
  bool hasChanged = false;

  String errorMessage = '';
  bool logout = false;

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
                ? buildUserListTiles()
                : sizedText(errorMessage, 30),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          refresh();
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

  void refresh() {
    setState(() {});
  }

  Future<List<ListTile>> getUserListTiles() async {
    try {
      final AppResponse response = await widget.userService.getUsers();
      if (response.statusCode == 200) {
        return (response.data as List<PCUser>).map((user) {
          return ListTile(
            title: Text(user.username),
            subtitle: buildUserPCTiles(user.pcs),
            trailing: ElevatedButton(
              onPressed: () {
                awaitEditUserPage(context, user);
              },
              child: const Text('Rediger'),
            ),
            shape: Border.all(),
          );
        }).toList();
      } else if (response.statusCode > 400 && response.statusCode < 404) {
        logout = true;
        return [];
      } else {
        throw Exception('Fejl ved hentning af brugere: ${response.statusCode}');
      }
    } catch (e) {
      setState(() {
        errorMessage = e.toString();
      });
      return [];
    }
  }

  void awaitEditUserPage(BuildContext context, PCUser user) async {
    await Navigator.pushNamed(context, UserEditPage.route, arguments: user);

    setState(() {});
  }

  ListView buildUserPCTiles(List<PCSimple> pcs) {
    List<ListTile> tiles = [];
    if (pcs.isEmpty) {
      tiles.add(const ListTile(
        title: Text('Ingen PCer tildelt'),
      ));
    } else {
      tiles = pcs.map((pc) {
        return ListTile(
          title: Text(pc.name),
          subtitle: coloredText(pc.isOnline ? 'Online' : 'Offline', 12,
              pc.isOnline ? Colors.green : Colors.red),
        );
      }).toList();
    }

    return ListView(
      scrollDirection: Axis.vertical,
      shrinkWrap: true,
      children: tiles,
    );
  }

  FutureBuilder<List<ListTile>> buildUserListTiles() {
    FutureBuilder<List<ListTile>> ft = FutureBuilder<List<ListTile>>(
      future: getUserListTiles(),
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
}
