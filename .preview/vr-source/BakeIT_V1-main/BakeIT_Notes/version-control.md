# BakeIT Version Control

## Repository boundary

The Git repository root is `C:\UnityProjects\BakeIT_V1`.

## Track in Git

- `BakeITCapstoneProject/Assets`, including all `.meta` files.
- `BakeITCapstoneProject/Packages`.
- `BakeITCapstoneProject/ProjectSettings`.
- BakeIT source code, scenes, prefabs, materials, input assets, and required licensed project assets.
- `.gitignore`, `.gitattributes`, project instructions, and `BakeIT_Notes`.

## Ignore in Git

- Unity-generated `Library`, `Temp`, `Obj`, `Logs`, and `UserSettings` directories.
- Build and exported-project output.
- Generated IDE projects and local editor settings.
- Local Plastic SCM workspace metadata.
- Local Codex or agent state.
- Operating-system metadata and application backup files.
- The local capstone manuscript DOCX.

## Important constraint

A Unity repository cannot be reconstructed from `.cs` files alone. Scenes, prefabs, materials, settings, package manifests, and `.meta` GUID files are part of the source and must remain versioned.

## Large files

Git LFS is not configured by this change. The initial trackable set has no file larger than 50 MB; its largest file is approximately 4.51 MB. Add LFS later only if repository growth or the selected remote hosting limits make it useful.

## Line endings

`.gitattributes` automatically detects text files and keeps them as LF. This prevents Windows `core.autocrlf=true` from rewriting Unity YAML and source files to CRLF while leaving binary assets untouched.

## Initial staging validation

`git add --dry-run .` succeeds from the repository root. Unity `Temp` files and nested `.plastic` workspace metadata are excluded. The initial files remain unstaged until the first commit is deliberately prepared.
