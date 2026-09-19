# Security Model

- The named pipe uses current-user-only access.
- The bridge does not open an HTTP port.
- The discovery file records the local pipe and process.
- Requests can include an expected document GUID.
- Mutating operations must run on the Revit main thread.
- Arbitrary code execution is not part of the tool catalog.
- File paths are resolved by explicit tools only.
- Models, logs and exports are not committed to the repository.
- Review all third-party binaries before creating a release package.
