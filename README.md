### This is a fork from the original project [UWPHook](https://briano.dev/UWPHook/) of BrianLima that I maintain on my side following some issues and the unavailability of him.
All properties of the source code belong to BrianLima.

# UWPHook

[![](http://imgur.com/gWwR02D.png)](https://briano.dev/UWPHook/)

Small project to link UWP games and XboxGamePass to Steam

Since it's not possible to run directly an XBOX Games to Steam, this application will act as a middleware. 
While your game is running through this application via the steam shortcut created by the application, it will showing you "playin as [game]" for your friends.

# To add UWP or XGP games to Steam #

[Download the latest version of UWPHook](https://github.com/stevealexandre/UWPHook/releases) and install it on your PC.

![](https://i.imgur.com/pjGfGHe.png)

Select every app you want to add to Steam and press "Export selected apps to Steam", every app you selected will be added to Steam and your Steam will be automatically restart with your new games added.

![](https://i.imgur.com/on46CMQ.png)

Close UWPHook, click play on your UWP game, and Steam will show your current game on your status as long as you are playing it!

----------

# SteamGridDB #

Since v2.8, UWPHook can automatically import grid, icons and hero images from [SteamGridDB](https://www.steamgriddb.com)

On your first usage, the app will ask you if you want it to download images, redirecting you to the settings page.

![](https://i.imgur.com/K0Cm4IL.png)

By adding a API Key obtained in the SteamGridDB preferences, UWPHook will try to find matching images for any exported games, giving you the following result:

![](https://i.imgur.com/mlvVdwb.png)

You can refine the images by using filters for animated images, blurred, no logo or memes for example, but it will always pick the first it finds for the filters automatically.

Special thanks to @FusRoDah061 for implementing the base feature!

# Troubleshooting #

- **Steam's Overlay isn't working!**
  - Unfortunately, it's a Steam limitation, Valve has to update it in order to work properly with UWP, DXTory is a recommended overlay for UWP games.
- **Steam Link launches the game but input doesn't work!**
  - Unfortunately, another limitation by Steam, i have some ideas as to why it isn't working, but i can't give an ETA for when i can fix this, or even if it's fixable on my end, since Valve didn't released the Link in Brazil and i can't get one to test.
- **Steam Controller isn't working**
  - Another limitation by Steam, some people reported it works with "Desktop Mode" configuration, but i can't verify this.
- **My question isn't listed here!**
  - Ask a question over there, maybe someone will help you, i surely will as soon as i can
 **[https://github.com/stevealexandre/UWPHook/issues)**

----------

# Building #

- Clone project or forked project.
- Install Visual Studio 2019 with .NET Framework 4.6.2.
- Press run!

----------

# About #

This software is open-source under the MIT License.

If you like what i did with it and want to suport me, you can cheer me up at my [Twitter](http://www.twitter.com/brianostorm "Twitter") or [pay me a coffee via Paypal, it will help me to continue to build amazing open source tools for you!"](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=9YPV3FHEFRAUQ)

Thanks for your support, and game on!
