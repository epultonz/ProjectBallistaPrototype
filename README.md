# ProjectBallistaPrototype
The complete Ballista Duel Prototype. Repo for UWB CSS497 Capstone Multiplayer AR with Environment Interaction

The application is built inside ASL with latest commit of 06/07/19. The ARCore API key can be found in CRCS folder. All ballista duel related files is in BallistaPackage
folder. 

To install the app, download or clone the whole repository.
Use unity version 2018.3 to open from the ASL folder.

1. From the project settings tab, in the ARCore settings, copy paste the ARCore API key and make sure anchor cloud is checked.
2. Copy paste the gamespark API key in the gamespark manager settings.
3. On Build Settings, switch to Android Platform. And open Player Settings.
4. In Player Settings -> Other Settings, check off Multithreaded Rendering, and Minimum API level under Identification is level 24.
5. In Player Settings -> XR Settings, make sure ARCore Supported is checked.
6. Build both the BallitaPorototype and ASL_LobbyScene on the plugged in Android devices.

To play the application, make sure both android device have the application.

1. Log into ASL.
2. After both players clicked Ready, first player click the Host button.
3. Second Player click Resolve/Join button.
4. Then click Spawn to spawn player's ballista object.
