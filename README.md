<div align="center">

<!-- Animated Banner -->
<img src="https://readme-typing-svg.demolab.com?font=JetBrains+Mono&weight=700&size=28&duration=3000&pause=1000&color=0057FF&center=true&vCenter=true&width=700&lines=🏢+Workforce+Management+System;Enterprise-Grade+HR+Platform;ASP.NET+Core+%7C+Angular+%7C+Azure" alt="WMS Banner"/>

<br/>

![.NET](https://img.shields.io/badge/.NET_8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![Azure](https://img.shields.io/badge/Azure_DevOps-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Python](https://img.shields.io/badge/Python-3776AB?style=for-the-badge&logo=python&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)

<br/>

> An **enterprise-grade, cloud-hosted HR and Workforce Management System** built as a full-stack capstone project.  
> Streamlines employee onboarding, real-time attendance tracking, leave management workflows, and timesheet generation —  
> powered by a secure **C# REST API**, a responsive **Angular SPA**, and automated **Azure CI/CD pipelines**.

<br/>

[![GitHub repo](https://img.shields.io/badge/GitHub-hariom710%2FWMS--Project-181717?style=flat-square&logo=github)](https://github.com/hariom710/WMS-Project.git)
[![Portfolio](https://img.shields.io/badge/Portfolio-hariombalang.netlify.app-0057FF?style=flat-square&logo=netlify)](https://hariombalang.netlify.app)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-hariombalang-0A66C2?style=flat-square&logo=linkedin)](https://linkedin.com/in/hariombalang)

</div>

---

## ✨ Key Features

<table>
<tr>
<td>

### 🔐 Role-Based Auth
- **JWT Bearer tokens** protect every API endpoint
- **BCrypt password hashing** (enterprise-standard)
- Dual roles: **Admin (HR/Manager)** and **Employee**
- Auto-provisioned login on employee registration

</td>
<td>

### 👥 Employee Management
- Full **CRUD** with search by name, ID, role, department
- **Python bulk importer** — streams CSV records through the API while triggering server-side hashing
- Auto 0.2s throttling respects Azure App Service limits

</td>
</tr>
<tr>
<td>

### ⏱️ Attendance & Timesheets
- **Real-time clock in/out** with exact second-level tracking
- **UTC → IST** timezone handling on the frontend
- **QuestPDF** generates downloadable styled timesheet PDFs

</td>
<td>

### 🏖️ Leave Workflow
- Employees apply for **Sick / Casual / Earned** leave
- Manager approval portal — approve or reject in real-time
- Full leave history and cancellation support

</td>
</tr>
<tr>
<td>

### 📊 Real-Time Dashboard
- KPI summary cards with live counts
- **Chart.js** attendance and leave statistics charts
- **RxJS BehaviorSubject** for reactive state management

</td>
<td>

### 🚀 Azure CI/CD
- Git push to `main` triggers full Azure DevOps pipeline
- Stages: **restore → build → test → publish → deploy**
- Zero-downtime deployment to Azure App Services

</td>
</tr>
</table>

---

## 🛠️ Technology Stack

| Layer | Technologies |
|---|---|
| **Frontend** | Angular 17, TypeScript, Angular Material, Bootstrap, RxJS, Chart.js |
| **Backend** | ASP.NET Core 8 Web API (C#), EF Core Code-First, AutoMapper, xUnit |
| **Security** | JWT Bearer, BCrypt.Net, Data Annotations, Role Guards |
| **Reporting** | QuestPDF (PDF timesheet generation) |
| **Database** | Azure SQL Server, EF Core Migrations, 10+ normalized tables |
| **Cloud & DevOps** | Azure App Service, Azure SQL, Azure DevOps CI/CD YAML pipelines |
| **Automation** | Python 3 (Pandas, Requests) — bulk CSV ingestion via REST API |

---

## 🏗️ Architecture Flow

```
┌─────────────────────────────────────────────────────────────┐
│  CLIENT TIER   Angular SPA  →  JWT Bearer in HTTP headers   │
└──────────────────────────┬──────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│  API TIER      ASP.NET Core Web API                         │
│                Validate JWT → Business Logic → EF Core      │
│                Repository + Service Pattern · AutoMapper    │
└──────────────────────────┬──────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│  DATA TIER     Azure SQL Database                           │
│                10+ tables · FK constraints · Audit Log      │
└──────────────────────────┬──────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│  DEVOPS TIER   Azure DevOps                                 │
│  git push main → Restore → Build → Test → Publish → Deploy  │
└─────────────────────────────────────────────────────────────┘
```

---

## 📂 Project Structure

```
/WMS-Solution
 ├─ /WMS.API             → ASP.NET Core Web API (Controllers, Middleware, Auth)
 ├─ /WMS.Application     → Services, DTOs, Validation, AutoMapper profiles
 ├─ /WMS.Domain          → Entities, Interfaces, Domain logic
 ├─ /WMS.Infrastructure  → EF Core DbContext, Migrations, SQL Server
 ├─ /WMS.Frontend        → Angular SPA
 │    └─ /src/app
 │         ├─ /auth          → Login, JWT interceptor, Route guards
 │         ├─ /employees     → Employee CRUD module
 │         ├─ /attendance    → Clock in/out, timesheet view
 │         ├─ /leaves        → Leave application and approval
 │         ├─ /dashboard     → KPI cards and Chart.js widgets
 │         └─ /shared        → Reusable components, services
 ├─ /WMS.Tests           → xUnit unit tests
 ├─ /WMS.DevOps          → Azure Pipelines YAML scripts
 └─ /Automation          → Python bulk importer (import_via_api.py)
```

---

## 💻 Local Development Setup

### Prerequisites

| Tool | Version |
|---|---|
| .NET SDK | 8.0 |
| Node.js | LTS |
| Angular CLI | Latest |
| SQL Server | 2019 / Azure SQL |
| Visual Studio | 2022 |

### 1 — Database

```powershell
# In Visual Studio Package Manager Console
Update-Database
```

### 2 — Backend API

```bash
# 1. Open WMS-Solution.sln in Visual Studio 2022
# 2. Update appsettings.json with your SQL connection string
# 3. Run project — Swagger opens at:
#    https://localhost:<PORT>/swagger
```

### 3 — Angular Frontend

```bash
cd WMS.Frontend
npm install
# Update environments/environment.ts with your local API URL
ng serve
# Navigate to http://localhost:4200
```

---

## 🐍 Enterprise Bulk Upload (Python)

The Python importer streams large employee datasets through the live API — preserving all server-side business logic like BCrypt password hashing — rather than bypassing application rules with a direct DB insert.

```bash
cd Automation
# 1. Ensure employees.csv is correctly formatted
# 2. Update import_via_api.py with your active Admin JWT token
python import_via_api.py
# Built-in 0.2s throttle respects Azure App Service compute limits
```

---

## 📋 Module Summary

| Module | Functionality | Pattern |
|---|---|---|
| **Employee** | CRUD, role assignment, auto-provisioned login, bulk import | Repository + Service |
| **Attendance** | Clock in/out, UTC→IST, monthly view, QuestPDF timesheets | EF Core + QuestPDF |
| **Leave** | Apply, cancel, manager approve/reject workflow | Approval workflow |
| **Projects** | Assign employees, client mapping, status tracking | Many-to-many allocation |
| **Auth** | JWT login/logout, BCrypt hashing, role-based guards | JWT + BCrypt |
| **Dashboard** | KPI cards, Chart.js charts, real-time stats | RxJS BehaviorSubject |
| **Department** | CRUD with employee associations | Repository |
| **DevOps** | Azure CI/CD — automated build, test, deploy pipeline | Azure DevOps YAML |

---

## 🗃️ Database Schema (Key Tables)

<details>
<summary>Click to expand — 10 tables</summary>

- **Employee** — `EmployeeId, FirstName, LastName, Email, DepartmentId, RoleId, Status`
- **Department** — `DepartmentId, DepartmentName, Description`
- **Role** — `RoleId, RoleName (Admin/Manager/Employee)`
- **Attendance** — `AttendanceId, EmpId, CheckIn, CheckOut, TotalHours, WorkMode`
- **Leave** — `LeaveId, EmpId, LeaveType, FromDate, ToDate, Status, ApprovedBy`
- **UserLogin** — `UserId, Username, PasswordHash (BCrypt), RoleId, LastLogin`
- **Project** — `ProjectId, ProjectName, ClientId, StartDate, EndDate, Status`
- **Client** — `ClientId, ClientName, ClientAddress, Status`
- **EmployeeProject** — `AllocationId, EmpId, ProjectId, AssignedOn, Status`
- **AuditLog** — `AuditId, EntityName, RecordId, Action, CreatedBy, CreatedOn`

</details>

---

## 👤 Author

**Hariom Ashok Balang**

Trainee Analyst @ Capgemini · BTech Computer Technology, YCCE Nagpur (2022–2026)

| Platform | Link |
|---|---|
| 🌐 Portfolio | [hariombalang.netlify.app](https://hariombalang.netlify.app) |
| 💼 LinkedIn | [linkedin.com/in/hariombalang](https://linkedin.com/in/hariombalang) |
| 🐙 GitHub | [github.com/hariom710](https://github.com/hariom710) |
| 📧 Email | hariombalang@gmail.com |

---

<div align="center">

*Built with ☕ and enterprise architecture.*

</div>
