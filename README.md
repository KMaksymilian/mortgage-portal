# Branching Conventions

In our project, we follow the Git branching conventions below to keep code organized and facilitate team collaboration:

## 1. `main`
- The main branch of the project.
- Always contains stable, production-ready code.
- Only merged via pull requests after review.

## 2. `develop`
- Integration branch for new features.
- All feature branches are merged here before moving to `main`.
- Code should be stable but may contain unfinished functionalities.

## 3. `feature/*`
- Branches for new features or major changes.
- Naming: `feature/feature-name`.
- Created from `develop`.
- Once done, merged back into `develop`.

## 4. `bugfix/*`
- Branches for fixing bugs during development.
- Naming: `bugfix/short-bug-description`.
- Created from `develop` and merged back into `develop` after the fix.

## 5. `release/*` (Staging)
- Branches for preparing a new release.
- Naming: `release/vX.Y.Z`.
- Created from `develop` when a release is near.
- Used for final testing, bug fixes, and version updates.
- After testing, merged into both `main` (production) and `develop` (to keep hotfixes).

## 6. `hotfix/*`
- Branches for critical fixes on `main`.
- Naming: `hotfix/short-description`.
- Created from `main` and merged back into both `main` and `develop`.

## Summary
- **Create feature/bugfix/release/hotfix branches from the appropriate base branch.**
- **Merge only via pull requests and after testing.**
- This ensures every team member knows where to implement changes and where to find stable code.
