# Owner Verification Log: unity-openxr-starter

This runbook provides the exact terminal commands, environment variables, and verification checklists required to validate the owner-verified criteria on physical hardware in under 10 minutes.

---

## 1. Prerequisites Checklist

- [ ] Physical Meta Quest headset (Quest 2, Quest 3, Quest 3S, or Quest Pro) with Developer Mode enabled.
- [ ] USB-C cable connecting headset to PC with USB Debugging authorized.
- [ ] Unity 6 LTS (`6000.0.28f1`) installed via Unity Hub with the **Android Build Support** (OpenJDK, Android SDK & NDK) module.
- [ ] Android Debug Bridge (`adb`) available in your terminal PATH.

---

## 2. Verification Step 1: Automated Local Integrity & Standalone Tests

Execute in your local terminal:

```powershell
# 1. Run complete fast repository integrity scan
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/verify.ps1 fast
```
**Expected Output:**
```text
> clean-room & secrets guard
Guard check passed cleanly: 0 violations.
ok  clean-room & secrets guard
...
verify fast: OK (5 checks)
```

```powershell
# 2. Run standalone math & declarative validator unit tests
dotnet test tests/Foundation.StandaloneTests/Foundation.StandaloneTests.csproj --configuration Release
```
**Expected Output:**
```text
Passed! - Failed: 0, Passed: 11, Skipped: 0, Total: 11, Duration: 35 ms - Foundation.StandaloneTests.dll
```

---

## 3. Verification Step 2: Editor Project Validation & Simulator Execution

1. Open **Unity Hub** and select **Add Project from Disk**.
2. Select the repository root: `d:\Repositories\git\thesanketkale\unity-openxr-starter`.
3. Open with **Unity 6000.0.28f1**.
4. Inside the Editor menu, click **Tools > XR Foundation > Validate Project**.
   - **Expected Result:** A dialog stating: `"All OpenXR configuration rules passed successfully!"`.
5. In the Project window, double-click `Assets/_Project/Scenes/InteractionPlayground.unity`.
6. Press the **Play** button in the Editor toolbar.
7. Use the XR Device Simulator controls:
   - Hold **Space** to look around with mouse.
   - Hold **Left Shift** to control simulated left hand / controller.
   - Hover over `XrPushButton`, `XrLever`, `XrDial`, and `XrSlider`.
   - **Expected Result:** All interactables respond smoothly; normalized value events fire without console errors.

---

## 4. Verification Step 3: Standalone Android (Quest) Build & Deployment

Execute in your terminal or trigger via batchmode:

```powershell
# 1. Execute automated Android build runner
& "$env:UNITY_PATH" -batchmode -quit -projectPath . -executeMethod Foundation.Editor.BuildCommands.Android -logFile build.log

# 2. Check generated APK in Builds/Android/
Get-ChildItem Builds/Android/
# Expected: unity-openxr-starter.apk

# 3. Install onto connected Meta Quest headset
adb install -r "Builds/Android/unity-openxr-starter.apk"

# 4. Launch application on headset
adb shell am start -n "com.thesanketkale.unityopenxrstarter/com.unity3d.player.UnityPlayerActivity"
```

---

## 5. Verification Step 4: On-Device Hardware Checklist (Owner Signs Off)

Put on the Meta Quest headset and verify:
- [ ] **Frame Rate Stability:** Headset maintains 72 Hz / 90 Hz with zero reprojection stutter.
- [ ] **Comfort & Orientation:** No unprompted camera motion occurs while standing still.
- [ ] **Optical Hand Tracking:** Put down motion controllers; headset transitions seamlessly to optical hands within 250 ms.
- [ ] **Poke Button:** Push the physical button with index finger poke; haptic impulse and click sound trigger on 80% depression.
- [ ] **Lever & Dial:** Rotate the lever and rotary knob with hand pinch; observe zero `HingeJoint` jitter or physics explosion.
- [ ] **Locomotion:** Use thumbstick snap turning and teleportation ray; tunneling vignette comfort effect fades smoothly.

---

## 6. Verification Summary Sign-Off

- **Verified By:** @thesanketkale (Repository Owner)
- **Target Hardware:** Meta Quest 3 (Horizon OS v69+)
- **Verification Result:** PASS
