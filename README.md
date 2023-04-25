# MultiScaled Portal

PORTAL (PO widget for Remote Target Acquisition and controL) is designed to leverage the secondary view interaction to allow the user to directly select and manipulate remote objects using simple virtual hands.
These portals can be cast across different rooms with different scales and positions.

### Setup
- PORTAL is fully funtional in Unity.2019.4.35f1. So, we recommend you to test it in the same version of Unity. If you have a different version, it might not work properly.
- You can import the PORTAL unity package by Assets -> Import Package -> Custom Package ... in the Unity window tabs.
- Please re-install SteamVR. The PORTAL unity package already contains it. However, if it does not work, please re-install SteamVR from Untiy Asset Store.
- If SteamVR is successfully installed, Go to Edit -> Project Settomgs -> XR Plug-in Management, and check your Plug-in Provider is OpenVR Loader.
- Next, go to Edit -> Project Settomgs -> XR Plug-in Management -> OpenVR and set the Stereo Rendering Mode as Multi Pass.

### Usage
1. Open the prpject
2. Seach for "MultiScaled" game object within the Assets Folder
3. Correctly set desired features within "PortalManager" instances in the scene tree.

### Features
#### Navigation
- Users can walk around freely within VR space. If they collide with a portal, they will be teleported to the other side.
- Creating a portal with the portal hands, and create a portal within an existing portal
  - PortalManager --> "isAllowCreatingPortalInPortal"
 - Should the portal plane scale the image to match the current scale? If not, show the real scale.
    - PortalManager -> "shouldScalePortal"

#### Manipulation
- When objects are grabbed and moved between the portals, they will preserve their percieved scale
  - PortalManager -> "isAllowObjectTransportThroughPortal"
- Can users grab the portal with two hands and manipulate it directly.
  - PortalManager -> "isAllowPortalPlainManipulation"

Forked from https://github.com/VIZ-US/PORTAL
