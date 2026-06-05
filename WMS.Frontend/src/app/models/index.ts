export interface Employee {
  employeeId: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  gender: string;
  dateOfBirth: string;
  dateOfJoining: string;
  departmentId: number;
  roleId: number;
  status: string;
  department?: Department;
  role?: Role;
}

export interface Department {
  departmentId: number;
  departmentName: string;
  description: string;
  createdOn: string;
}

export interface Role {
  roleId: number;
  roleName: string;
}

export interface Attendance {
  attendanceId: number;
  empId: number;
  attendanceDate: string;
  checkIn: string;
  checkOut: string | null;
  totalHours: number;
  workMode: string;
  employee?: Employee;
}

export interface Leave {
  leaveId: number;
  empId: number;
  leaveType: string;
  fromDate: string;
  toDate: string;
  reason: string;
  status: string;
  employee?: Employee;
}

export interface Project {
  projectId: number;
  projectName: string;
  startDate: string;
  endDate: string;
  status: string;
  clientId: number | null;
  client?: Client;
}

export interface Allocation {
  allocationId: number;
  empId: number;
  projectId: number;
  assignedOn: string;
  employee?: Employee;
  project?: Project;
}

export interface Client {
  clientId: number;
  clientName: string;
  clientPhoneNumber: string;
  clientLocation: string;
  clientAddress: string;
}

export interface Announcement {
  announcementId: number;
  title: string;
  message: string;
  isActive: boolean;
  createdOn: string;
}

export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
  pagination?: {
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasNext: boolean;
    hasPrevious: boolean;
  };
}

export interface DashboardSummary {
  kpis: KpiCards;
  attendance: AttendanceAnalytics;
  leaves: LeaveAnalytics;
  projects: ProjectAnalytics;
  departments: DepartmentAnalytics;
  clients: ClientAnalytics;
  recentActivities: DashboardActivityLog[];
}

export interface KpiCards {
  totalEmployees: number;
  activeEmployees: number;
  presentToday: number;
  employeesOnLeave: number;
  activeProjects: number;
  activeClients: number;
  totalAllocations: number;
  announcementsPublished: number;
}

export interface AttendanceAnalytics {
  attendanceRate: number;
  presentCount: number;
  absentCount: number;
  monthlyTrend: MonthlyTrend[];
}

export interface LeaveAnalytics {
  pendingCount: number;
  approvedCount: number;
  rejectedCount: number;
  monthlyTrend: MonthlyTrend[];
}

export interface ProjectAnalytics {
  activeCount: number;
  completedCount: number;
  onHoldCount: number;
  statusDistribution: StatusCount[];
}

export interface DepartmentAnalytics {
  employeeCounts: DepartmentCount[];
}

export interface ClientAnalytics {
  activeCount: number;
  inactiveCount: number;
}

export interface MonthlyTrend {
  month: string;
  count: number;
}

export interface StatusCount {
  status: string;
  count: number;
}

export interface DepartmentCount {
  departmentName: string;
  employeeCount: number;
}

export interface DashboardActivityLog {
  auditId: number;
  entityName: string;
  recordId: number;
  action: string;
  description?: string;
  username?: string;
  userRole?: string;
  ipAddress?: string;
  timestamp: string;
}
