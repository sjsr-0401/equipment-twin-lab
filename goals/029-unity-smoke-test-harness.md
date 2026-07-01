# Goal 029: Unity Smoke Test Harness

## Objective

Add a repeatable local Unity smoke test for the visible process demo.

## Scope

- Unity Editor smoke-test menu
- Unity batchmode smoke-test entry point
- PowerShell runner script
- smoke-test checklist documentation
- CI file-presence checks
- daily work log and loop-state updates

## Architecture

```text
scripts/Invoke-UnitySmokeTest.ps1
    -> Unity.exe -batchmode -executeMethod
    -> MolyAldEditorSmokeTest.RunBatchSmokeTest
    -> sample timeline parse
    -> generated demo scene/component validation
    -> EQUIPMENT_TWIN_UNITY_SMOKE_TEST_PASS
```

## Validation boundary

This goal can validate the script and repository wiring without an active Unity license. Actual Unity Editor compilation still requires Unity Hub sign-in/license activation.

## Expected manual follow-up

1. Sign in to Unity Hub.
2. Activate the Unity Editor license.
3. Run `.\scripts\Invoke-UnitySmokeTest.ps1`.
4. If batchmode still fails, open the project and use `Equipment Twin > Run Moly ALD Smoke Test`.
