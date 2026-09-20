# Security Policy

Please report security issues privately to the repository owner instead of opening a public issue.

Do not include private signing keys, PFX/P12 files, DPAPI backups, API keys, access tokens, chat history, user settings, model files, or machine-specific paths in issues or pull requests.

ReVitAI Bridge uses a current-user Named Pipe. Mutating operations run in Revit transactions, dry-run operations roll back, and failures must be reported as rollback. Release signing material is kept outside the repository.