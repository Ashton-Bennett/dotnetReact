namespace Api.Enums
{
    public enum UserRole
    {
        Admin = 1,    // Full access to all resources
        Manager = 2,  // View + edit within their department
        User = 3,     // View/edit only their own data
        Guest = 4     // Read-only, limited access
    }
}
