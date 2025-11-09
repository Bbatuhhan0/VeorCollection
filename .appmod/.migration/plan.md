# Migration Plan: output logs to file -> output logs to console

Technology X: Output logs to file
Technology Y: Output logs to console
KnowledgeBase: #ConsoleLoggingKnowledgeBase

Summary / analysis
- Repository scanned: `VeorCollection` ASP.NET Core web app.
- Files inspected (representative): `Program.cs`, `VeorCollection.csproj`, `appsettings.json`, `appsettings.Development.json`, `Controllers/HomeController.cs`, `Views/Shared/Error.cshtml`, and `wwwroot/**/*` static assets.
- Findings: No explicit file-based logging sinks or file I/O for logging were found (no usages of `Serilog` with `WriteTo.File`, `NLog` file targets, `loggerFactory.AddFile`, `System.IO.File.WriteAllText`, `StreamWriter`, or custom file logger providers). `appsettings.json` only contains the standard `Logging` section. The app uses `Microsoft.Extensions.Logging` via the generic host (default providers).

Migration goal
- Ensure the application emits logs to the console (recommended for cloud hosting) and remove/replace any file-based logging if present.
- Keep the existing project framework and build targets.
- Only perform code/config changes required for compilation and switching logging to console.

Planned tasks
1. Read the Console Logging knowledge base and collect any recommended package dependencies (none required in this project unless a third-party logger is introduced). (Reference: #ConsoleLoggingKnowledgeBase)
2. Confirm there are no file-based logging usages in the codebase. If any are discovered, list them and plan replacements. (scanned files listed above)
3. Add an explicit console logger configuration in `Program.cs` if not already present (minimal change: ensure `builder.Logging.ClearProviders(); builder.Logging.AddConsole();` is used so only console output is active, or keep defaults if acceptable).
4. Remove or disable any file-logging configuration in `appsettings*.json` (none found; include a step that would remove any file sink sections if present).
5. Run build and verify compilation succeeds.
6. CVE check of any packages added during migration (none expected). If packages are added, run vulnerability check.
7. Commit each completed task on a new branch `appmod/dotnet-migration-output-file-to-console-[CurrentTimestamp]` (create branch after stashing uncommitted changes).

Files likely to be modified
- `Program.cs` (add/ensure console logger registration)
- `appsettings.json` / `appsettings.Development.json` (remove file sink config if present)

Version control flow (to be executed before making code changes)
1. Check git status.
2. If uncommitted changes exist, stash them (include untracked) except `.appmod/`.
3. Get current timestamp `yyyyMMddHHmmss` and replace `[CurrentTimestamp]` in the branch name.
4. Create branch: `appmod/dotnet-migration-output-file-to-console-[CurrentTimestamp]`.

Notes / rationale
- Cloud-friendly logging uses console output as the host captures stdout/stderr. File logging is brittle in cloud environments.
- No third-party logging libraries are required for a console-only migration. If the project later adopts structured logging, Serilog (console sink) is an option.

Deliverables
- `.appmod/.migration/plan.md` (this file)
- `.appmod/.migration/progress.md` (progress tracking with checklist)

Next step
- I created this plan file and a `progress.md` (progress file created alongside). I will pause here. Type `Continue` to instruct me to:
1) Inspect git status and perform the VCS steps (stash & create branch), and
2) Execute the migration tasks in `progress.md` in order.

