# Network Change Reactor
A windows service that "Reacts" to network connection changes and then performs the pre-configured tasks.
For the time being, on detecting a network, it only opens the applications specified in the configuration

E.g. let's say your company's Intranet is accessible only from your office network.
This can thus tell the service that you have checked in into the office and hence automatically kick start some applications e.g Email Client; Google Chrome; Vim; with which you start your day

### How to install
1. Run the installer in Dist folder that will install the latest stable version.

  OR

2. Build the installer from the code and install (if you want to play with the code first)

### Usage
1. In the installation folder open ReactorService.exe.config file.
2. Change the value of HOST_TO_CHECK attribute to a specific host name or IP address
that is going to be reachable from the changed network state.
3. Change the value of APPS_TO_START to semi-colon separated executables path

### Coming Soon...
1. A simple UI for selecting application which you want to open
2. System tray Settings icon to configure settings

