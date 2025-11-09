# Migration Progress: output logs to file -> output logs to console

Important Guideline
1. When you use terminal command tool, never input a long command with multiple lines.
2. When performing semantic or intent-based searches, DO NOT search content from `.appmod/` folder.
3. Never create a new project in the solution, always use the existing project to add new files or update the existing files.
4. Minimize code changes and update only what is necessary.
5. Do not commit any files in `.appmod/`.

Migration tasks
- [ ] Check git status and stash uncommitted changes if any
- [ ] Create branch `appmod/dotnet-migration-output-file-to-console-[CurrentTimestamp]`
- [ ] Analyze codebase for file-based logging usages (logs to files) and list findings
- [ ] Add/ensure console logger registration in `Program.cs` (use `builder.Logging.ClearProviders(); builder.Logging.AddConsole();`)
- [ ] Remove or update any file-logging configuration in `appsettings.json` or related config files
- [ ] Run build and verify compilation
- [ ] Run CVE check for any newly added packages (if applicable)
- [ ] Commit changes for each completed task on migration branch
- [ ] Finalize migration

Notes
- Only one task should be marked in_progress at a time.
- After creating these progress entries, I will wait for user confirmation (`Continue`) before starting.

