# MetaStreetArt

![](img/screenshot%20(1).png)
![](img/screenshot%20(2).png)
![](img/screenshot%20(3).png)
![](img/screenshot%20(4).png)
![](img/screenshot%20(5).png)
![](img/screenshot%20(6).png)

---
## Introduction

This project was made for a one-week workshop, including students from CNAM-ENJMIN, EESI Angoulême and EESI Poitiers.
It uses a template made by ENJMIN students for an online game, using Smartfoxserver and Unity.

See below for more informations on this template.

### Server setup instructions

- Open the src folder with IntelliJ, add from C:\Users\<User>\SmartFoxServer_2X\SFS2X\lib two files, sfs2x.jar and sfs2x-core.jar to IntelliJ libs (File > Project Structure > Module > Dependency), change Project > Language Level to 11
- Create the .jar following the instructions in the [Jetbrains documentation](https://www.jetbrains.com/help/idea/working-with-artifacts.html#examples)
Build > Build Artifacts
See also [smartfoxserver documentation](http://docs2x.smartfoxserver.com/)
- Add the .jar to C:\Users\<User>\SmartFoxServer_2X\SFS2X\extensions\CubeSpawnerRoomExtension (the name of the last folder must be the same as the Room Extension ID in Server Connection Data in Unity)
- Launch the server sfs2x-service.exe
- Go on [127.0.0.1:8080](http://127.0.0.1:8080), open administration tools, connect (default sfsadmin mdp:sfsadmin), go on ZoneConfigurator, Create a "MetaStreetArt" Zone (the zone must have the same name as the Zone Name in Server Connection Data)
- Add cube_spawner to the Public Room Group in the Zone Configurator (same name as zone group in Unity)
- Remove default from Default Room Group
- Set the max number of variables to 20 (or more)
- Augment maximum room's name length to 20 (or more)

### Client setup instructions

 - You must be on the same local network than the server, and have the server launched.
 - Login with the server ip and the name
 - If no room, click on start game
 - Go on the room with the icon on the left

### Controls

ZQSD Move
Space Jump
Mouse look
Left click apply stamp
Right click select stamp
A/E Rotate stamp
F flip stamp
Scroll wheel change size stamp
Shift run
C add cube
Escape/P display mouse

### Know issues

There is some freeze when connecting to the server.
The order of decals is not assured when adding stamps on others.
Some graphics artefacts can appear on some models.
There is a distance and size limit.
No resilient test have been done on the server side to know number of maximum players and objects, or others network problems.

_____________________________________________________________________



# Documentation of original Smartfox Metavers (Template for Unity)

## Introduction

### What is it ?

This project is a template for making an online game using Smartfoxserver and Unity.

The base of this template permits you to connect to a zone, create and connect to a room, have a few choices on the player appearance and spawn cubes. It has been made so that you have all the basic bricks to make a real game using Smartfoxserver and Unity.

> _Bonus : If you have access to the CNAM-ENJMIN server and can transfer your files on it you will be able to host your game rooms from anywhere for free !_

### Context

We are students from the engineer "**Informatique et Multimedia**" formation at the CNAM-ENJMIN and this project has been developed for a "Metavers" project class. This is based on the basic examples from the [smartfoxserver website](http://docs2x.smartfoxserver.com/ExamplesUnity/introduction) and was made by _Ulysse Regnier_, _Etienne Moulin_ and _Victor Mommalier_.

This project should be upgraded in the future to make the rooms connect to each other and permit a ton of users to connect to it without any lag. For now, this is just a basic template of an online video-game.

> **Warning : Smartfoxserver uses encrypted connections and different security protocols that we didn't implement in the template yet !**

## Understanding the template

### Installation

This project uses (basic installations to launch the template) :

- [Unity 2021.3.11f1](https://download.unity3d.com/download_unity/0a5ca18544bf/UnityDownloadAssistant-2021.3.11f1.exe) for the client. You can also use a newer version of Unity and it should work just fine.
- [Smartfoxserver v2.18.0](https://www.smartfoxserver.com/download/get/306) for the server. You can install it on your computer or a server. For the example here or your personal tests, I recommend you to install it on your personal computer. For the sake of simplicity, the following will suppose your server is hosted on your local computer (same as the client).

To develop your own game, you will also need :

- A **Java compiler**, for server extensions. Such as _JetBrains IntelliJ IDEA_ or _IBM Eclipse_.
- A **C# IDE**, compatible with Unity to make your client side. Such as _JetBrains Rider_ or _Visual Studio_.

### Server architecture reminder

Once you installed smartfoxserver on your computer, you can launch the `sfs2x-service` program. This will launch the server in the background. You will then be able to access the smartfoxserver interface by typing [127.0.0.1:8080](http://127.0.0.1:8080) in your web browser.

You can use the [smartfoxserver documentation](http://docs2x.smartfoxserver.com/) to understand clearly how the system works but basically :

- The server is divided into **zones** to separate clearly the different uses and games (an MMO game zone, a chat zone, etc.). In this template, you will not create zones and will always work on the same zone called `Metavers`. This has been chosen to make it simpler and only manage room extensions.

  > If you want to make it a little more challenging you can try creating different zones and make custom zone extensions. But this template is not made for this.

- Each zone is divided into different **rooms**. A room is where the game happens. A room can be used for any kind of game, chat, lobby or anything you want to make really. All the players that will play together have to connect to the same room to interact.
- Rooms are grouped into **room groups** symbolised by an ID. This template uses the `cube_spawner` room group because the game is "CubeSpawner". Each room created with this example will be in this group and the lobby will only show the rooms registered in this group. You can separate your different games with this group idea. If your game is called "Call of Duty", you should name your room group `call_of_duty`.

In the [administration tool](http://127.0.0.1:8080/admin/), you can manage the server configuration. You can create zones and rooms and restart the server.

> **To make the template work, you must create a zone in the _zone configurator_ tab.** You can play with the different settings but know that the template has been made with the default zone settings and the zone name `Metavers` (if you use a different name, you will need to change it in the client data). Once you restart the server using the yellow arrow in the administration tool, the zone will be automatically created.
>
> Be careful with restarting the server because it will **delete all the zones and the rooms created**. If a zone or a room has been created with the zone configurator, it will be recreated but all the data stored in those will be lost.

### Connection between the server and the client

From the client side in Unity, there is three scenes :

- The **Login** scene, which handles the connection with the smartfox server.
- The **Lobby** scene, which shows all the rooms you can connect to (within the room group subscribed) and offers you to create rooms.
- Once you connect to a room, you enter the **Game** scene, where you can say hello to your friends.

From the server side, the zone `Metavers`should be created to host your rooms. The rooms will automatically be created from client requests.

Every room can have an extension script developed in Java. This permits you to make custom interactions between the client and the server and store custom data. For example, when the client requests the server to spawn a cube, the `CubeSpawnerRoomExtension` class catches the request and add a cube to the room. The native MMO API from smartfox will then automatically send the position of the cube to the nearest players. But you still need to code the creation of the cube data server side. The room extension is usually used to build the game logic, so you will probably need to make your own.

To make it work, you need to make a `.java` file, you can use the `CubeSpawnerRoomExtension` class as a base to make yours and build it into a `.jar` file. You will then need to transfer this file to the server under `.../SFS2X/extensions/` in your smartfoxserver installation path.

All the explanations to make a java extension for smartfoxserver can be found [here](http://docs2x.smartfoxserver.com/ExtensionsJava/quick-start). And all the explanations to build a `.jar` from a `.java` file using _JetBrains IntelliJ IDEA_ can be found [here](https://www.jetbrains.com/help/idea/working-with-artifacts.html#examples).

> If you configured your client well, when creating a room the server will load your java extension automatically. At this moment, you should always watch your logs under `.../SFS2X/logs/smartfox.log` to see if the extension loads correctly or throws some exceptions. You should also keep an eye on it at any time when you test your connections and your game because any error could be from the client or from the server extension, you never know.

### The MMO API

Last thing you need to know before starting creating your games using this template is the MMO API.

Each room in smartfoxserver can be of three different types : default / game / MMO. The "default" rooms are usually made for online lobbies or chat rooms and the "game" rooms are made for games rooms. After the first versions of smartfox, they decided to add the MMO API to make MMO rooms.

They basically work the same way as the classic game rooms but they add some useful functionalities such as MMO Items or AOI. Those are well explained in the [documentation](http://docs2x.smartfoxserver.com/AdvancedTopics/mmo-rooms).

> I strongly advise you to read this documentation to understand the difference between classical game room and MMO rooms. In fact, the basic data transfer like user and item positions are handled by this API in the template and will not be explained in the following. You can also read the code documentation, it should help you understand how it concretely works.

~ Now that you know how the client is build and how to load a room extension, you should be ready to make a game with the template.

## Creating a game using the template

Ok so if you followed well all the explanations before, you should be able to launch the Unity project `Smartfox Metavers Client`, make a room, connect to it and spawn your first cubes. If you have some issues when trying to do that you should make sure that your server is launched and that your zone exists.

Now, certain basic and important things on how to use the client template :

- All the connection data are grouped into a scriptable object in `Assets/Data/Server Connection Data`. Here you can chose the zone name (should always be the same as the zone made in the zone configurator), the room group (should be one by game), the parameters of the room creation (AOI size and map limits) and the room extension name (make sure that this is correct or your java extension will not be loaded when creating a room).
- The different `SceneController` scripts each manage a scene. You should develop in the `GameSceneController` script the client functionalities of your games and the request and listeners to the server. I advise you to start dividing this class into different scripts for the different functionalities or it will rapidly start to be huge.
- The `GlobalManager` script is loaded automatically and never destroyed between the scenes. It keeps the connection to the server constantly open to avoid a disconnection when loading the lobby or game scene.

Now, you should use the [smartfox documentation](http://docs2x.smartfoxserver.com/) (explanations webpages, server java API, client C# API and Unity examples) and the commented code of the template to make your own game (everything should be well explained in it).

If you need some help, you can always contact us by mail : ulysse.regnier.auditeur@lecnam.net

Good luck and have fun !
