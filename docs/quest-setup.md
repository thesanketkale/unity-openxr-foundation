# Meta Quest Setup & Deployment Guide

This guide covers preparing your Meta Quest headset (Quest 2, Quest 3, Quest 3S, Quest Pro), enabling developer mode, running ADB commands, and deploying builds.

---

## 1. Enabling Developer Mode

1. **Register as a Meta Developer:** Visit [developer.oculus.com](https://developer.oculus.com/) and register an organization.
2. **Meta Horizon Mobile App:**
   - Open the Meta Horizon app on your mobile phone (iOS / Android).
   - Ensure the headset is powered on and connected via Bluetooth.
   - Navigate to **Menu > Devices > [Your Headset] > Headset Settings > Developer Mode**.
   - Toggle **Developer Mode** to **ON**.
3. **USB Debugging Authorization:**
   - Connect the headset to your development machine using a high-speed USB-C cable (USB 3.0 recommended).
   - Put on the headset and look for the prompt: **"Allow USB Debugging?"**.
   - Check **"Always allow from this computer"** and select **Allow**.

---

## 2. ADB Command Reference

Verify your device connection and check running logs using Android Debug Bridge (ADB):

```bash
# Verify connected headset
adb devices
# Expected output:
# List of devices attached
# 2G0YC1JF000000    device

# Install compiled APK
adb install -r -d "Builds/Android/unity-openxr-starter.apk"

# Launch application on headset
adb shell am start -n "com.thesanketkale.unityopenxrstarter/com.unity3d.player.UnityPlayerActivity"

# Stream live Unity logcat output (filtered to Unity engine logs)
adb logcat -s Unity ActivityManager PackageManager VRAPI

# Wireless ADB Setup (Untethered Testing)
# Step 1: Connect via USB, then set TCPIP port
adb tcpip 5555
# Step 2: Disconnect USB cable, then connect via headset IP address
adb connect 192.168.1.100:5555
# Step 3: Verify wireless connection
adb devices
```

---

## 3. Project Validation Before Building

Always execute the automated validator before building:

In Unity Editor:
- Navigate to **Tools > XR Foundation > Validate Project**.
- Ensure all 8 validation vectors display green checkmarks.

In Command Line / CI:
```powershell
# Headless batchmode validation
& "$env:UNITY_PATH" -batchmode -quit -projectPath . -executeMethod Foundation.Editor.XrProjectValidator.ValidateBatch -logFile validator.log
```

---

## 4. Building and Running from Unity Editor

1. Open **File > Build Settings**.
2. Ensure Platform is set to **Android** (with Texture Compression `ASTC`).
3. Set **Run Device** to your connected Quest headset.
4. Click **Build and Run**.
