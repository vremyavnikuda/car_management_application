# Technical Debt Report

This document summarizes the current technical debt of the project. Use it as a checklist for future improvements and refactoring.

## 1. Unused/Obsolete Code and Imports

- [ ] Periodically review and remove dead code and unused imports (e.g., legacy OpenXML code, duplicate Razor sections).
- [ ] Refactor Razor templates to avoid conflicts and duplication.

## 2. Unimplemented UI Features

- [ ] Implement document filtering (by type, status, date) — currently only UI exists.
- [ ] Implement document search — search bar is present but not functional.
- [ ] Add bulk actions (delete, download, status change) for documents.
- [ ] Add user notifications for events (upload, errors, status changes).

## 3. Lack of Automated Tests

- [ ] Add unit tests for business logic and controllers.
- [ ] Add integration and end-to-end tests for critical flows.

## 4. Validation and Error Handling

- [ ] Improve client-side validation (currently minimal or missing).
- [ ] Make error messages more informative for users (especially for file preview and upload).

## 5. Security

- [ ] Centralize and extend role/permission management (currently only partial checks for Admin).
- [ ] Add XSS protection for user-generated content in views.
- [ ] Enforce file size/type restrictions on the server.

## 6. File Management

- [ ] Ensure consistency between database (FileContent) and disk (FilePath) storage.
- [ ] Implement background cleanup for orphaned files on disk.

## 7. Logging

- [ ] Standardize logging format and coverage (log all key actions and errors).

## 8. Documentation

- [ ] Expand README with architecture, DB structure, business logic, and user roles.

## 9. Migrations and DB Structure

- [x] Automate migration checks and application on startup.
- [x] Add scripts for backup and restore.

## 10. UX/UI

- [ ] Hide or implement all non-functional UI elements (filters, search, etc.).
- [ ] Improve mobile responsiveness.

## 11. API and Integration

- [ ] Add REST API for external integrations and mobile apps.

## 12. Test Data and Seeding

- [ ] Add mechanism for generating test/demo data (seeding).

---

**Return to this file regularly to track and reduce technical debt!**
