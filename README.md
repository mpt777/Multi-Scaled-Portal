# MultiScaled Portal

PORTAL (PO widget for Remote Target Acquisition and controL) is designed to leverage the secondary view interaction to allow the user to directly select and manipulate remote objects using simple virtual hands.
These portals can be cast across different rooms with different scales and positions.

### Setup
- PORTAL is fully funtional in Unity.2019.4.35f1. So, we recommend you to test it in the same version of Unity. If you have a different version, it might not work properly.
- You can import the PORTAL unity package by Assets -> Import Package -> Custom Package ... in the Unity window tabs.
- Please re-install SteamVR. The PORTAL unity package already contains it. However, if it does not work, please re-install SteamVR from Untiy Asset Store.
- If SteamVR is successfully installed, Go to Edit -> Project Settomgs -> XR Plug-in Management, and check your Plug-in Provider is OpenVR Loader.
- Next, go to Edit -> Project Settomgs -> XR Plug-in Management -> OpenVR and set the Stereo Rendering Mode as Multi Pass.

## Usage
1. Open the project
2. Seach for "MultiScaled" game object within the Assets Folder
3. Correctly set desired features within "PortalManager" instances in the scene tree.

## Features
- There are 3 prebuilt rooms within the asset. There is a room at 0.5x scale, 1x scale, and 2x scale. Each of the rooms can be scaled to any arbitrary size by modifying the "Room" object within the scene tree.
- The left hand is tied to the 0.5x room which the right hand is tied to the 2x room
- Users can walk around freely within VR space. If they collide with a portal, they will be teleported to the other side.


## Parameters

**isAllowCreatingPortalInPortal**
- Parameter. Can the teleported hands create their own portals? 
  
**shouldScalePortal**
 - Parameter. Should the portal plane scale the image to match the current scale? If not, show the real scale.
 
**portalTechnique**
 - Parameter. How should the target portal movement be handled?
       - Stationary: Target portal does not move
       - Joystick: Joystick moves the position of the target portal
       
**rotationTechnique**
  - Parameter. How should the target portal rotation be handled?
       - Stationary
       - Joystick: Use the rotation hand's joystick to rotate the portal. Disallows movement with the joystick.
       - Button Toggle: The rotation hand joystick moves the position or rotation based on the (x/b) button presses
       - Rotation: Use the rotation hand to rotate the portal incrementally.

**isAllowObjectTransportThroughPortal**
- Parameter. Objects can always be manipulated through the portal. If this is true, then users can pull objects through the portal.

**isAllowPortalPlainManipulation**
- Parameter. Can users grab the portal directly and set the scale, position, and rotation of the origin portal using 2 handed transformations.


Forked from https://github.com/VIZ-US/PORTAL
